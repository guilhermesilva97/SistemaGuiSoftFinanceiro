using Sistema.Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Data
{
    public class ContextGS:DbContext
    {
        public ContextGS() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ContextGS>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<Pessoa> Pessoas { get; set; }


        public DbSet<PlanoDeConta> PlanoDeConta { get; set; }

        public DbSet<ModoPagamento> ModoPagamento { get; set; }

        public DbSet<UnidadeNeg> UnidadeNeg { get; set; }

        public DbSet<Financeiro> Financeiro { get; set; }

        public DbSet<FinanceiroDetalhe> FinanceiroDetalhes { get; set; }

        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<ContaCorrente> ContaCorrente { get; set; }
    }
}
