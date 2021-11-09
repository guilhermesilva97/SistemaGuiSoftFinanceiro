using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Dominio.Entities
{
    public class Usuario
    {
        [Key]
        public Guid IdUsuario { get; set; }

        [Display(Name = "Login")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(20, ErrorMessage = " O Campo {0} deve ter no minimo {2} caracteres ", MinimumLength = 6)]
        public string LoginUsuario { get; set; }


        [Display(Name = "Senha")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(6, ErrorMessage = " Sua senha deve ter 6 digitos! ", MinimumLength = 6)]
        public string Senha { get; set; }

        public virtual ICollection<ContaCorrente> ContaCorrente { get; set; }
        public virtual ICollection<Pessoa> Pessoa { get; set; }
        public virtual ICollection<Financeiro> Financeiro { get; set; }
        public virtual ICollection<PlanoDeConta> PlanoDeConta { get; set; }
        public virtual ICollection<FinanceiroDetalhe> FinanceiroDetalhe { get; set; }
        public virtual ICollection<ModoPagamento> ModoPagamento { get; set; }
        public virtual ICollection<UnidadeNeg> UnidadeNeg { get; set; }
    }
}
