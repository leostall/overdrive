SET XACT_ABORT ON;
BEGIN TRY
BEGIN TRAN;

------------------------------------------------------------
-- Mapear IDs dos estoques (Principal e Secundário por filial)
------------------------------------------------------------
DECLARE @paulista_principal  INT, @paulista_secundario  INT,
        @botafogo_principal  INT, @botafogo_secundario  INT,
        @savassi_principal   INT, @savassi_secundario   INT;

-- Filial Paulista
SELECT @paulista_principal = s.id
FROM stock s
JOIN branch b ON b.id = s.fk_branch
WHERE b.corporate_name = N'Filial Paulista' AND s.[note] = N'Principal';

SELECT @paulista_secundario = s.id
FROM stock s
JOIN branch b ON b.id = s.fk_branch
WHERE b.corporate_name = N'Filial Paulista' AND s.[note] = N'Secundário';

-- Filial Botafogo
SELECT @botafogo_principal = s.id
FROM stock s
JOIN branch b ON b.id = s.fk_branch
WHERE b.corporate_name = N'Filial Botafogo' AND s.[note] = N'Principal';

SELECT @botafogo_secundario = s.id
FROM stock s
JOIN branch b ON b.id = s.fk_branch
WHERE b.corporate_name = N'Filial Botafogo' AND s.[note] = N'Secundário';

-- Filial Savassi
SELECT @savassi_principal = s.id
FROM stock s
JOIN branch b ON b.id = s.fk_branch
WHERE b.corporate_name = N'Filial Savassi' AND s.[note] = N'Principal';

SELECT @savassi_secundario = s.id
FROM stock s
JOIN branch b ON b.id = s.fk_branch
WHERE b.corporate_name = N'Filial Savassi' AND s.[note] = N'Secundário';

-- Checagem
IF @paulista_principal IS NULL OR @paulista_secundario IS NULL
   OR @botafogo_principal IS NULL OR @botafogo_secundario IS NULL
   OR @savassi_principal  IS NULL OR @savassi_secundario  IS NULL
BEGIN
    RAISERROR('Não encontrei todos os estoques Principal/Secundário por filial.',16,1);
END

/* =========================================================
   INSERTS "NA MÃO" — 15 peças por estoque (90 ao todo)
   Somente colunas: code, description, quantity, price, fk_stock, created_at
   ========================================================= */

--------------------- Paulista - Principal (15) ---------------------
INSERT INTO dbo.part (code, description, quantity, price, fk_stock, created_at) VALUES
('PAU-PR-001', N'Filtro de óleo motor',                  30,  34.90, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-002', N'Filtro de ar',                          20,  59.90, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-003', N'Filtro de combustível',                 18,  42.50, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-004', N'Pastilha de freio dianteira',           12, 189.90, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-005', N'Disco de freio ventilado',               8, 349.00, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-006', N'Velas de ignição (jogo)',               25,  89.00, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-007', N'Correia dentada',                       14, 129.00, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-008', N'Amortecedor dianteiro',                  6, 499.00, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-009', N'Amortecedor traseiro',                   6, 479.00, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-010', N'Bateria 60Ah',                           5, 689.00, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-011', N'Lâmpada farol H7',                      40,  29.90, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-012', N'Palheta parabrisa (par)',               22,  69.90, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-013', N'Fluido de freio DOT 4 500ml',           16,  34.90, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-014', N'Aditivo de radiador 1L',                15,  39.90, @paulista_principal,  SYSUTCDATETIME()),
('PAU-PR-015', N'Óleo 5W30 sintético 1L',                36,  49.90, @paulista_principal,  SYSUTCDATETIME());

--------------------- Paulista - Secundário (15) ---------------------
INSERT INTO dbo.part (code, description, quantity, price, fk_stock, created_at) VALUES
('PAU-SC-001', N'Filtro de óleo motor',                  15,  34.90, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-002', N'Filtro de ar',                          12,  59.90, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-003', N'Filtro de cabine (ar-cond.)',           10,  64.90, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-004', N'Pastilha de freio traseira',             8, 169.90, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-005', N'Disco de freio sólido',                  6, 299.00, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-006', N'Velas de ignição (jogo)',               10,  89.00, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-007', N'Correia Poly-V',                         9,  99.00, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-008', N'Kit de embreagem',                       3, 859.00, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-009', N'Coxim do motor',                         5, 219.00, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-010', N'Bomba de combustível',                   4, 449.00, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-011', N'Sensor lambda',                          5, 339.00, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-012', N'Lâmpada farol H4',                      20,  24.90, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-013', N'Palheta traseira',                      15,  34.90, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-014', N'Aditivo limpa para-brisa 500ml',        12,  19.90, @paulista_secundario, SYSUTCDATETIME()),
('PAU-SC-015', N'Óleo 5W40 sintético 1L',                20,  54.90, @paulista_secundario, SYSUTCDATETIME());

--------------------- Botafogo - Principal (15) ---------------------
INSERT INTO dbo.part (code, description, quantity, price, fk_stock, created_at) VALUES
('BOT-PR-001', N'Filtro de óleo motor',                  25,  34.90, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-002', N'Filtro de ar',                          18,  59.90, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-003', N'Filtro de combustível',                 14,  42.50, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-004', N'Pastilha de freio dianteira',           10, 189.90, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-005', N'Disco de freio ventilado',               7, 349.00, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-006', N'Velas de ignição (jogo)',               18,  89.00, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-007', N'Correia dentada',                       12, 129.00, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-008', N'Amortecedor dianteiro',                  4, 499.00, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-009', N'Amortecedor traseiro',                   4, 479.00, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-010', N'Bateria 60Ah',                           4, 689.00, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-011', N'Lâmpada farol H7',                      30,  29.90, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-012', N'Palheta parabrisa (par)',               16,  69.90, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-013', N'Fluido de freio DOT 4 500ml',           12,  34.90, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-014', N'Aditivo de radiador 1L',                12,  39.90, @botafogo_principal,  SYSUTCDATETIME()),
('BOT-PR-015', N'Óleo 5W30 sintético 1L',                28,  49.90, @botafogo_principal,  SYSUTCDATETIME());

--------------------- Botafogo - Secundário (15) ---------------------
INSERT INTO dbo.part (code, description, quantity, price, fk_stock, created_at) VALUES
('BOT-SC-001', N'Filtro de óleo motor',                  12,  34.90, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-002', N'Filtro de ar',                          10,  59.90, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-003', N'Filtro de cabine (ar-cond.)',           10,  64.90, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-004', N'Pastilha de freio traseira',             6, 169.90, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-005', N'Disco de freio sólido',                  5, 299.00, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-006', N'Velas de ignição (jogo)',                8,  89.00, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-007', N'Correia Poly-V',                         8,  99.00, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-008', N'Kit de embreagem',                       3, 859.00, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-009', N'Coxim do motor',                         4, 219.00, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-010', N'Bomba de combustível',                   3, 449.00, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-011', N'Sensor lambda',                          4, 339.00, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-012', N'Lâmpada farol H4',                      16,  24.90, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-013', N'Palheta traseira',                      12,  34.90, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-014', N'Aditivo limpa para-brisa 500ml',        10,  19.90, @botafogo_secundario, SYSUTCDATETIME()),
('BOT-SC-015', N'Óleo 5W40 sintético 1L',                15,  54.90, @botafogo_secundario, SYSUTCDATETIME());

--------------------- Savassi - Principal (15) ---------------------
INSERT INTO dbo.part (code, description, quantity, price, fk_stock, created_at) VALUES
('SAV-PR-001', N'Filtro de óleo motor',                  22,  34.90, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-002', N'Filtro de ar',                          16,  59.90, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-003', N'Filtro de combustível',                 12,  42.50, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-004', N'Pastilha de freio dianteira',            9, 189.90, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-005', N'Disco de freio ventilado',               6, 349.00, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-006', N'Velas de ignição (jogo)',               16,  89.00, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-007', N'Correia dentada',                       10, 129.00, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-008', N'Amortecedor dianteiro',                  4, 499.00, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-009', N'Amortecedor traseiro',                   4, 479.00, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-010', N'Bateria 60Ah',                           4, 689.00, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-011', N'Lâmpada farol H7',                      28,  29.90, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-012', N'Palheta parabrisa (par)',               14,  69.90, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-013', N'Fluido de freio DOT 4 500ml',           10,  34.90, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-014', N'Aditivo de radiador 1L',                10,  39.90, @savassi_principal,   SYSUTCDATETIME()),
('SAV-PR-015', N'Óleo 5W30 sintético 1L',                24,  49.90, @savassi_principal,   SYSUTCDATETIME());

--------------------- Savassi - Secundário (15) ---------------------
INSERT INTO dbo.part (code, description, quantity, price, fk_stock, created_at) VALUES
('SAV-SC-001', N'Filtro de óleo motor',                  10,  34.90, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-002', N'Filtro de ar',                          10,  59.90, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-003', N'Filtro de cabine (ar-cond.)',           10,  64.90, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-004', N'Pastilha de freio traseira',             6, 169.90, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-005', N'Disco de freio sólido',                  5, 299.00, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-006', N'Velas de ignição (jogo)',                8,  89.00, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-007', N'Correia Poly-V',                         8,  99.00, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-008', N'Kit de embreagem',                       2, 859.00, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-009', N'Coxim do motor',                         4, 219.00, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-010', N'Bomba de combustível',                   3, 449.00, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-011', N'Sensor lambda',                          4, 339.00, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-012', N'Lâmpada farol H4',                      14,  24.90, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-013', N'Palheta traseira',                      12,  34.90, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-014', N'Aditivo limpa para-brisa 500ml',         8,  19.90, @savassi_secundario,  SYSUTCDATETIME()),
('SAV-SC-015', N'Óleo 5W40 sintético 1L',                12,  54.90, @savassi_secundario,  SYSUTCDATETIME());

COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;