using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    internal class DaoCliente : AcessoDados
    {
        internal long Incluir(Cliente cliente, IList<Beneficiario> beneficiarios)
        {
            using (SqlConnection conexao = AbrirConexao())
            using (SqlTransaction transacao = conexao.BeginTransaction())
            {
                DataSet ds = Consultar(conexao, transacao, "FI_SP_IncCliente", ParametrosCliente(cliente));
                long idCliente = Convert.ToInt64(ds.Tables[0].Rows[0][0]);
                new DaoBeneficiario().Sincronizar(conexao, transacao, idCliente, beneficiarios);
                transacao.Commit();
                return idCliente;
            }
        }

        internal void Alterar(Cliente cliente, IList<Beneficiario> beneficiarios)
        {
            using (SqlConnection conexao = AbrirConexao())
            using (SqlTransaction transacao = conexao.BeginTransaction())
            {
                Executar(conexao, transacao, "FI_SP_AltCliente", ParametrosCliente(cliente, true));
                new DaoBeneficiario().Sincronizar(conexao, transacao, cliente.Id, beneficiarios);
                transacao.Commit();
            }
        }

        internal Cliente Consultar(long id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@ID", id));
            DataSet ds = Consultar("FI_SP_ConsCliente", parametros);
            return Converter(ds).FirstOrDefault();
        }

        internal bool VerificarExistencia(string cpf, long ignorarId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@CPF", Digitos(cpf)));
            parametros.Add(new SqlParameter("@ID", ignorarId));
            DataSet ds = Consultar("FI_SP_VerificaCliente", parametros);
            return ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && Convert.ToBoolean(ds.Tables[0].Rows[0][0]);
        }

        internal List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@iniciarEm", iniciarEm));
            parametros.Add(new SqlParameter("@quantidade", quantidade));
            parametros.Add(new SqlParameter("@campoOrdenacao", campoOrdenacao));
            parametros.Add(new SqlParameter("@crescente", crescente));
            DataSet ds = Consultar("FI_SP_PesqCliente", parametros);
            int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out qtd);
            return Converter(ds);
        }

        internal List<Cliente> Listar()
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@ID", 0));
            return Converter(Consultar("FI_SP_ConsCliente", parametros));
        }

        internal void Excluir(long id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@ID", id));
            Executar("FI_SP_DelCliente", parametros);
        }

        private static List<SqlParameter> ParametrosCliente(Cliente cliente, bool incluirId = false)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Nome", cliente.Nome));
            parametros.Add(new SqlParameter("@Sobrenome", cliente.Sobrenome));
            parametros.Add(new SqlParameter("@Nacionalidade", cliente.Nacionalidade));
            parametros.Add(new SqlParameter("@CEP", cliente.CEP));
            parametros.Add(new SqlParameter("@Estado", cliente.Estado));
            parametros.Add(new SqlParameter("@Cidade", cliente.Cidade));
            parametros.Add(new SqlParameter("@Logradouro", cliente.Logradouro));
            parametros.Add(new SqlParameter("@Email", (object)cliente.Email ?? DBNull.Value));
            parametros.Add(new SqlParameter("@Telefone", (object)cliente.Telefone ?? DBNull.Value));
            parametros.Add(new SqlParameter("@CPF", Digitos(cliente.CPF)));
            if (incluirId)
                parametros.Add(new SqlParameter("@ID", cliente.Id));
            return parametros;
        }

        private static List<Cliente> Converter(DataSet ds)
        {
            List<Cliente> clientes = new List<Cliente>();
            if (ds == null || ds.Tables.Count == 0)
                return clientes;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Cliente cliente = new Cliente();
                cliente.Id = Convert.ToInt64(row["ID"]);
                cliente.CEP = Valor(row, "CEP");
                cliente.Cidade = Valor(row, "CIDADE");
                cliente.Email = Valor(row, "EMAIL");
                cliente.Estado = Valor(row, "ESTADO");
                cliente.Logradouro = Valor(row, "LOGRADOURO");
                cliente.Nacionalidade = Valor(row, "NACIONALIDADE");
                cliente.Nome = Valor(row, "NOME");
                cliente.Sobrenome = Valor(row, "SOBRENOME");
                cliente.Telefone = Valor(row, "TELEFONE");
                cliente.CPF = Valor(row, "CPF");
                clientes.Add(cliente);
            }
            return clientes;
        }

        private static string Valor(DataRow row, string coluna)
        {
            if (!row.Table.Columns.Contains(coluna) || row.IsNull(coluna))
                return null;
            return Convert.ToString(row[coluna]);
        }

        private static string Digitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(c => c >= '0' && c <= '9').ToArray());
        }
    }
}
