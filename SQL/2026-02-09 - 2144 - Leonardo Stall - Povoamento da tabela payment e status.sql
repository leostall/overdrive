SET XACT_ABORT ON;
BEGIN TRAN;

------------------------------------------------------------
-- STATUS (situação de venda/OS)
-- Exemplos: pendente → aprovado → faturado → cancelado → devolvido etc.
------------------------------------------------------------
;WITH S(description) AS (
    SELECT N'Pendente'    UNION ALL
    SELECT N'Aprovado'    UNION ALL
    SELECT N'Em Separação' UNION ALL
    SELECT N'Faturado'    UNION ALL
    SELECT N'Despachado'  UNION ALL
    SELECT N'Entregue'    UNION ALL
    SELECT N'Cancelado'   UNION ALL
    SELECT N'Devolvido'
)
INSERT INTO dbo.[status] (description)
SELECT S.description
FROM S
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.[status] x WHERE x.description = S.description
);

------------------------------------------------------------
-- PAYMENT (métodos de pagamento)
-- payment_method: nome curto/código; description: detalhamento
------------------------------------------------------------
;WITH P(payment_method, description) AS (
    SELECT N'Dinheiro'       , N'Pagamento em espécie'                                   UNION ALL
    SELECT N'Pix'            , N'Transferência instantânea (Pix)'                        UNION ALL
    SELECT N'CartaoCredito'  , N'Cartão de crédito à vista'                              UNION ALL
    SELECT N'CartaoCreditoParc', N'Cartão de crédito parcelado (2–12x)'                  UNION ALL
    SELECT N'CartaoDebito'   , N'Cartão de débito'                                       UNION ALL
    SELECT N'Boleto'         , N'Boleto bancário'                                        UNION ALL
    SELECT N'Transferencia'  , N'TED/DOC/transferência bancária'                         UNION ALL
    SELECT N'Cheque'         , N'Cheque (sujeito à compensação)'
)
INSERT INTO dbo.[payment] (payment_method, description)
SELECT P.payment_method, P.description
FROM P
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.[payment] x WHERE x.payment_method = P.payment_method
);

COMMIT;