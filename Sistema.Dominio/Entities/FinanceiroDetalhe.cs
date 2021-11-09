using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Dominio.Entities
{
    public class FinanceiroDetalhe
    {

        [Key]
        public Guid IdFinanceiroDetalhe { get; set; }
        public Guid IdFinanceiro { get; set; }

        [Required(ErrorMessage = "A ordem está vazia!")]
        public Guid IdUnidade { get; set; }

        public decimal SubTotalFinanceiroDetalhe { get; set; }

        public Guid IdUsuario { get; set; }

        public virtual UnidadeNeg Unidade { get; set; }
        public virtual Financeiro Financeiro { get; set; }
        public virtual Usuario Usuario { get; set; }


        public FinanceiroDetalhe(Guid idFinanceiroDetalhe, Guid idFinanceiro, Guid idUnidade, decimal subTotalFinanceiroDetalhe, Guid usuario)
        {
            IdFinanceiroDetalhe = idFinanceiroDetalhe;
            IdFinanceiro = idFinanceiro;
            IdUnidade = idUnidade;
            SubTotalFinanceiroDetalhe = subTotalFinanceiroDetalhe;
            IdUsuario = usuario;
        }

        public FinanceiroDetalhe(Guid idFinanceiroDetalhe)
        {
            IdFinanceiroDetalhe = idFinanceiroDetalhe;
        }

        public FinanceiroDetalhe()
        {

        }
    }
}
