using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Dominio.Entities
{
    public class ContaCorrente
    {
        [Key]
        public Guid IdContaCorrente { get; set; }

        [Display(Name = "Descrição De Caixa")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(50, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 1)]
        public string BancoContaCorrente { get; set; }

        [Display(Name = "Agência")]
        [StringLength(10, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 1)]
        public string AgenciaContaCorrente { get; set; }

        [Display(Name = "Conta")]
        [StringLength(15, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 1)]
        public string ContaContaCorrente { get; set; }

        public bool ATIVO { get; set; }
        public Guid IdUsuario { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Financeiro> Financeiro { get; set; }
    }
}
