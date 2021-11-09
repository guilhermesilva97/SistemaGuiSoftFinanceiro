using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Dominio.Entities
{
    public class UnidadeNeg
    {
        [Key]
        public Guid IdUnidade { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(50, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 1)]
        public string NomeUnidadeNeg { get; set; }

        public Guid IdUsuario { get; set; }


        public virtual Usuario Usuario {get;set;}
        public virtual ICollection<FinanceiroDetalhe> FinanceiroDetalhe { get; set; }
    }
}
