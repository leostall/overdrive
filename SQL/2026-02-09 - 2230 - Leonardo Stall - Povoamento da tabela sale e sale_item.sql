
SET NOCOUNT ON;

INSERT INTO sale (
    sale_date, subtotal, discount, additional_fee, total,
    fk_customer, fk_branch, fk_employee, fk_payment, fk_status
)
SELECT TOP (60)
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 120, GETDATE()) AS sale_date,
    CAST(0 AS DECIMAL(18,2)) AS subtotal,
    CAST(0 AS DECIMAL(18,2)) AS discount,
    CAST(0 AS DECIMAL(18,2)) AS additional_fee,
    CAST(0 AS DECIMAL(18,2)) AS total,
    c.id AS fk_customer,
    (SELECT TOP 1 b.id FROM branch b ORDER BY NEWID()) AS fk_branch,
    (SELECT TOP 1 e.id FROM employee e ORDER BY NEWID()) AS fk_employee,
    (SELECT TOP 1 p.id FROM payment p ORDER BY NEWID()) AS fk_payment,
    (SELECT TOP 1 st.id FROM status st ORDER BY NEWID()) AS fk_status
FROM customer c
ORDER BY NEWID();



;WITH sales_ranked AS (
    SELECT
        id AS sale_id,
        ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn
    FROM sale
),
items_to_create AS (
    SELECT sale_id, 1 AS item_n FROM sales_ranked WHERE rn BETWEEN 1 AND 20

    UNION ALL
    SELECT sale_id, 1 FROM sales_ranked WHERE rn BETWEEN 21 AND 40
    UNION ALL
    SELECT sale_id, 2 FROM sales_ranked WHERE rn BETWEEN 21 AND 40

    UNION ALL
    SELECT sale_id, 1 FROM sales_ranked WHERE rn BETWEEN 41 AND 60
    UNION ALL
    SELECT sale_id, 2 FROM sales_ranked WHERE rn BETWEEN 41 AND 60
    UNION ALL
    SELECT sale_id, 3 FROM sales_ranked WHERE rn BETWEEN 41 AND 60
),
typed_items AS (
    SELECT
        sale_id,
        item_n,
        CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 55 THEN 'PART' ELSE 'VEHICLE' END AS item_type
    FROM items_to_create
)
INSERT INTO sale_item (
    item_type, quantity, unit_price, discount,
    fk_part, fk_vehicle, fk_sale
)
SELECT
    ti.item_type,

    CASE
        WHEN ti.item_type = 'PART' THEN 1 + (ABS(CHECKSUM(NEWID())) % 3)  -- 1..3 peças
        ELSE 1
    END AS quantity,

    CASE
        WHEN ti.item_type = 'PART' THEN pr.price
        ELSE vh.value
    END AS unit_price,

    CAST(ROUND((ABS(CHECKSUM(NEWID())) % 1500) / 100.0, 2) AS DECIMAL(18,2)) AS discount,

    CASE WHEN ti.item_type = 'PART' THEN pr.id ELSE NULL END AS fk_part,
    CASE WHEN ti.item_type = 'VEHICLE' THEN vh.id ELSE NULL END AS fk_vehicle,

    ti.sale_id
FROM typed_items ti
OUTER APPLY (
    SELECT TOP 1 id, price
    FROM part
    ORDER BY NEWID()
) pr
OUTER APPLY (
    SELECT TOP 1 id, value
    FROM vehicle
    ORDER BY NEWID()
) vh;

UPDATE si
SET si.discount =
    CASE
        WHEN si.discount > (si.quantity * si.unit_price) THEN CAST(0 AS DECIMAL(18,2))
        ELSE si.discount
    END
FROM sale_item si;

UPDATE s
SET s.subtotal = x.subtotal
FROM sale s
CROSS APPLY (
    SELECT CAST(SUM((si.quantity * si.unit_price) - si.discount) AS DECIMAL(18,2)) AS subtotal
    FROM sale_item si
    WHERE si.fk_sale = s.id
) x;

UPDATE s
SET
    s.discount = CAST(ROUND(s.subtotal * ((ABS(CHECKSUM(NEWID())) % 600) / 10000.0), 2) AS DECIMAL(18,2)),
    s.additional_fee = CAST(ROUND(s.subtotal * ((ABS(CHECKSUM(NEWID())) % 300) / 10000.0), 2) AS DECIMAL(18,2))
FROM sale s;

UPDATE s
SET s.total = CAST(ROUND(s.subtotal - s.discount + s.additional_fee, 2) AS DECIMAL(18,2))
FROM sale s;


UPDATE sale
SET total = 0
WHERE total < 0;

PRINT 'OK: sale e sale_item populadas.';

SET NOCOUNT ON;

------------------------------------------------------------
-- 1) Inserir MAIS vendas e guardar os IDs criados
------------------------------------------------------------
DECLARE @NewSales TABLE (sale_id INT);

INSERT INTO sale (
    sale_date, subtotal, discount, additional_fee, total,
    fk_customer, fk_branch, fk_employee, fk_payment, fk_status
)
OUTPUT inserted.id INTO @NewSales(sale_id)
SELECT TOP (120)
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 180, GETDATE()) AS sale_date,
    CAST(0 AS DECIMAL(18,2)) AS subtotal,
    CAST(0 AS DECIMAL(18,2)) AS discount,
    CAST(0 AS DECIMAL(18,2)) AS additional_fee,
    CAST(0 AS DECIMAL(18,2)) AS total,
    (SELECT TOP 1 c.id  FROM customer c ORDER BY NEWID()) AS fk_customer,
    (SELECT TOP 1 b.id  FROM branch b   ORDER BY NEWID()) AS fk_branch,
    (SELECT TOP 1 e.id  FROM employee e ORDER BY NEWID()) AS fk_employee,
    (SELECT TOP 1 p.id  FROM payment p  ORDER BY NEWID()) AS fk_payment,
    (SELECT TOP 1 st.id FROM status st  ORDER BY NEWID()) AS fk_status
FROM sys.all_objects
ORDER BY NEWID();


------------------------------------------------------------
-- 2) Inserir 1..3 itens por venda (sem loop)
------------------------------------------------------------
;WITH nums AS (
    SELECT 1 AS n UNION ALL SELECT 2 UNION ALL SELECT 3
),
sale_with_max AS (
    SELECT
        ns.sale_id,
        1 + (ABS(CHECKSUM(NEWID())) % 3) AS max_items  -- 1..3
    FROM @NewSales ns
),
items AS (
    SELECT
        s.sale_id,
        n.n AS item_n
    FROM sale_with_max s
    JOIN nums n ON n.n <= s.max_items
),
typed_items AS (
    SELECT
        i.sale_id,
        i.item_n,
        CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 60 THEN 'PART' ELSE 'VEHICLE' END AS item_type
    FROM items i
)
INSERT INTO sale_item (
    item_type, quantity, unit_price, discount,
    fk_part, fk_vehicle, fk_sale
)
SELECT
    ti.item_type,
    CASE WHEN ti.item_type = 'PART' THEN 1 + (ABS(CHECKSUM(NEWID())) % 3) ELSE 1 END AS quantity,
    CASE WHEN ti.item_type = 'PART' THEN pr.price ELSE vh.value END AS unit_price,
    CAST(ROUND((ABS(CHECKSUM(NEWID())) % 1500) / 100.0, 2) AS DECIMAL(18,2)) AS discount,
    CASE WHEN ti.item_type = 'PART' THEN pr.id ELSE NULL END AS fk_part,
    CASE WHEN ti.item_type = 'VEHICLE' THEN vh.id ELSE NULL END AS fk_vehicle,
    ti.sale_id
FROM typed_items ti
OUTER APPLY (SELECT TOP 1 id, price FROM part   ORDER BY NEWID()) pr
OUTER APPLY (SELECT TOP 1 id, value FROM vehicle ORDER BY NEWID()) vh;


------------------------------------------------------------
-- 3) Garantir: desconto do item não pode ser maior que o item
------------------------------------------------------------
UPDATE si
SET si.discount =
    CASE
        WHEN si.discount > (si.quantity * si.unit_price) THEN CAST(0 AS DECIMAL(18,2))
        ELSE si.discount
    END
FROM sale_item si
WHERE si.fk_sale IN (SELECT sale_id FROM @NewSales);


------------------------------------------------------------
-- 4) Calcular subtotal das vendas novas
------------------------------------------------------------
UPDATE s
SET s.subtotal = x.subtotal
FROM sale s
CROSS APPLY (
    SELECT CAST(SUM((si.quantity * si.unit_price) - si.discount) AS DECIMAL(18,2)) AS subtotal
    FROM sale_item si
    WHERE si.fk_sale = s.id
) x
WHERE s.id IN (SELECT sale_id FROM @NewSales);


------------------------------------------------------------
-- 5) Definir desconto e taxa extra no cabeçalho da venda
------------------------------------------------------------
UPDATE s
SET
    s.discount = CAST(ROUND(s.subtotal * ((ABS(CHECKSUM(NEWID())) % 600) / 10000.0), 2) AS DECIMAL(18,2)),        -- 0..6%
    s.additional_fee = CAST(ROUND(s.subtotal * ((ABS(CHECKSUM(NEWID())) % 300) / 10000.0), 2) AS DECIMAL(18,2))   -- 0..3%
FROM sale s
WHERE s.id IN (SELECT sale_id FROM @NewSales);


------------------------------------------------------------
-- 6) Total = subtotal - discount + additional_fee
------------------------------------------------------------
UPDATE s
SET s.total = CAST(ROUND(s.subtotal - s.discount + s.additional_fee, 2) AS DECIMAL(18,2))
FROM sale s
WHERE s.id IN (SELECT sale_id FROM @NewSales);

------------------------------------------------------------
-- 7) Segurança: total nunca negativo
------------------------------------------------------------
UPDATE sale
SET total = 0
WHERE id IN (SELECT sale_id FROM @NewSales)
  AND total < 0;

PRINT 'OK: novas vendas e itens inseridos.';