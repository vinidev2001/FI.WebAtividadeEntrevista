using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FI.AtividadeEntrevista.DAL
{
    internal class AcessoDados
    {
        protected string stringDeConexao
        {
            get
            {
                ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings["BancoDeDados"];
                if (conn != null)
                    return conn.ConnectionString;
                else
                    return string.Empty;
            }
        }

        internal SqlConnection AbrirConexao()
        {
            SqlConnection conexao = new SqlConnection(stringDeConexao);
            conexao.Open();
            return conexao;
        }

        internal void Executar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlConnection conexao = AbrirConexao())
            using (SqlCommand comando = CriarComando(conexao, null, NomeProcedure, parametros))
                comando.ExecuteNonQuery();
        }

        internal void Executar(SqlConnection conexao, SqlTransaction transacao, string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlCommand comando = CriarComando(conexao, transacao, NomeProcedure, parametros))
                comando.ExecuteNonQuery();
        }

        internal DataSet Consultar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlConnection conexao = AbrirConexao())
                return Consultar(conexao, null, NomeProcedure, parametros);
        }

        internal DataSet Consultar(SqlConnection conexao, SqlTransaction transacao, string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlCommand comando = CriarComando(conexao, transacao, NomeProcedure, parametros))
            using (SqlDataAdapter adapter = new SqlDataAdapter(comando))
            {
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }

        private SqlCommand CriarComando(SqlConnection conexao, SqlTransaction transacao, string NomeProcedure, List<SqlParameter> parametros)
        {
            SqlCommand comando = new SqlCommand(NomeProcedure, conexao, transacao);
            comando.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parametro in parametros)
                comando.Parameters.Add(parametro);
            return comando;
        }

    }
}
