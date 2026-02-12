use overdrive;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- (Recria o trigger se já existir)
IF OBJECT_ID('dbo.tr_update_department', 'TR') IS NOT NULL
    DROP TRIGGER dbo.tr_update_department;
GO

CREATE TRIGGER dbo.tr_update_department
ON dbo.department
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    /* Atualiza edited_at apenas quando name/active realmente mudarem.
       Obs.: se você tiver mais colunas que quer rastrear, adicione-as no WHERE. */
    UPDATE d
      SET edited_at = SYSUTCDATETIME()
    FROM dbo.department AS d
    INNER JOIN inserted AS i ON i.id = d.id
    INNER JOIN deleted  AS o ON o.id = d.id
    WHERE
        ISNULL(i.name  , '') <> ISNULL(o.name  , '')
        OR ISNULL(i.active, 0) <> ISNULL(o.active, 0);
END;
GO
