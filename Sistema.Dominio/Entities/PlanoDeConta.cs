using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Dominio.Entities
{
    public class PlanoDeConta
    {
        [Key]
        public Guid IdPlanoDeConta { get; set; }

        [Display(Name = "Plano De Conta")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(50, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 1)]
        public string DescricaoPlanoDeConta { get; set; }

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(50, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 1)]
        public string CategoriaPlanoDeConta { get; set; }

        public bool ATIVO { get; set; }

        public Guid IdUsuario { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<Financeiro> Financeiro { get; set; }
    }
}
