using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View(new ClienteModel());
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            return Salvar(model, false);
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            return Salvar(model, true);
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente boCliente = new BoCliente();
            Cliente cliente = boCliente.Consultar(id);
            if (cliente == null)
                return HttpNotFound();

            ClienteModel model = MontarModel(cliente);
            model.Beneficiarios = MontarModelBeneficiarios(new BoBeneficiario().Listar(cliente.Id));
            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                string[] ordenacao = (jtSorting ?? "NOME ASC").Split(' ');
                string campo = ordenacao.Length > 0 ? ordenacao[0] : "NOME";
                bool crescente = ordenacao.Length < 2 || ordenacao[1].Equals("ASC", StringComparison.OrdinalIgnoreCase);
                int quantidade;
                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente, out quantidade);
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = quantidade });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private JsonResult Salvar(ClienteModel model, bool alterar)
        {
            if (model == null)
            {
                Response.StatusCode = 400;
                return Json("Informe os dados do cliente.");
            }

            BoCliente boCliente = new BoCliente();
            if (!string.IsNullOrWhiteSpace(model.CPF) && boCliente.VerificarExistencia(model.CPF, model.Id))
                ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");

            ValidarBeneficiarios(model);
            if (!ModelState.IsValid)
                return ErroValidacao();

            try
            {
                Cliente cliente = MontarCliente(model);
                List<Beneficiario> beneficiarios = MontarBeneficiarios(model);
                if (alterar)
                    boCliente.Alterar(cliente, beneficiarios);
                else
                    boCliente.Incluir(cliente, beneficiarios);

                string mensagem = alterar ? "Cadastro alterado com sucesso." : "Cadastro efetuado com sucesso.";
                return Json(mensagem);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    Response.StatusCode = 400;
                    return Json("O CPF informado já está cadastrado.");
                }
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Response.StatusCode = 400;
                return Json(ex.Message);
            }
        }

        private JsonResult ErroValidacao()
        {
            Response.StatusCode = 400;
            return Json(ListarErros());
        }

        private string ListarErros()
        {
            List<string> erros = new List<string>();
            foreach (ModelState state in ModelState.Values)
            {
                foreach (ModelError erro in state.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(erro.ErrorMessage) && !erros.Contains(erro.ErrorMessage))
                        erros.Add(erro.ErrorMessage);
                }
            }
            return string.Join(Environment.NewLine, erros);
        }

        private void ValidarBeneficiarios(ClienteModel model)
        {
            HashSet<string> cpfs = new HashSet<string>(StringComparer.Ordinal);
            WebAtividadeEntrevista.Validation.CpfValidoAttribute validadorCpf = new WebAtividadeEntrevista.Validation.CpfValidoAttribute();
            if (model.Beneficiarios == null)
                return;

            for (int indice = 0; indice < model.Beneficiarios.Count; indice++)
            {
                BeneficiarioModel beneficiario = model.Beneficiarios[indice];
                if (beneficiario == null)
                {
                    ModelState.AddModelError("Beneficiarios[" + indice + "]", "Beneficiário inválido.");
                    continue;
                }

                if (!validadorCpf.IsValid(beneficiario.CPF))
                    ModelState.AddModelError("Beneficiarios[" + indice + "].CPF", "Informe um CPF válido para o beneficiário.");

                string cpf = Digitos(beneficiario.CPF);
                if (!cpfs.Add(cpf))
                    ModelState.AddModelError("Beneficiarios[" + indice + "].CPF", "Este CPF já está na lista de beneficiários.");

                if (string.IsNullOrWhiteSpace(beneficiario.Nome))
                    ModelState.AddModelError("Beneficiarios[" + indice + "].Nome", "Informe o nome do beneficiário.");
            }
        }

        private Cliente MontarCliente(ClienteModel model)
        {
            Cliente cliente = new Cliente();
            cliente.Id = model.Id;
            cliente.CEP = model.CEP;
            cliente.Cidade = model.Cidade;
            cliente.Email = model.Email;
            cliente.Estado = model.Estado;
            cliente.Logradouro = model.Logradouro;
            cliente.Nacionalidade = model.Nacionalidade;
            cliente.Nome = model.Nome;
            cliente.Sobrenome = model.Sobrenome;
            cliente.Telefone = model.Telefone;
            cliente.CPF = Digitos(model.CPF);
            return cliente;
        }

        private List<Beneficiario> MontarBeneficiarios(ClienteModel model)
        {
            List<Beneficiario> beneficiarios = new List<Beneficiario>();
            if (model.Beneficiarios == null)
                return beneficiarios;

            foreach (BeneficiarioModel item in model.Beneficiarios)
            {
                beneficiarios.Add(new Beneficiario
                {
                    Id = item.Id,
                    CPF = Digitos(item.CPF),
                    Nome = item.Nome
                });
            }
            return beneficiarios;
        }

        private ClienteModel MontarModel(Cliente cliente)
        {
            ClienteModel model = new ClienteModel();
            model.Id = cliente.Id;
            model.CEP = cliente.CEP;
            model.Cidade = cliente.Cidade;
            model.Email = cliente.Email;
            model.Estado = cliente.Estado;
            model.Logradouro = cliente.Logradouro;
            model.Nacionalidade = cliente.Nacionalidade;
            model.Nome = cliente.Nome;
            model.Sobrenome = cliente.Sobrenome;
            model.Telefone = cliente.Telefone;
            model.CPF = FormatarCpf(cliente.CPF);
            return model;
        }

        private List<BeneficiarioModel> MontarModelBeneficiarios(List<Beneficiario> beneficiarios)
        {
            List<BeneficiarioModel> models = new List<BeneficiarioModel>();
            foreach (Beneficiario beneficiario in beneficiarios)
            {
                models.Add(new BeneficiarioModel
                {
                    Id = beneficiario.Id,
                    CPF = FormatarCpf(beneficiario.CPF),
                    Nome = beneficiario.Nome
                });
            }
            return models;
        }

        private static string Digitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(caractere => caractere >= '0' && caractere <= '9').ToArray());
        }

        private static string FormatarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;
            return cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);
        }
    }
}
