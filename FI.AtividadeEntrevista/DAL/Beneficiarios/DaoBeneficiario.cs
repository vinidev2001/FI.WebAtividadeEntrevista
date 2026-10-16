using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    internal class DaoBeneficiario : AcessoDados
    {
        internal List<Beneficiario> Listar(long idCliente)
        {
            using (SqlConnection conexao = AbrirConexao())
                return Listar(conexao, null, idCliente);
        }

        internal void Incluir(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = Parametros(beneficiario);
            Executar("FI_SP_IncBeneficiario", parametros);
        }

        internal void Alterar(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = Parametros(beneficiario);
            parametros.Add(new SqlParameter("@ID", beneficiario.Id));
            Executar("FI_SP_AltBeneficiario", parametros);
        }

        internal void Excluir(long id, long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@ID", id));
            parametros.Add(new SqlParameter("@IDCLIENTE", idCliente));
            Executar("FI_SP_DelBeneficiario", parametros);
        }

        internal void Sincronizar(SqlConnection conexao, SqlTransaction transacao, long idCliente, IList<Beneficiario> beneficiarios)
        {
            List<Beneficiario> existentes = Listar(conexao, transacao, idCliente);
            IList<Beneficiario> recebidos = beneficiarios ?? new List<Beneficiario>();
            List<long> idsExistentes = existentes.Select(item => item.Id).ToList();

            foreach (Beneficiario item in recebidos)
            {
                if (item.Id > 0 && !idsExistentes.Contains(item.Id))
                    throw new InvalidOperationException("Beneficiário inválido para este cliente.");
            }

            foreach (Beneficiario item in existentes)
            {
                if (!recebidos.Any(recebido => recebido.Id == item.Id))
                    Excluir(conexao, transacao, item.Id, idCliente);
            }

            foreach (Beneficiario item in recebidos)
            {
                item.IdCliente = idCliente;
                if (item.Id > 0)
                    Alterar(conexao, transacao, item);
                else
                    Incluir(conexao, transacao, item);
            }
        }

        private List<Beneficiario> Listar(SqlConnection conexao, SqlTransaction transacao, long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@IDCLIENTE", idCliente));
            DataSet ds = Consultar(conexao, transacao, "FI_SP_ConsBeneficiariosPorCliente", parametros);
            List<Beneficiario> beneficiarios = new List<Beneficiario>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Beneficiario beneficiario = new Beneficiario();
                beneficiario.Id = Convert.ToInt64(row["ID"]);
                beneficiario.CPF = Convert.ToString(row["CPF"]);
                beneficiario.Nome = Convert.ToString(row["NOME"]);
                beneficiario.IdCliente = Convert.ToInt64(row["IDCLIENTE"]);
                beneficiarios.Add(beneficiario);
            }
            return beneficiarios;
        }

        private void Incluir(SqlConnection conexao, SqlTransaction transacao, Beneficiario beneficiario)
        {
            Executar(conexao, transacao, "FI_SP_IncBeneficiario", Parametros(beneficiario));
        }

        private void Alterar(SqlConnection conexao, SqlTransaction transacao, Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = Parametros(beneficiario);
            parametros.Add(new SqlParameter("@ID", beneficiario.Id));
            Executar(conexao, transacao, "FI_SP_AltBeneficiario", parametros);
        }

        private void Excluir(SqlConnection conexao, SqlTransaction transacao, long id, long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@ID", id));
            parametros.Add(new SqlParameter("@IDCLIENTE", idCliente));
            Executar(conexao, transacao, "FI_SP_DelBeneficiario", parametros);
        }

        private static List<SqlParameter> Parametros(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@CPF", Digitos(beneficiario.CPF)));
            parametros.Add(new SqlParameter("@NOME", beneficiario.Nome));
            parametros.Add(new SqlParameter("@IDCLIENTE", beneficiario.IdCliente));
            return parametros;
        }

        private static string Digitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(c => c >= '0' && c <= '9').ToArray());
        }
    }
}
