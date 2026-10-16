using FI.AtividadeEntrevista.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace FI.WebAtividadeEntrevista.Infrastructure
{
    public static class DatabaseInitializer
    {
        public static void EnsureSchema()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["BancoDeDados"];
            if (settings == null)
                throw new ConfigurationErrorsException("A conexão BancoDeDados não foi configurada.");

            using (SqlConnection conexao = new SqlConnection(settings.ConnectionString))
            {
                conexao.Open();
                CriarColunaCpf(conexao);
                CriarTabelaBeneficiarios(conexao);
                CriarIndicesUnicos(conexao);
                CriarProcedures(conexao);
            }
        }

        private static void CriarColunaCpf(SqlConnection conexao)
        {
            Executar(conexao, @"IF COL_LENGTH('dbo.CLIENTES', 'CPF') IS NULL
    ALTER TABLE dbo.CLIENTES ADD CPF VARCHAR(11) NULL;");
        }

        private static void CriarTabelaBeneficiarios(SqlConnection conexao)
        {
            Executar(conexao, @"IF OBJECT_ID('dbo.BENEFICIARIOS', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BENEFICIARIOS
    (
        ID BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BENEFICIARIOS PRIMARY KEY,
        CPF VARCHAR(11) NOT NULL,
        NOME VARCHAR(255) NOT NULL,
        IDCLIENTE BIGINT NOT NULL,
        CONSTRAINT FK_BENEFICIARIOS_CLIENTES FOREIGN KEY (IDCLIENTE)
            REFERENCES dbo.CLIENTES(ID) ON DELETE CASCADE
    );
END;");
        }

        private static void CriarIndicesUnicos(SqlConnection conexao)
        {
            Executar(conexao, @"IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_CLIENTES_CPF' AND object_id = OBJECT_ID('dbo.CLIENTES'))
    CREATE UNIQUE INDEX UX_CLIENTES_CPF ON dbo.CLIENTES(CPF) WHERE CPF IS NOT NULL;");

            Executar(conexao, @"IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_BENEFICIARIOS_CLIENTE_CPF' AND object_id = OBJECT_ID('dbo.BENEFICIARIOS'))
    CREATE UNIQUE INDEX UX_BENEFICIARIOS_CLIENTE_CPF
        ON dbo.BENEFICIARIOS(IDCLIENTE, CPF);");
        }

        private static void CriarProcedures(SqlConnection conexao)
        {
            Assembly assembly = typeof(BoCliente).Assembly;
            List<string> resources = new List<string>();
            foreach (string nome in assembly.GetManifestResourceNames())
            {
                if (nome.Contains(".Procedures.") && nome.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                    resources.Add(nome);
            }
            resources.Sort(StringComparer.Ordinal);

            foreach (string resource in resources)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resource))
                {
                    if (stream == null)
                        throw new InvalidOperationException("Não foi possível carregar a procedure " + resource + ".");

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Executar(conexao, reader.ReadToEnd());
                    }
                }
            }
        }

        private static void Executar(SqlConnection conexao, string comandoSql)
        {
            using (SqlCommand comando = new SqlCommand(comandoSql, conexao))
                comando.ExecuteNonQuery();
        }
    }
}
