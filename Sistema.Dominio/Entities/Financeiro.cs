
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sistema.Dominio.Entities
{
    public class Financeiro
    {
        [Key]
        public Guid IdFinanceiro { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = " O Campo {0} é Obrigatório!")]
        [StringLength(50, ErrorMessage = " O Campo {0} pode ter no máximo {1} e minimo {2} caracteres ", MinimumLength = 5)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O valor do financeiro está nulo!")]
        public decimal TotalFinanceiro { get; set; }

        [Required(ErrorMessage = " Selecione uma pessoa!")]
        public Guid IdPessoa { get; set; }

        [Required(ErrorMessage = " Selecione um plano de conta!")]
        public Guid IdPlanoDeConta { get; set; }

        [Required(ErrorMessage = " Selecione uma forma de pagamento!")]
        public Guid IdPagamento { get; set; }

        [Required(ErrorMessage = " Selecione uma data de lançamento ")]
        public DateTime DataLancamento { get; set; }

        public DateTime? DataPagamento { get; set; }

        [Required(ErrorMessage = " Selecione uma data de vencimento")]
        public DateTime DataVencimento { get; set; }


        public Guid IdUsuario { get; set; }

        [Required(ErrorMessage = " Selecione uma conta para o financeiro!")]
        public Guid IdContaCorrente {get;set;}


        public virtual Usuario Usuario { get; set; }
        public virtual ContaCorrente ContaCorrente { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual PlanoDeConta PlanoDeConta { get; set; }
        public virtual ModoPagamento ModoPagamento { get; set; }
        public virtual ICollection<FinanceiroDetalhe> FinanceiroDetalhe { get; set; }
    }
}
