using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.Dominio.Entities
{
    public class Pessoa
    {
        [Key]
        public Guid IdPessoa { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(100, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 2)]
        public string NomePessoa { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(15, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        public string TelefonePessoa { get; set; }

        [Display(Name = "E-Mail")]
        [StringLength(75, ErrorMessage = "O Campo {0} pode ter no máximo {1} caracteres")]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Informe um email válido")]
        public string EmailPessoa { get; set; }

        [Display(Name = "CPF/CNPJ")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(25, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        [Index(IsUnique = true)]
        public string IdentificacaoPessoa { get; set; }

        [Display(Name = "CEP")]
        [StringLength(10, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        public string CepPessoa { get; set; }


        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(50, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        public string LogradouroPessoa { get; set; }

        [Display(Name = "Número")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(5, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        public string NumeroPessoa { get; set; }

        [Display(Name = "Compl")]
        [StringLength(15, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        public string ComplementoPessoa { get; set; }

        [Display(Name = "Bairro")]
        [StringLength(30, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        public string BairroPessoa { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(30, ErrorMessage = " O Campo {0} pode ter no máximo {1} caracteres ")]
        public string CidadePessoa { get; set; }

        [Display(Name = "UF")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(2, ErrorMessage = " O Campo {0} pode ter no máximo {1} e no mínimo {2} caractéres", MinimumLength =2)]
        public string EstadoPessoa { get; set; }

        public Guid IdUsuario { get; set; }


        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Financeiro> financeiro { get; set; }
    }
}
