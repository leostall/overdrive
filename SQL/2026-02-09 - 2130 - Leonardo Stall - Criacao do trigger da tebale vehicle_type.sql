USE [overdrive];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Recria se já existir
IF OBJECT_ID('dbo.tr_update_vehicle_type', 'TR') IS NOT NULL
    DROP TRIGGER dbo.tr_update_vehicle_type;
GO

CREATE TRIGGER dbo.tr_update_vehicle_type
ON dbo.vehicle_type
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE vt
       SET edited_at = SYSUTCDATETIME()
    FROM dbo.vehicle_type AS vt
    INNER JOIN inserted AS i ON i.id = vt.id
    INNER JOIN deleted  AS d ON d.id = vt.id
    WHERE
           ISNULL(i.name,        '') <> ISNULL(d.name,        '')
        OR ISNULL(i.wheel_count, -1) <> ISNULL(d.wheel_count, -1)
        OR ISNULL(i.active,       0) <> ISNULL(d.active,       0);
END;
GO