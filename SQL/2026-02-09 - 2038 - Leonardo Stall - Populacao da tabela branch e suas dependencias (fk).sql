SET XACT_ABORT ON;
BEGIN TRAN;

------------------------------------------------------------
-- FILIAL 1 — "Filial Paulista" (São Paulo/SP)
------------------------------------------------------------
DECLARE @addr1_id INT;

-- 1) ENDEREÇO (FK)
IF NOT EXISTS (
    SELECT 1 FROM address 
    WHERE street = N'Av. Paulista' AND number = N'1578'
      AND city = N'São Paulo' AND state = N'SP' AND zip = '01310200'
)
BEGIN
    INSERT INTO address (street, number, complement, neighborhood, city, state, zip, created_at)
    VALUES (N'Av. Paulista', N'1578', N'Conj. 101', N'Bela Vista', N'São Paulo', N'SP', '01310200', SYSUTCDATETIME());
END

SELECT @addr1_id = id FROM address
WHERE street = N'Av. Paulista' AND number = N'1578'
  AND city = N'São Paulo' AND state = N'SP' AND zip = '01310200';

-- 2) FILIAL
IF NOT EXISTS (
    SELECT 1 FROM branch WHERE corporate_name = N'Filial Paulista' AND fk_address = @addr1_id
)
BEGIN
    INSERT INTO branch (corporate_name, cnpj, phone, email, active, fk_address, created_at)
    VALUES (N'Filial Paulista', '52791456000110', '5511948123045', 'paulista@empresa.com.br', 1, @addr1_id, SYSUTCDATETIME());
END

------------------------------------------------------------
-- FILIAL 2 — "Filial Botafogo" (Rio de Janeiro/RJ)
------------------------------------------------------------
DECLARE @addr2_id INT;

-- 1) ENDEREÇO (FK)
IF NOT EXISTS (
    SELECT 1 FROM address 
    WHERE street = N'Rua Voluntários da Pátria' AND number = N'45'
      AND city = N'Rio de Janeiro' AND state = N'RJ' AND zip = '22270000'
)
BEGIN
    INSERT INTO address (street, number, complement, neighborhood, city, state, zip, created_at)
    VALUES (N'Rua Voluntários da Pátria', N'45', N'Sala 804', N'Botafogo', N'Rio de Janeiro', N'RJ', '22270000', SYSUTCDATETIME());
END

SELECT @addr2_id = id FROM address
WHERE street = N'Rua Voluntários da Pátria' AND number = N'45'
  AND city = N'Rio de Janeiro' AND state = N'RJ' AND zip = '22270000';

-- 2) FILIAL
IF NOT EXISTS (
    SELECT 1 FROM branch WHERE corporate_name = N'Filial Botafogo' AND fk_address = @addr2_id
)
BEGIN
    INSERT INTO branch (corporate_name, cnpj, phone, email, active, fk_address, created_at)
    VALUES (N'Filial Botafogo', '45369143000103', '5521976542210', 'botafogo@empresa.com.br', 1, @addr2_id, SYSUTCDATETIME());
END

------------------------------------------------------------
-- FILIAL 3 — "Filial Savassi" (Belo Horizonte/MG)
------------------------------------------------------------
DECLARE @addr3_id INT;

-- 1) ENDEREÇO (FK)
IF NOT EXISTS (
    SELECT 1 FROM address 
    WHERE street = N'Av. Contorno' AND number = N'3000'
      AND city = N'Belo Horizonte' AND state = N'MG' AND zip = '30110915'
)
BEGIN
    INSERT INTO address (street, number, complement, neighborhood, city, state, zip, created_at)
    VALUES (N'Av. Contorno', N'3000', N'Loja 02', N'Savassi', N'Belo Horizonte', N'MG', '30110915', SYSUTCDATETIME());
END

SELECT @addr3_id = id FROM address
WHERE street = N'Av. Contorno' AND number = N'3000'
  AND city = N'Belo Horizonte' AND state = N'MG' AND zip = '30110915';

-- 2) FILIAL
IF NOT EXISTS (
    SELECT 1 FROM branch WHERE corporate_name = N'Filial Savassi' AND fk_address = @addr3_id
)
BEGIN
    INSERT INTO branch (corporate_name, cnpj, phone, email, active, fk_address, created_at)
    VALUES (N'Filial Savassi', '54855717000144', '5531988774402', 'savassi@empresa.com.br', 1, @addr3_id, SYSUTCDATETIME());
END

-- COMMIT;