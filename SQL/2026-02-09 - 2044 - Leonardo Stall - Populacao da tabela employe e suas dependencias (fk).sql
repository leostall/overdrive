SET XACT_ABORT ON;
SET DATEFORMAT ymd;  -- garante que '1992-05-14' seja interpretado como 14/05/1992
BEGIN TRAN;

------------------------------------------------------------
-- 0) Mapear IDs das filiais criadas anteriormente
------------------------------------------------------------
DECLARE @Branch TABLE (corporate_name NVARCHAR(80), branch_id INT);
INSERT INTO @Branch (corporate_name, branch_id)
SELECT b.corporate_name, b.id
FROM branch b
WHERE b.corporate_name IN (N'Filial Paulista', N'Filial Botafogo', N'Filial Savassi');

------------------------------------------------------------
-- 1) Staging: funcionários + endereços (SEM MÁSCARA) + filial por nome
--    (20 Paulista, 10 Botafogo, 15 Savassi) = 45 linhas
------------------------------------------------------------
DECLARE @EmpStage TABLE (
    registration VARCHAR(30),
    name         NVARCHAR(100),
    position     VARCHAR(30),
    commission_rate DECIMAL(5,4),
    active       BIT,
    birth_date   DATETIME,
    street       VARCHAR(120),
    number       VARCHAR(20),
    complement   VARCHAR(60),
    neighborhood VARCHAR(60),
    city         VARCHAR(60),
    state        CHAR(2),
    zip          CHAR(8),
    corporate_name NVARCHAR(80)
);

-- ========= Filial Paulista (20) =========
INSERT INTO @EmpStage VALUES
('EMP0001', N'Ana Beatriz Souza', 'Vendedor',   0.0300, 1, '1992-05-14', 'Av. Brig. Luís Antônio','2200', 'Ap 11', 'Bela Vista','São Paulo','SP','01318100', N'Filial Paulista'),
('EMP0002', N'Carlos Eduardo Lima','Vendedor',  0.0280, 1, '1990-03-02', 'Rua Augusta','1550', 'Ap 402','Consolação','São Paulo','SP','01304001', N'Filial Paulista'),
('EMP0003', N'Fernanda Alves',     'Gerente',   0.0500, 1, '1986-11-23', 'Rua Haddock Lobo','780', NULL,'Cerqueira César','São Paulo','SP','01414000', N'Filial Paulista'),
('EMP0004', N'Gabriel Santos',     'Mecânico',  0.0150, 1, '1994-07-08', 'Rua Frei Caneca','350', 'Casa 2','Consolação','São Paulo','SP','01307000', N'Filial Paulista'),
('EMP0005', N'Juliana Martins',    'Caixa',     0.0100, 1, '1995-01-19', 'Rua Treze de Maio','520', NULL,'Bela Vista','São Paulo','SP','01327000', N'Filial Paulista'),
('EMP0006', N'Lucas Oliveira',     'Estoque',   0.0000, 1, '1997-12-30', 'Rua Itapeva','420', NULL,'Bela Vista','São Paulo','SP','01332000', N'Filial Paulista'),
('EMP0007', N'Mariana Ferreira',   'Vendedor',  0.0250, 1, '1991-08-10', 'Rua Pamplona','900', 'Ap 33','Jardim Paulista','São Paulo','SP','01405000', N'Filial Paulista'),
('EMP0008', N'Roberto Nunes',      'Mecânico',  0.0120, 1, '1989-10-05', 'Rua Estados Unidos','120', NULL,'Jardim América','São Paulo','SP','01427002', N'Filial Paulista'),
('EMP0009', N'Simone Carvalho',    'Atendente', 0.0080, 1, '1998-04-12', 'Rua Peixoto Gomide','610', NULL,'Jardins','São Paulo','SP','01409002', N'Filial Paulista'),
('EMP0010', N'Pedro Henrique Dias','Vendedor',  0.0220, 1, '1993-09-01', 'Alameda Santos','2300', 'Conj 808','Jardim Paulista','São Paulo','SP','01418002', N'Filial Paulista'),
('EMP0011', N'Bianca Rocha',       'Mecânico',  0.0140, 1, '1996-02-16', 'Rua Batataes','77',  NULL,'Jardim Paulista','São Paulo','SP','01423000', N'Filial Paulista'),
('EMP0012', N'Caio Ribeiro',       'Estoque',   0.0000, 1, '1999-06-28', 'Rua Oscar Freire','1010',NULL,'Cerqueira César','São Paulo','SP','01426000', N'Filial Paulista'),
('EMP0013', N'Daniela Prado',      'Vendedor',  0.0275, 1, '1992-12-22', 'Rua Bela Cintra','455',NULL,'Consolação','São Paulo','SP','01415000', N'Filial Paulista'),
('EMP0014', N'Eduarda Barros',     'Atendente', 0.0070, 1, '1997-03-09', 'Rua Carlos Sampaio','60',NULL,'Bela Vista','São Paulo','SP','01333020', N'Filial Paulista'),
('EMP0015', N'Fábio Pires',        'Mecânico',  0.0130, 1, '1988-05-27', 'Rua Cubatão','900',  NULL,'Paraiso','São Paulo','SP','04013041', N'Filial Paulista'),
('EMP0016', N'Helena Araújo',      'Caixa',     0.0100, 1, '1995-11-11', 'Rua Vergueiro','2100',NULL,'Paraiso','São Paulo','SP','04101000', N'Filial Paulista'),
('EMP0017', N'Igor Moraes',        'Estoque',   0.0000, 1, '1998-07-19', 'Rua Domingos de Morais','3000',NULL,'Vila Mariana','São Paulo','SP','04036040', N'Filial Paulista'),
('EMP0018', N'Karen Batista',      'Vendedor',  0.0240, 1, '1991-01-30', 'Rua Afonso Brás','550',NULL,'Vila Nova Conceição','São Paulo','SP','04511011', N'Filial Paulista'),
('EMP0019', N'Leandro Fagundes',   'Mecânico',  0.0160, 1, '1987-02-04', 'Rua Joaquim Floriano','220',NULL,'Itaim Bibi','São Paulo','SP','04534001', N'Filial Paulista'),
('EMP0020', N'Mônica Rezende',     'Atendente', 0.0080, 1, '1999-10-21', 'Rua Clodomiro Amazonas','700',NULL,'Vila Olímpia','São Paulo','SP','04537001', N'Filial Paulista');

-- ========= Filial Botafogo (10) =========
INSERT INTO @EmpStage VALUES
('EMP0101', N'Arthur Queiroz',     'Vendedor',  0.0260, 1, '1993-04-02', 'Rua São Clemente','250','Sala 501','Botafogo','Rio de Janeiro','RJ','22260000', N'Filial Botafogo'),
('EMP0102', N'Bruna Coutinho',     'Atendente', 0.0070, 1, '1998-01-12', 'Rua Voluntários da Pátria','300',NULL,'Botafogo','Rio de Janeiro','RJ','22270010', N'Filial Botafogo'),
('EMP0103', N'Caue Menezes',       'Mecânico',  0.0120, 1, '1989-08-30', 'Rua Dezenove de Fevereiro','90',NULL,'Botafogo','Rio de Janeiro','RJ','22280003', N'Filial Botafogo'),
('EMP0104', N'Débora Vilela',      'Caixa',     0.0100, 1, '1996-12-23', 'Rua Real Grandeza','400',NULL,'Botafogo','Rio de Janeiro','RJ','22281020', N'Filial Botafogo'),
('EMP0105', N'Erick Barcellos',    'Estoque',   0.0000, 1, '1999-02-17', 'Rua Hans Staden','40',NULL,'Botafogo','Rio de Janeiro','RJ','22251100', N'Filial Botafogo'),
('EMP0106', N'Flávia Monteiro',    'Vendedor',  0.0290, 1, '1991-09-14', 'Praia de Botafogo','340',NULL,'Botafogo','Rio de Janeiro','RJ','22250040', N'Filial Botafogo'),
('EMP0107', N'Gustavo Campos',     'Mecânico',  0.0150, 1, '1988-06-07', 'Rua Sorocaba','210',NULL,'Botafogo','Rio de Janeiro','RJ','22271110', N'Filial Botafogo'),
('EMP0108', N'Heloísa Freitas',    'Atendente', 0.0080, 1, '1997-05-01', 'Rua Assis Bueno','55',NULL,'Botafogo','Rio de Janeiro','RJ','22271140', N'Filial Botafogo'),
('EMP0109', N'Ian Peixoto',        'Estoque',   0.0000, 1, '1998-03-20', 'Rua Capitão César de Andrade','12',NULL,'Botafogo','Rio de Janeiro','RJ','22271052', N'Filial Botafogo'),
('EMP0110', N'Júlia Siqueira',     'Vendedor',  0.0230, 1, '1994-11-09', 'Rua Muniz Barreto','77',NULL,'Botafogo','Rio de Janeiro','RJ','22251050', N'Filial Botafogo');

-- ========= Filial Savassi (15) =========
INSERT INTO @EmpStage VALUES
('EMP0201', N'Adriana Lopes',      'Vendedor',  0.0250, 1, '1992-07-25', 'Rua Pernambuco','850', NULL,'Funcionários','Belo Horizonte','MG','30130011', N'Filial Savassi'),
('EMP0202', N'Bruno Teixeira',     'Mecânico',  0.0140, 1, '1987-01-09', 'Av. Contorno','3050', 'Loja 05','Savassi','Belo Horizonte','MG','30110915', N'Filial Savassi'),
('EMP0203', N'Camila Andrade',     'Atendente', 0.0080, 1, '1999-08-18', 'Rua Sergipe','600', NULL,'Funcionários','Belo Horizonte','MG','30130170', N'Filial Savassi'),
('EMP0204', N'Diego Paiva',        'Estoque',   0.0000, 1, '1998-10-29', 'Rua Alagoas','430', NULL,'Funcionários','Belo Horizonte','MG','30130160', N'Filial Savassi'),
('EMP0205', N'Elisa Moreira',      'Vendedor',  0.0260, 1, '1991-05-06', 'Rua Paraíba','200', NULL,'Funcionários','Belo Horizonte','MG','30130020', N'Filial Savassi'),
('EMP0206', N'Felipe Barros',      'Mecânico',  0.0150, 1, '1989-12-01', 'Rua Cláudio Manoel','90', NULL,'Funcionários','Belo Horizonte','MG','30140100', N'Filial Savassi'),
('EMP0207', N'Giovana Castro',     'Caixa',     0.0100, 1, '1996-04-03', 'Rua Antônio de Albuquerque','150',NULL,'Savassi','Belo Horizonte','MG','30112010', N'Filial Savassi'),
('EMP0208', N'Henrique Duarte',    'Estoque',   0.0000, 1, '1998-02-22', 'Rua Inconfidentes','70',NULL,'Funcionários','Belo Horizonte','MG','30140060', N'Filial Savassi'),
('EMP0209', N'Isabela Guimarães',  'Vendedor',  0.0240, 1, '1993-09-27', 'Rua da Bahia','2700',NULL,'Lourdes','Belo Horizonte','MG','30160010', N'Filial Savassi'),
('EMP0210', N'João Pedro Faria',   'Mecânico',  0.0130, 1, '1988-11-15', 'Rua Rio Grande do Norte','350',NULL,'Funcionários','Belo Horizonte','MG','30130030', N'Filial Savassi'),
('EMP0211', N'Larissa Tavares',    'Atendente', 0.0080, 1, '1997-07-07', 'Rua Gonçalves Dias','500',NULL,'Funcionários','Belo Horizonte','MG','30140010', N'Filial Savassi'),
('EMP0212', N'Mateus Rangel',      'Estoque',   0.0000, 1, '1999-01-26', 'Rua Pium-í','45',   NULL,'Carmo','Belo Horizonte','MG','30310080', N'Filial Savassi'),
('EMP0213', N'Nathalia Pimenta',   'Vendedor',  0.0270, 1, '1992-03-19', 'Rua Maranhão','640',NULL,'Funcionários','Belo Horizonte','MG','30150030', N'Filial Savassi'),
('EMP0214', N'Otávio Luz',         'Mecânico',  0.0160, 1, '1987-06-12', 'Rua Fernandes Tourinho','200',NULL,'Savassi','Belo Horizonte','MG','30112010', N'Filial Savassi'),
('EMP0215', N'Priscila Azevedo',   'Atendente', 0.0080, 1, '1998-09-04', 'Rua Paraíba','350', NULL,'Funcionários','Belo Horizonte','MG','30130020', N'Filial Savassi');

------------------------------------------------------------
-- 2) Inserir/obter endereços únicos (FK) e mapear id
------------------------------------------------------------
DECLARE @AddrMap TABLE (
    street VARCHAR(120), number VARCHAR(20), complement VARCHAR(60),
    neighborhood VARCHAR(60), city VARCHAR(60), state CHAR(2), zip CHAR(8),
    address_id INT
);

-- a) Reaproveita endereços já existentes
INSERT INTO @AddrMap (street, number, complement, neighborhood, city, state, zip, address_id)
SELECT DISTINCT E.street, E.number, E.complement, E.neighborhood, E.city, E.state, E.zip, A.id
FROM @EmpStage E
LEFT JOIN address A
  ON A.street=E.street AND A.number=E.number AND A.city=E.city AND A.state=E.state AND A.zip=E.zip;

-- b) Insere os que não existirem
INSERT INTO address (street, number, complement, neighborhood, city, state, zip, created_at)
SELECT M.street, M.number, M.complement, M.neighborhood, M.city, M.state, M.zip, SYSUTCDATETIME()
FROM @AddrMap M
WHERE M.address_id IS NULL;

-- c) Atualiza o map com IDs agora existentes
UPDATE M
SET address_id = A.id
FROM @AddrMap M
JOIN address A
  ON A.street=M.street AND A.number=M.number AND A.city=M.city AND A.state=M.state AND A.zip=M.zip;

------------------------------------------------------------
-- 3) Inserir funcionários apontando para fk_address e fk_branch
------------------------------------------------------------
INSERT INTO employee (
    registration, name, position, commission_rate, active, birth_date,
    fk_address, fk_branch, fk_department, created_at
)
SELECT
    E.registration, E.name, E.position, E.commission_rate, E.active, E.birth_date,
    M.address_id,
    B.branch_id,
    NULL, -- fk_department não obrigatório (coluna existe mas não há FK no script)
    SYSUTCDATETIME()
FROM @EmpStage E
JOIN @AddrMap  M ON M.street=E.street AND M.number=E.number AND M.city=E.city AND M.state=E.state AND M.zip=E.zip
JOIN @Branch   B ON B.corporate_name = E.corporate_name
WHERE NOT EXISTS (
    SELECT 1 FROM employee X
    WHERE X.registration = E.registration AND X.fk_branch = B.branch_id
);

--COMMIT;
ROLLBACK TRAN;