SET NOCOUNT ON;

DECLARE @NewOrders TABLE (service_order_id INT);

INSERT INTO service_order (
    number, open_date, close_date, status, total_value, notes,
    fk_vehicle, fk_branch
)
OUTPUT inserted.id INTO @NewOrders(service_order_id)
SELECT TOP (500)
    CONCAT(
        'OS-',
        FORMAT(GETDATE(), 'yyyyMMdd'),
        '-',
        RIGHT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 10)
    ) AS number,
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 220, GETDATE()) AS open_date,
    NULL AS close_date,
    CASE (ABS(CHECKSUM(NEWID())) % 100)
        WHEN 0 THEN 'CANCELED'
        WHEN 1 THEN 'CANCELED'
        WHEN 2 THEN 'CANCELED'
        WHEN 3 THEN 'CANCELED'      -- ~4%
        WHEN 4 THEN 'CLOSED'
        WHEN 5 THEN 'CLOSED'
        WHEN 6 THEN 'CLOSED'
        WHEN 7 THEN 'CLOSED'
        WHEN 8 THEN 'CLOSED'
        WHEN 9 THEN 'CLOSED'
        WHEN 10 THEN 'CLOSED'
        WHEN 11 THEN 'CLOSED'
        WHEN 12 THEN 'CLOSED'
        WHEN 13 THEN 'CLOSED'
        WHEN 14 THEN 'CLOSED'
        WHEN 15 THEN 'CLOSED'
        WHEN 16 THEN 'CLOSED'
        WHEN 17 THEN 'CLOSED'
        WHEN 18 THEN 'CLOSED'
        WHEN 19 THEN 'CLOSED'       -- ~16% CLOSED (total ~20% com canceled)
        WHEN 20 THEN 'OPEN'
        WHEN 21 THEN 'OPEN'
        WHEN 22 THEN 'OPEN'
        WHEN 23 THEN 'OPEN'
        WHEN 24 THEN 'OPEN'
        WHEN 25 THEN 'OPEN'
        WHEN 26 THEN 'OPEN'
        WHEN 27 THEN 'OPEN'
        WHEN 28 THEN 'OPEN'
        WHEN 29 THEN 'OPEN'         -- ~10% OPEN
        ELSE 'IN_PROGRESS'
    END AS status,
    CAST(0 AS DECIMAL(18,2)) AS total_value,
    CASE ABS(CHECKSUM(NEWID())) % 8
        WHEN 0 THEN 'Cliente relatou ruído ao frear. Verificar freios.'
        WHEN 1 THEN 'Revisão preventiva solicitada.'
        WHEN 2 THEN 'Troca de óleo e filtros. Checar vazamentos.'
        WHEN 3 THEN 'Alinhamento/balanceamento e inspeção de pneus.'
        WHEN 4 THEN 'Luz de injeção acesa. Diagnóstico eletrônico.'
        WHEN 5 THEN 'Ar-condicionado fraco. Verificar gás/compressor.'
        WHEN 6 THEN 'Bateria descarregando. Testar alternador.'
        ELSE 'Cliente pediu orçamento antes de extras.'
    END AS notes,
    (SELECT TOP 1 v.id FROM vehicle v ORDER BY NEWID()) AS fk_vehicle,
    (SELECT TOP 1 b.id FROM branch b ORDER BY NEWID()) AS fk_branch
FROM sys.all_objects
ORDER BY NEWID();

------------------------------------------------------------
-- 2) close_date coerente
------------------------------------------------------------
UPDATE so
SET close_date =
    CASE
        WHEN so.status IN ('CLOSED','CANCELED')
            THEN DATEADD(DAY, 1 + (ABS(CHECKSUM(NEWID())) % 25), so.open_date)
        ELSE NULL
    END
FROM service_order so
WHERE so.id IN (SELECT service_order_id FROM @NewOrders);

------------------------------------------------------------
-- 3) CRIAR ITENS (1..6 por OS)
------------------------------------------------------------
;WITH nums AS (
    SELECT 1 AS n UNION ALL SELECT 2 UNION ALL SELECT 3
    UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6
),
orders_with_itemcount AS (
    SELECT
        o.service_order_id,
        1 + (ABS(CHECKSUM(NEWID())) % 6) AS max_items  -- 1..6
    FROM @NewOrders o
),
items AS (
    SELECT ow.service_order_id, n.n AS item_n
    FROM orders_with_itemcount ow
    JOIN nums n ON n.n <= ow.max_items
)
INSERT INTO service_order_item (
    description, quantity, price, subtotal, fk_service_order
)
SELECT
    CASE ABS(CHECKSUM(NEWID())) % 16
        WHEN 0 THEN 'Troca de óleo do motor'
        WHEN 1 THEN 'Substituição de filtro de óleo'
        WHEN 2 THEN 'Substituição de filtro de ar'
        WHEN 3 THEN 'Troca de pastilhas de freio'
        WHEN 4 THEN 'Troca de discos de freio'
        WHEN 5 THEN 'Limpeza de bicos injetores'
        WHEN 6 THEN 'Alinhamento e balanceamento'
        WHEN 7 THEN 'Troca de velas'
        WHEN 8 THEN 'Troca de correia'
        WHEN 9 THEN 'Sistema de arrefecimento (check)'
        WHEN 10 THEN 'Diagnóstico eletrônico (scanner)'
        WHEN 11 THEN 'Revisão completa (checklist)'
        WHEN 12 THEN 'Troca de fluido de freio'
        WHEN 13 THEN 'Serviço de ar-condicionado'
        WHEN 14 THEN 'Troca de amortecedores'
        ELSE 'Mão de obra geral'
    END AS description,
    1 + (ABS(CHECKSUM(NEWID())) % 3) AS quantity, -- 1..3
    CAST(ROUND(60 + (ABS(CHECKSUM(NEWID())) % 1201), 2) AS DECIMAL(18,2)) AS price, -- 60..1260
    CAST(0 AS DECIMAL(18,2)) AS subtotal,
    i.service_order_id
FROM items i;

-- subtotal = quantity * price
UPDATE soi
SET soi.subtotal = CAST(ROUND(soi.quantity * soi.price, 2) AS DECIMAL(18,2))
FROM service_order_item soi
WHERE soi.fk_service_order IN (SELECT service_order_id FROM @NewOrders);

------------------------------------------------------------
-- 4) USE_PART: 0..3 peças por item, MAS SÓ DO ESTOQUE DA FILIAL DA OS
------------------------------------------------------------
;WITH nums3 AS (
    SELECT 1 AS n UNION ALL SELECT 2 UNION ALL SELECT 3
),
target_items AS (
    SELECT
        soi.id AS service_order_item_id,
        so.fk_branch,
        CASE
            WHEN r < 35 THEN 0
            WHEN r < 80 THEN 1
            WHEN r < 95 THEN 2
            ELSE 3
        END AS max_parts
    FROM service_order_item soi
    JOIN service_order so ON so.id = soi.fk_service_order
    CROSS APPLY (SELECT ABS(CHECKSUM(NEWID())) % 100 AS r) x
    WHERE soi.fk_service_order IN (SELECT service_order_id FROM @NewOrders)
),
item_slots AS (
    SELECT
        ti.service_order_item_id,
        ti.fk_branch,
        n.n AS slot_n
    FROM target_items ti
    JOIN nums3 n ON n.n <= ti.max_parts
),
ranked_parts AS (
    SELECT
        islot.service_order_item_id,
        p.id AS fk_part,
        ROW_NUMBER() OVER (
            PARTITION BY islot.service_order_item_id
            ORDER BY NEWID()
        ) AS rn
    FROM item_slots islot
    JOIN stock st ON st.fk_branch = islot.fk_branch
    JOIN part p ON p.fk_stock = st.id
)
INSERT INTO use_part (fk_part, fk_service_order_item, quantity)
SELECT
    rp.fk_part,
    rp.service_order_item_id,
    1 + (ABS(CHECKSUM(NEWID())) % 3) AS quantity
FROM ranked_parts rp
JOIN target_items ti
  ON ti.service_order_item_id = rp.service_order_item_id
WHERE rp.rn <= ti.max_parts;


------------------------------------------------------------
-- 5) total_value da OS = soma dos subtotais dos itens
------------------------------------------------------------
UPDATE so
SET so.total_value = x.total_value
FROM service_order so
CROSS APPLY (
    SELECT CAST(SUM(soi.subtotal) AS DECIMAL(18,2)) AS total_value
    FROM service_order_item soi
    WHERE soi.fk_service_order = so.id
) x
WHERE so.id IN (SELECT service_order_id FROM @NewOrders);

PRINT 'OK: 500 service_order + itens + use_part (peças do estoque da filial) inseridos.';

commit