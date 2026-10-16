CREATE OR ALTER PROCEDURE dbo.FI_SP_PesqCliente
    @iniciarEm INT,
    @quantidade INT,
    @campoOrdenacao VARCHAR(200),
    @crescente BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @campo VARCHAR(20);
    DECLARE @ordem VARCHAR(4);
    DECLARE @sql NVARCHAR(MAX);

    SET @campo = CASE WHEN UPPER(@campoOrdenacao) = 'EMAIL' THEN 'EMAIL' ELSE 'NOME' END;
    SET @ordem = CASE WHEN @crescente = 0 THEN 'DESC' ELSE 'ASC' END;

    SET @sql = N'
        SELECT ID, NOME, SOBRENOME, NACIONALIDADE, CEP, ESTADO, CIDADE, LOGRADOURO, EMAIL, TELEFONE, CPF
        FROM (
            SELECT ROW_NUMBER() OVER (ORDER BY ' + QUOTENAME(@campo) + N' ' + @ordem + N') AS RowNumber,
                   ID, NOME, SOBRENOME, NACIONALIDADE, CEP, ESTADO, CIDADE, LOGRADOURO, EMAIL, TELEFONE, CPF
            FROM dbo.CLIENTES WITH (NOLOCK)
        ) AS Clientes
        WHERE RowNumber > @iniciarEm AND RowNumber <= (@iniciarEm + @quantidade)
        ORDER BY RowNumber;';

    EXECUTE sp_executesql @sql,
        N'@iniciarEm INT, @quantidade INT',
        @iniciarEm = @iniciarEm,
        @quantidade = @quantidade;

    SELECT COUNT(1) FROM dbo.CLIENTES WITH (NOLOCK);
END;
