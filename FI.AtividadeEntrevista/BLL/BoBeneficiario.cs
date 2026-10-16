using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        public List<Beneficiario> Listar(long idCliente)
        {
            return new DAL.DaoBeneficiario().Listar(idCliente);
        }

        public void Incluir(Beneficiario beneficiario)
        {
            new DAL.DaoBeneficiario().Incluir(beneficiario);
        }

        public void Alterar(Beneficiario beneficiario)
        {
            new DAL.DaoBeneficiario().Alterar(beneficiario);
        }

        public void Excluir(long id, long idCliente)
        {
            new DAL.DaoBeneficiario().Excluir(id, idCliente);
        }
    }
}
