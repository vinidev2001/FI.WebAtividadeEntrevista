using FI.AtividadeEntrevista.DML;
using FI.AtividadeEntrevista.DAL;
using System.Collections.Generic;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        public long Incluir(Cliente cliente, IList<Beneficiario> beneficiarios)
        {
            DaoCliente dao = new DaoCliente();
            return dao.Incluir(cliente, beneficiarios);
        }

        public void Alterar(Cliente cliente, IList<Beneficiario> beneficiarios)
        {
            DaoCliente dao = new DaoCliente();
            dao.Alterar(cliente, beneficiarios);
        }

        public Cliente Consultar(long id)
        {
            DaoCliente dao = new DaoCliente();
            return dao.Consultar(id);
        }

        public void Excluir(long id)
        {
            DaoCliente dao = new DaoCliente();
            dao.Excluir(id);
        }

        public List<Cliente> Listar()
        {
            DaoCliente dao = new DaoCliente();
            return dao.Listar();
        }

        public List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            DaoCliente dao = new DaoCliente();
            return dao.Pesquisa(iniciarEm, quantidade, campoOrdenacao, crescente, out qtd);
        }

        public bool VerificarExistencia(string cpf, long ignorarId)
        {
            DaoCliente dao = new DaoCliente();
            return dao.VerificarExistencia(cpf, ignorarId);
        }
    }
}
