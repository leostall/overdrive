use overdrive;

SET XACT_ABORT ON;
SET DATEFORMAT ymd;
BEGIN TRY
BEGIN TRAN;

------------------------------------------------------------
-- 0) Obter até 6 clientes existentes (para não dar veículo a todos)
------------------------------------------------------------
DECLARE @Customers TABLE (rid INT IDENTITY(1,1), id INT);
INSERT INTO @Customers(id)
SELECT TOP (6) id FROM dbo.customer WITH (READPAST) ORDER BY id;

IF NOT EXISTS (SELECT 1 FROM @Customers)
BEGIN
    RAISERROR('Nao ha clientes na base. Crie alguns customers antes de inserir vehicles.',16,1);
END

-- Mapear até 6 IDs (alguns podem ficar NULL se houver <6 clientes)
DECLARE @c1 INT = (SELECT id FROM @Customers WHERE rid=1);
DECLARE @c2 INT = (SELECT id FROM @Customers WHERE rid=2);
DECLARE @c3 INT = (SELECT id FROM @Customers WHERE rid=3);
DECLARE @c4 INT = (SELECT id FROM @Customers WHERE rid=4);
DECLARE @c5 INT = (SELECT id FROM @Customers WHERE rid=5);
DECLARE @c6 INT = (SELECT id FROM @Customers WHERE rid=6);

------------------------------------------------------------
-- 1) Pegar IDs dos tipos de veículo
------------------------------------------------------------
DECLARE @vt_carro INT, @vt_moto INT, @vt_suv INT, @vt_pickup INT, @vt_caminhao INT, @vt_van INT;

SELECT @vt_carro    = id FROM dbo.vehicle_type WHERE name = 'Carro';
SELECT @vt_moto     = id FROM dbo.vehicle_type WHERE name = 'Moto';
SELECT @vt_suv      = id FROM dbo.vehicle_type WHERE name = 'SUV';
SELECT @vt_pickup   = id FROM dbo.vehicle_type WHERE name = 'Pickup';
SELECT @vt_caminhao = id FROM dbo.vehicle_type WHERE name = 'Caminhao';
SELECT @vt_van      = id FROM dbo.vehicle_type WHERE name = 'Van';

IF @vt_carro IS NULL OR @vt_moto IS NULL
BEGIN
    RAISERROR('Tipos de veiculo ausentes. Execute o script de vehicle_type antes.',16,1);
END

------------------------------------------------------------
-- 2) Enderecos (FK) para os veiculos – sem mascara (zip = 8 digitos)
--    Para cada veículo crio um endereço próprio (rua/numero/bairro/cidade/UF).
------------------------------------------------------------
DECLARE @addr1 INT, @addr2 INT, @addr3 INT, @addr4 INT, @addr5 INT, @addr6 INT,
        @addr7 INT, @addr8 INT, @addr9 INT, @addr10 INT, @addr11 INT, @addr12 INT;

-- helper de upsert de endereço (padrão por igualdade dos 5 campos-chave)
-- Veículo 1
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Vergueiro' AND number='2100' AND city=N'São Paulo' AND state='SP' AND zip='04101000')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Vergueiro','2100',NULL,N'Paraíso',N'São Paulo','SP','04101000',SYSUTCDATETIME());
SELECT @addr1 = id FROM dbo.address WHERE street=N'Rua Vergueiro' AND number='2100' AND city=N'São Paulo' AND state='SP' AND zip='04101000';

-- Veículo 2
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Avenida Brasil' AND number='450' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22290040')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Avenida Brasil','450',N'Box 3',N'Benfica',N'Rio de Janeiro','RJ','22290040',SYSUTCDATETIME());
SELECT @addr2 = id FROM dbo.address WHERE street=N'Avenida Brasil' AND number='450' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22290040';

-- Veículo 3
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Pernambuco' AND number='850' AND city=N'Belo Horizonte' AND state='MG' AND zip='30130011')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Pernambuco','850',NULL,N'Funcionários',N'Belo Horizonte','MG','30130011',SYSUTCDATETIME());
SELECT @addr3 = id FROM dbo.address WHERE street=N'Rua Pernambuco' AND number='850' AND city=N'Belo Horizonte' AND state='MG' AND zip='30130011';

-- Veículo 4
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Oscar Freire' AND number='1010' AND city=N'São Paulo' AND state='SP' AND zip='01426000')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Oscar Freire','1010',NULL,N'Cerqueira César',N'São Paulo','SP','01426000',SYSUTCDATETIME());
SELECT @addr4 = id FROM dbo.address WHERE street=N'Rua Oscar Freire' AND number='1010' AND city=N'São Paulo' AND state='SP' AND zip='01426000';

-- Veículo 5
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua São Clemente' AND number='250' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22260000')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua São Clemente','250',N'Garagem B',N'Botafogo',N'Rio de Janeiro','RJ','22260000',SYSUTCDATETIME());
SELECT @addr5 = id FROM dbo.address WHERE street=N'Rua São Clemente' AND number='250' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22260000';

-- Veículo 6
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Alagoas' AND number='430' AND city=N'Belo Horizonte' AND state='MG' AND zip='30130160')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Alagoas','430',NULL,N'Funcionários',N'Belo Horizonte','MG','30130160',SYSUTCDATETIME());
SELECT @addr6 = id FROM dbo.address WHERE street=N'Rua Alagoas' AND number='430' AND city=N'Belo Horizonte' AND state='MG' AND zip='30130160';

-- Veículo 7
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Augusta' AND number='1550' AND city=N'São Paulo' AND state='SP' AND zip='01304001')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Augusta','1550',N'Box 2',N'Consolação',N'São Paulo','SP','01304001',SYSUTCDATETIME());
SELECT @addr7 = id FROM dbo.address WHERE street=N'Rua Augusta' AND number='1550' AND city=N'São Paulo' AND state='SP' AND zip='01304001';

-- Veículo 8
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Praia de Botafogo' AND number='340' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22250040')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Praia de Botafogo','340',NULL,N'Botafogo',N'Rio de Janeiro','RJ','22250040',SYSUTCDATETIME());
SELECT @addr8 = id FROM dbo.address WHERE street=N'Praia de Botafogo' AND number='340' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22250040';

-- Veículo 9
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua da Bahia' AND number='2700' AND city=N'Belo Horizonte' AND state='MG' AND zip='30160010')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua da Bahia','2700',NULL,N'Lourdes',N'Belo Horizonte','MG','30160010',SYSUTCDATETIME());
SELECT @addr9 = id FROM dbo.address WHERE street=N'Rua da Bahia' AND number='2700' AND city=N'Belo Horizonte' AND state='MG' AND zip='30160010';

-- Veículo 10
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Alameda Santos' AND number='2300' AND city=N'São Paulo' AND state='SP' AND zip='01418002')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Alameda Santos','2300',N'Conj 808',N'Jardim Paulista',N'São Paulo','SP','01418002',SYSUTCDATETIME());
SELECT @addr10 = id FROM dbo.address WHERE street=N'Alameda Santos' AND number='2300' AND city=N'São Paulo' AND state='SP' AND zip='01418002';

-- Veículo 11
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Inconfidentes' AND number='70' AND city=N'Belo Horizonte' AND state='MG' AND zip='30140060')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Inconfidentes','70',NULL,N'Funcionários',N'Belo Horizonte','MG','30140060',SYSUTCDATETIME());
SELECT @addr11 = id FROM dbo.address WHERE street=N'Rua Inconfidentes' AND number='70' AND city=N'Belo Horizonte' AND state='MG' AND zip='30140060';

-- Veículo 12
IF NOT EXISTS (SELECT 1 FROM dbo.address WHERE street=N'Rua Voluntários da Pátria' AND number='300' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22270010')
    INSERT INTO dbo.address(street,number,complement,neighborhood,city,state,zip,created_at)
    VALUES (N'Rua Voluntários da Pátria','300',NULL,N'Botafogo',N'Rio de Janeiro','RJ','22270010',SYSUTCDATETIME());
SELECT @addr12 = id FROM dbo.address WHERE street=N'Rua Voluntários da Pátria' AND number='300' AND city=N'Rio de Janeiro' AND state='RJ' AND zip='22270010';

------------------------------------------------------------
-- 3) Veículos (12 no total) – distribuídos entre poucos clientes
--    Campos: chassi(17), plate(7), brand, model, year, mileage, condition, status, value
------------------------------------------------------------

-- Cliente 1 (se existir): 3 veículos
IF @c1 IS NOT NULL
BEGIN
    INSERT INTO dbo.vehicle (chassi, plate, brand, model, [year], mileage, [condition], [status], [value], fk_address, fk_customer, fk_vehicle_type, created_at) VALUES
    ('9BWZZZ377VT004251', 'BRA1A23', 'Volkswagen', 'Gol 1.6',     2018,  64000, 'Usado',   'Disponivel',  38990.00, @addr1,  @c1, @vt_carro,  SYSUTCDATETIME()),
    ('9BG116GW04C400001', 'RJO2B45', 'Chevrolet',  'Onix 1.0',    2020,  32000, 'Usado',   'Em Revisao',  54990.00, @addr4,  @c1, @vt_carro,  SYSUTCDATETIME()),
    ('3C6UR5DL0KG501234', 'SPX3C67', 'Fiat',       'Toro Freedom',2019,  41000, 'Usado',   'Disponivel', 109900.00, @addr10, @c1, @vt_pickup, SYSUTCDATETIME());
END

-- Cliente 2: 2 veículos
IF @c2 IS NOT NULL
BEGIN
    INSERT INTO dbo.vehicle (chassi, plate, brand, model, [year], mileage, [condition], [status], [value], fk_address, fk_customer, fk_vehicle_type, created_at) VALUES
    ('93HMR1H21MJ012345', 'RJQ4D21', 'Honda',      'CG 160 Fan',  2021,  11000, 'Usado',   'Disponivel',  13990.00, @addr2,  @c2, @vt_moto,  SYSUTCDATETIME()),
    ('9BD17106X52456789', 'RJZ5E19', 'Fiat',       'Argo 1.3',    2019,  35000, 'Usado',   'Vendido',     52900.00, @addr5,  @c2, @vt_carro,  SYSUTCDATETIME());
END

-- Cliente 3: 3 veículos
IF @c3 IS NOT NULL
BEGIN
    INSERT INTO dbo.vehicle (chassi, plate, brand, model, [year], mileage, [condition], [status], [value], fk_address, fk_customer, fk_vehicle_type, created_at) VALUES
    ('9BS2E55A0E0126789', 'MGW6F43', 'Chevrolet',  'S10 LTZ 2.8', 2017,  98000, 'Usado',   'Disponivel', 129900.00, @addr3,  @c3, @vt_pickup, SYSUTCDATETIME()),
    ('PPVZZZ12ZJM765432', 'MGY7G12', 'Hyundai',    'Creta Pulse', 2022,  18000, 'Seminovo','Disponivel', 119900.00, @addr6,  @c3, @vt_suv,    SYSUTCDATETIME()),
    ('9BDXU19F08B123456', 'MGA8H55', 'Fiat',       'Ducato 2.3',  2016, 142000, 'Usado',   'Disponivel',  89900.00, @addr11, @c3, @vt_van,    SYSUTCDATETIME());
END

-- Cliente 4: 2 veículos
IF @c4 IS NOT NULL
BEGIN
    INSERT INTO dbo.vehicle (chassi, plate, brand, model, [year], mileage, [condition], [status], [value], fk_address, fk_customer, fk_vehicle_type, created_at) VALUES
    ('9FYZZZ8P0LA012345', 'SPA9J31', 'Toyota',     'Corolla 2.0', 2021,  22000, 'Seminovo','Disponivel', 124900.00, @addr7,  @c4, @vt_carro,  SYSUTCDATETIME()),
    ('93YBHBK15HB123456', 'SPB0K27', 'Yamaha',     'Fazer 250',   2019,  24000, 'Usado',   'Disponivel',  16990.00, @addr1,  @c4, @vt_moto,   SYSUTCDATETIME());
END

-- Cliente 5: 1 veículo
IF @c5 IS NOT NULL
BEGIN
    INSERT INTO dbo.vehicle (chassi, plate, brand, model, [year], mileage, [condition], [status], [value], fk_address, fk_customer, fk_vehicle_type, created_at) VALUES
    ('9BM384067AB123456', 'RJQ1L09', 'Mercedes',   'Sprinter 415',2018, 126000, 'Usado',   'Disponivel', 159900.00, @addr8,  @c5, @vt_van,    SYSUTCDATETIME());
END

-- Cliente 6: 1 veículo
IF @c6 IS NOT NULL
BEGIN
    INSERT INTO dbo.vehicle (chassi, plate, brand, model, [year], mileage, [condition], [status], [value], fk_address, fk_customer, fk_vehicle_type, created_at) VALUES
    ('9BRXK40G0JC123456', 'MGC2M35', 'Volkswagen', 'Constellation 24.280',2015, 320000,'Usado','Disponivel', 239900.00, @addr9,  @c6, @vt_caminhao, SYSUTCDATETIME());
END

COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;