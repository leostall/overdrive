use overdrive;

SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRAN;

    ------------------------------------------------------------
    -- 0) Garantir índices nas FKs (opcional, melhora performance)
    ------------------------------------------------------------
    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes 
        WHERE name = 'IX_stock_fk_branch' AND object_id = OBJECT_ID('dbo.stock')
    )
    BEGIN
        CREATE INDEX IX_stock_fk_branch ON dbo.stock(fk_branch);
    END

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes 
        WHERE name = 'IX_stock_fk_address' AND object_id = OBJECT_ID('dbo.stock')
    )
    BEGIN
        CREATE INDEX IX_stock_fk_address ON dbo.stock(fk_address);
    END

    ------------------------------------------------------------
    -- 1) Mapear filiais existentes + seus endereços
    ------------------------------------------------------------
    ;WITH B AS (
        SELECT id AS branch_id, fk_address, corporate_name
        FROM dbo.branch
        WHERE corporate_name IN (N'Filial Paulista', N'Filial Botafogo', N'Filial Savassi')
    )
    ------------------------------------------------------------
    -- 2) Criar um registro de estoque por filial (se não existir)
    --    Regra: estoque da filial fica no mesmo endereço da filial
    ------------------------------------------------------------
    INSERT INTO dbo.stock (fk_address, fk_branch, created_at)
    SELECT B.fk_address, B.branch_id, SYSUTCDATETIME()
    FROM B
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.stock S
        WHERE S.fk_branch = B.branch_id
          AND S.fk_address = B.fk_address
    );

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;

SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRAN;

    ------------------------------------------------------------
    -- 0) Garantir colunas name/note no STOCK (somente se faltarem)
    ------------------------------------------------------------
    IF COL_LENGTH('dbo.stock', 'name') IS NULL
    BEGIN
        ALTER TABLE dbo.stock 
        ADD [name] VARCHAR(80) NOT NULL 
            CONSTRAINT DF_stock_name DEFAULT ('Estoque');
    END

    IF COL_LENGTH('dbo.stock', 'note') IS NULL
    BEGIN
        ALTER TABLE dbo.stock 
        ADD [note] NVARCHAR(200) NULL;
    END

    ------------------------------------------------------------
    -- 1) Atualizar os estoques existentes para "Principal"
    --    Regra: 1 principal por filial (o já existente vira principal)
    ------------------------------------------------------------
    ;WITH s AS (
        SELECT s.id, s.fk_branch, b.corporate_name
        FROM dbo.stock s
        JOIN dbo.branch b ON b.id = s.fk_branch
    )
    UPDATE st
       SET st.[name] = CONCAT('Estoque Principal - ', s.corporate_name),
           st.[note] = N'Principal'
    FROM dbo.stock st
    JOIN s ON s.id = st.id
    WHERE (st.[note] IS NULL OR st.[note] <> N'Principal')
      AND NOT EXISTS (
            -- evita sobrescrever um principal já nomeado corretamente
            SELECT 1 
            FROM dbo.stock x 
            WHERE x.fk_branch = st.fk_branch 
              AND x.[note] = N'Principal' 
              AND x.id <> st.id
      );

    ------------------------------------------------------------
    -- 2) Inserir um estoque "Secundário" adicional por filial
    --    Usa o mesmo endereço da filial
    ------------------------------------------------------------
    INSERT INTO dbo.stock (fk_address, fk_branch, [name], [note], created_at)
    SELECT b.fk_address, b.id,
           CONCAT('Estoque Secundário - ', b.corporate_name) AS [name],
           N'Secundário' AS [note],
           SYSUTCDATETIME()
    FROM dbo.branch b
    WHERE NOT EXISTS (
        SELECT 1 
        FROM dbo.stock s 
        WHERE s.fk_branch = b.id 
          AND s.[note] = N'Secundário'
    );

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;