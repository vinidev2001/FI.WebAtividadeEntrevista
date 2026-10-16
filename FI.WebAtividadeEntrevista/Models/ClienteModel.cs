using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    public class ClienteModel
    {
        public long Id { get; set; }
        [Required]
        public string CEP { get; set; }
        [Required]
        public string Cidade { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Digite um e-mail válido")]
        public string Email { get; set; }
        [Required]
        [MaxLength(2)]
        public string Estado { get; set; }
        [Required]
        public string Logradouro { get; set; }
        [Required]
        public string Nacionalidade { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Sobrenome { get; set; }
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O CPF do cliente é obrigatório")]
        [WebAtividadeEntrevista.Validation.CpfValido(ErrorMessage = "Digite um CPF válido")]
        public string CPF { get; set; }

        public List<BeneficiarioModel> Beneficiarios { get; set; }

    }    

    public class BeneficiarioModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "O CPF do beneficiário é obrigatório")]
        [WebAtividadeEntrevista.Validation.CpfValido(ErrorMessage = "Digite um CPF válido para o beneficiário")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O nome do beneficiário é obrigatório")]
        [StringLength(255)]
        public string Nome { get; set; }
    }
}
