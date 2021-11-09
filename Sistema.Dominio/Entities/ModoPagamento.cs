using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Dominio.Entities
{
    public class ModoPagamento
    {
        [Key]
        public Guid IdPagamento { get; set; }

        [Display(Name = "Modo De Pagamento")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(30, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 1)]
        public string DescricaoPagamento { get; set; }

        public bool ATIVO { get; set; }

        public Guid IdUsuario { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Financeiro> financeiro { get; set; }
    }
}
