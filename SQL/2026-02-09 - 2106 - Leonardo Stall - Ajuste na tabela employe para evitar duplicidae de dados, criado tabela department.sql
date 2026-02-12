SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRAN;

    ------------------------------------------------------------
    -- 1) Criar catálogo DEPARTMENT (se não existir)
    ------------------------------------------------------------
    IF OBJECT_ID('dbo.department', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.department (
            id INT IDENTITY(1,1) PRIMARY KEY,
            name VARCHAR(60) NOT NULL,
            active BIT NOT NULL CONSTRAINT DF_department_active DEFAULT (1),
            created_at DATETIME2 NOT NULL CONSTRAINT DF_department_created_at DEFAULT (SYSUTCDATETIME()),
            edited_at DATETIME2 NULL
        );
        CREATE UNIQUE INDEX UX_department_name ON dbo.department(name);
    END;

    ------------------------------------------------------------
    -- 2) Semear departamentos padrão (idempotente)
    ------------------------------------------------------------
    ;WITH seed(name) AS (
        SELECT 'Vendas' UNION ALL
        SELECT 'Oficina' UNION ALL
        SELECT 'Atendimento' UNION ALL
        SELECT 'Estoque' UNION ALL
        SELECT 'Administrativo' UNION ALL
        SELECT 'Outros'
    )
    INSERT INTO dbo.department(name)
    SELECT s.name
    FROM seed s
    WHERE NOT EXISTS (SELECT 1 FROM dbo.department d WHERE d.name = s.name);

    ------------------------------------------------------------
    -- 3) Backfill: mapear employee.position -> department
    --    (com fallback para "Outros")
    ------------------------------------------------------------
    -- 3.1) Tabela de mapeamento sugerido (ajuste se necessário)
    DECLARE @Map TABLE (position_name VARCHAR(60), department_name VARCHAR(60));
    INSERT INTO @Map(position_name, department_name) VALUES
        ('Vendedor'  , 'Vendas'),
        ('Gerente'   , 'Administrativo'),
        ('Caixa'     , 'Atendimento'),
        ('Atendente' , 'Atendimento'),
        ('Mecânico'  , 'Oficina'),
        ('Estoque'   , 'Estoque'),
        ('Estoquista', 'Estoque');

    -- 3.2) Garantir "Outros" para posições não mapeadas
    ;WITH Unmapped AS (
        SELECT DISTINCT e.position AS position_name
        FROM dbo.employee e
        WHERE e.position IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM @Map m WHERE m.position_name = e.position)
    )
    INSERT INTO @Map(position_name, department_name)
    SELECT u.position_name, 'Outros'
    FROM Unmapped u;

    -- 3.3) Preencher fk_department para quem está NULL (baseado no mapeamento)
    ;WITH src AS (
        SELECT e.id AS employee_id,
               ISNULL(m.department_name, 'Outros') AS department_name
        FROM dbo.employee e
        LEFT JOIN @Map m
          ON m.position_name = e.position
    )
    UPDATE e
    SET e.fk_department = d.id
    FROM dbo.employee e
    JOIN src s ON s.employee_id = e.id
    JOIN dbo.department d ON d.name = s.department_name
    WHERE e.fk_department IS NULL;

    ------------------------------------------------------------
    -- 4) Garantir que todos agora possuem fk_department
    ------------------------------------------------------------
    IF EXISTS (SELECT 1 FROM dbo.employee WHERE fk_department IS NULL)
    BEGIN
        RAISERROR ('Existem funcionários sem fk_department após o backfill. Ajuste o mapeamento e reexecute.', 16, 1);
    END

    ------------------------------------------------------------
    -- 5) Ajustar nulabilidade de fk_department para NOT NULL
    --    (dropar/recriar objetos dependentes na ordem correta)
    ------------------------------------------------------------
    -- 5.1) Dropar índice dependente, se existir
    IF EXISTS (
        SELECT 1 FROM sys.indexes 
        WHERE name = 'IX_employee_fk_department' 
          AND object_id = OBJECT_ID('dbo.employee')
    )
    BEGIN
        DROP INDEX IX_employee_fk_department ON dbo.employee;
    END

    -- 5.2) Dropar FK, se existir
    IF EXISTS (
        SELECT 1 FROM sys.foreign_keys 
        WHERE name = 'FK_employee_department'
    )
    BEGIN
        ALTER TABLE dbo.employee DROP CONSTRAINT FK_employee_department;
    END

    -- 5.3) Alterar coluna para NOT NULL
    ALTER TABLE dbo.employee ALTER COLUMN fk_department INT NOT NULL;

    -- 5.4) Recriar FK + CHECK (trusted) e índice
    ALTER TABLE dbo.employee WITH CHECK
        ADD CONSTRAINT FK_employee_department
            FOREIGN KEY (fk_department) REFERENCES dbo.department(id);

    ALTER TABLE dbo.employee CHECK CONSTRAINT FK_employee_department;

    CREATE INDEX IX_employee_fk_department ON dbo.employee(fk_department);

    ------------------------------------------------------------
    -- 6) Remover a coluna textual position (se existir)
    ------------------------------------------------------------
    IF COL_LENGTH('dbo.employee', 'position') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.employee DROP COLUMN position;
    END

    ------------------------------------------------------------
    -- 7) Remover tabela POSITION (se existir)
    ------------------------------------------------------------
    IF OBJECT_ID('dbo.position', 'U') IS NOT NULL
    BEGIN
        DROP TABLE dbo.position;
    END

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;