SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRAN;

    IF OBJECT_ID('dbo.vehicle_type', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.vehicle_type (
            id INT IDENTITY(1,1) PRIMARY KEY,
            name VARCHAR(30) NOT NULL,
            wheel_count TINYINT NULL,
            active BIT NOT NULL CONSTRAINT DF_vehicle_type_active DEFAULT (1),
            created_at DATETIME2 NOT NULL CONSTRAINT DF_vehicle_type_created_at DEFAULT SYSUTCDATETIME(),
            edited_at DATETIME2 NULL
        );
        CREATE UNIQUE INDEX UX_vehicle_type_name ON dbo.vehicle_type(name);
    END;

    ;WITH seed(name, wheel_count) AS (
        SELECT 'Carro',4 UNION ALL
        SELECT 'Moto',2 UNION ALL
        SELECT 'Caminhao',6 UNION ALL
        SELECT 'Onibus',6 UNION ALL
        SELECT 'SUV',4 UNION ALL
        SELECT 'Pickup',4 UNION ALL
        SELECT 'Van',4
    )
    INSERT INTO dbo.vehicle_type(name, wheel_count)
    SELECT s.name, s.wheel_count
    FROM seed s
    WHERE NOT EXISTS (SELECT 1 FROM dbo.vehicle_type t WHERE t.name = s.name);

    IF COL_LENGTH('dbo.vehicle', 'fk_vehicle_type') IS NULL
    BEGIN
        ALTER TABLE dbo.vehicle ADD fk_vehicle_type INT NULL;
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_vehicle_vehicle_type')
    BEGIN
        ALTER TABLE dbo.vehicle WITH CHECK
        ADD CONSTRAINT FK_vehicle_vehicle_type
            FOREIGN KEY (fk_vehicle_type) REFERENCES dbo.vehicle_type(id);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes 
        WHERE name = 'IX_vehicle_fk_vehicle_type' 
          AND object_id = OBJECT_ID('dbo.vehicle')
    )
    BEGIN
        CREATE INDEX IX_vehicle_fk_vehicle_type ON dbo.vehicle(fk_vehicle_type);
    END;


    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_vehicle_year_range')
    BEGIN
        DECLARE @y INT = YEAR(GETDATE()) + 1;
        DECLARE @sql NVARCHAR(MAX) =
            N'ALTER TABLE dbo.vehicle ' +
            N'ADD CONSTRAINT CK_vehicle_year_range ' +
            N'CHECK ([year] BETWEEN 1950 AND ' + CONVERT(NVARCHAR(10), @y) + N')';
        EXEC sys.sp_executesql @sql;
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_vehicle_mileage_nonneg')
    BEGIN
        ALTER TABLE dbo.vehicle ADD CONSTRAINT CK_vehicle_mileage_nonneg CHECK ([mileage] >= 0);
    END;
    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_vehicle_value_nonneg')
    BEGIN
        ALTER TABLE dbo.vehicle ADD CONSTRAINT CK_vehicle_value_nonneg CHECK ([value] >= 0);
    END;

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;
GO