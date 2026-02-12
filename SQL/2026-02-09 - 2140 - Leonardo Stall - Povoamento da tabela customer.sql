SET XACT_ABORT ON;

BEGIN TRAN;

------------------------------------------------------------
-- 1) Endereços (pais das FKs) – SEM máscara
------------------------------------------------------------
DECLARE @a1 INT, @a2 INT, @a3 INT, @a4 INT, @a5 INT, @a6 INT;

IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Afonso Brás' AND number='550' AND city=N'São Paulo' AND state='SP' AND zip='04511011')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Afonso Brás','550',NULL,N'Vila Nova Conceição',N'São Paulo','SP','04511011',SYSUTCDATETIME());
SELECT @a1 = id FROM dbo.address WHERE street=N'Rua Afonso Brás' AND number='550' AND city=N'São Paulo' AND state='SP' AND zip='04511011';

IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua São Clemente' AND number='250' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22260000')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua São Clemente','250',N'Sala 501',N'Botafogo',N'Rio de Janeiro','RJ','22260000',SYSUTCDATETIME());
SELECT @a2 = id FROM dbo.address WHERE street=N'Rua São Clemente' AND number='250' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22260000';

IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Pernambuco' AND number='850' AND city=N'Belo Horizonte' AND state='MG' AND zip='30130011')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Pernambuco','850',NULL,N'Funcionários',N'Belo Horizonte','MG','30130011',SYSUTCDATETIME());
SELECT @a3 = id FROM dbo.address WHERE street=N'Rua Pernambuco' AND number='850' AND city=N'Belo Horizonte' AND state='MG' AND zip='30130011';

IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Treze de Maio' AND number='520' AND city=N'São Paulo' AND state='SP' AND zip='01327000')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Treze de Maio','520',NULL,N'Bela Vista',N'São Paulo','SP','01327000',SYSUTCDATETIME());
SELECT @a4 = id FROM dbo.address WHERE street=N'Rua Treze de Maio' AND number='520' AND city=N'São Paulo' AND state='SP' AND zip='01327000';

IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Real Grandeza' AND number='400' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22281020')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Real Grandeza','400',NULL,N'Botafogo',N'Rio de Janeiro','RJ','22281020',SYSUTCDATETIME());
SELECT @a5 = id FROM dbo.address WHERE street=N'Rua Real Grandeza' AND number='400' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22281020';

IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Inconfidentes' AND number='70' AND city=N'Belo Horizonte' AND state='MG' AND zip='30140060')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Inconfidentes','70',NULL,N'Funcionários',N'Belo Horizonte','MG','30140060',SYSUTCDATETIME());
SELECT @a6 = id FROM dbo.address WHERE street=N'Rua Inconfidentes' AND number='70' AND city=N'Belo Horizonte' AND state='MG' AND zip='30140060';

------------------------------------------------------------
-- 2) Customers (3 PF + 3 PJ) – SEM máscara (só dígitos)
--    customer_type: 'PF' ou 'PJ'
------------------------------------------------------------

-- PF #1
IF NOT EXISTS (SELECT 1 FROM dbo.customer WHERE cpf_cnpj = '00000000191')
INSERT INTO dbo.customer (cpf_cnpj, customer_type, name, phone, email, active, fk_address, created_at)
VALUES ('00000000191', 'PF', N'Ana Beatriz Souza', '5511984123045', 'ana.souza@example.com', 1, @a1, SYSUTCDATETIME());

-- PF #2
IF NOT EXISTS (SELECT 1 FROM dbo.customer WHERE cpf_cnpj = '12345678909')
INSERT INTO dbo.customer (cpf_cnpj, customer_type, name, phone, email, active, fk_address, created_at)
VALUES ('12345678909', 'PF', N'Carlos Eduardo Lima', '5521976542210', 'carlos.lima@example.com', 1, @a2, SYSUTCDATETIME());

-- PF #3
IF NOT EXISTS (SELECT 1 FROM dbo.customer WHERE cpf_cnpj = '11144477735')
INSERT INTO dbo.customer (cpf_cnpj, customer_type, name, phone, email, active, fk_address, created_at)
VALUES ('11144477735', 'PF', N'Fernanda Alves', '5531988774402', 'fernanda.alves@example.com', 1, @a3, SYSUTCDATETIME());

-- PJ #1
IF NOT EXISTS (SELECT 1 FROM dbo.customer WHERE cpf_cnpj = '19131243000197')
INSERT INTO dbo.customer (cpf_cnpj, customer_type, name, phone, email, active, fk_address, created_at)
VALUES ('19131243000197', 'PJ', N'Oficina Botafogo Ltda', '552140001234', 'contato@oficinabotafogo.com.br', 1, @a5, SYSUTCDATETIME());

-- PJ #2
IF NOT EXISTS (SELECT 1 FROM dbo.customer WHERE cpf_cnpj = '11444777000161')
INSERT INTO dbo.customer (cpf_cnpj, customer_type, name, phone, email, active, fk_address, created_at)
VALUES ('11444777000161', 'PJ', N'Savassi Auto Peças ME', '553199998877', 'vendas@savassiautopecas.com.br', 1, @a6, SYSUTCDATETIME());

-- PJ #3
IF NOT EXISTS (SELECT 1 FROM dbo.customer WHERE cpf_cnpj = '06990590000123')
INSERT INTO dbo.customer (cpf_cnpj, customer_type, name, phone, email, active, fk_address, created_at)
VALUES ('06990590000123', 'PJ', N'Paulista Motors EIRELI', '551130001999', 'contato@paulistamotors.com.br', 1, @a4, SYSUTCDATETIME());

COMMIT;