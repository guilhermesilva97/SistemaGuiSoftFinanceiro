using CrystalDecisions.CrystalReports.Engine;
using Sistema.Data;
using Sistema.Dominio.Core;
using Sistema.Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GuiSoft.Controllers
{
    public class CaixaController : Controller
    {
        ContextGS db = new ContextGS();

        private class RelatorioMensal
        {
            internal string NomeDoCaixa;
            internal string PlanoDeConta;
            internal decimal Total;
            internal Guid Id;
            internal string Tipo;
            internal decimal TotalDebito;
            internal decimal TotalCredito;
            internal decimal SaldoAnterior;
            internal decimal SaldoAtual;
        }

        private class RelatorioAnual
        {
            public decimal Total { get; internal set; }
            public string Conta { get; internal set; }
            public string PlanoDeConta { get; internal set; }
            public string Tipo { get; internal set; }
            public decimal Janeiro { get; internal set; }
            public decimal Fevereiro { get; internal set; }
            public decimal Marco { get; internal set; }
            public decimal Abril { get; internal set; }
            public decimal Maio { get; internal set; }
            public decimal Junho { get; internal set; }
            public decimal Julho { get; internal set; }
            public decimal Agosto { get; internal set; }
            public decimal Setembro { get; internal set; }
            public decimal Outubro { get; internal set; }
            public decimal Novembro { get; internal set; }
            public decimal Dezembro { get; internal set; }

        }

        #region GERAR RELATÓRIO
        public ActionResult exportReport(string PeriodoRelatorio, string Opcao, DateTime? DataDe, DateTime? DataAte, string TipoDeFiltro)
        {

            ReportDocument rd = new ReportDocument();

            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;
            var IdUsuario = db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario;

            #region OPÇÃO EXTRATO
            if (Opcao == "Extrato")
            {
                // QUERY PARA RECUPERAR OS DADOS
                var query = (
                from a in db.Financeiro
                join b in db.Pessoas on a.IdPessoa equals b.IdPessoa
                join c in db.ModoPagamento on a.IdPagamento equals c.IdPagamento
                join d in db.PlanoDeConta on a.IdPlanoDeConta equals d.IdPlanoDeConta
                join e in db.ContaCorrente on a.IdContaCorrente equals e.IdContaCorrente
                where a.IdUsuario == IdUsuario

                select new
                {
                    DescricaoFinanceiro = a.Descricao,
                    Pessoa = b.NomePessoa,
                    Conta = e.BancoContaCorrente,
                    c.DescricaoPagamento,
                    d.CategoriaPlanoDeConta,
                    a.DataPagamento,
                    a.DataLancamento,
                    a.DataVencimento,
                    a.TotalFinanceiro
                });


                // APLICANDO OS FILTROS DE ACORDO COM O QUE FOI PASSADO COMO PARÂMETRO
                var Filtro = "";
                switch (TipoDeFiltro)
                {
                    case "PAGAMENTO":
                        Filtro = "Ordens Pagas";
                        query = query
                            .Where(x => x.DataPagamento >= DataDe && x.DataPagamento <= DataAte)
                            .OrderBy(x=> x.DataPagamento);
                        break;

                    case "LANÇAMENTO":
                        Filtro = "Ordens Lançadas";
                        query = query
                            .Where(x => x.DataLancamento >= DataDe && x.DataLancamento <= DataAte)
                            .OrderBy(x=> x.DataLancamento);
                        break;

                    case "VENCIMENTO":
                        Filtro = "Ordens Vencidas";
                        query = query
                            .Where(x => x.DataVencimento >= DataDe && x.DataVencimento <= DataAte)
                            .OrderBy(x=> x.DataVencimento);
                        break;
                }

                // PASSANDO A QUERY PARA UMA CLASSE ANONIMA PARA NEGATIVAR OS VALORES QUE SÃO DÉBITO
                var ItensDoExtrato = query
                    .Select(x => new
                {
                    DescricaoFinanceiro = x.DescricaoFinanceiro,
                    Conta = x.Conta,
                    Pessoa = x.Pessoa,
                    DescricaoPagamento = x.DescricaoFinanceiro,
                    DataPagamento = x.DataPagamento,
                    DataLancamento = x.DataLancamento,
                    DataVencimento = x.DataVencimento,
                    TotalFinanceiro = x.TotalFinanceiro * (x.CategoriaPlanoDeConta == "DÉBITO" ? -1 : 1)
                });

                rd.Load(Path.Combine(Server.MapPath("~/Relatorio/RelatorioCaixa.rpt")));
                rd.SetDataSource(ItensDoExtrato.ToList().ToDataTable());
                rd.SetParameterValue("Usuario", User);
                rd.SetParameterValue("DataDe", DataDe);
                rd.SetParameterValue("DataAte", DataAte);
                rd.SetParameterValue("Tipo", Filtro.ToString());
            }
            #endregion

            #region OPÇÃO BALANCETE MENSAL
            else if (Opcao == "BalanceteMensal")
            {
                // QUERY PARA RECUPERAR OS DADOS
                var query = (
                  from a in db.Financeiro
                  join b in db.PlanoDeConta on a.IdPlanoDeConta equals b.IdPlanoDeConta
                  join c in db.ContaCorrente on a.IdContaCorrente equals c.IdContaCorrente

                  where a.IdUsuario == IdUsuario

                  select new
                  {
                      a.IdPlanoDeConta,
                      PlanoDeConta = b.DescricaoPlanoDeConta,
                      CategoriaPlanoDeConta = b.CategoriaPlanoDeConta,
                      DataLanc = a.DataLancamento,
                      DataPag = a.DataPagamento,
                      DataVenc = a.DataVencimento,
                      Total = a.TotalFinanceiro,
                      Conta = c.BancoContaCorrente
                  });


                // LISTANDO OS DADOS E NEGATIVANDO OS VALORES QUE FOREM DÉBITO
                var ItensDaQuery = query
                    .Select(x => new
                    {
                        IdPlanoDeConta = x.IdPlanoDeConta,
                        PlanoDeConta = x.PlanoDeConta,
                        DataLanc = x.DataLanc,
                        DataPag = x.DataPag,
                        DataVenc = x.DataVenc,
                        Total = x.Total * (x.CategoriaPlanoDeConta == "DÉBITO" ? -1 : 1),
                        Conta = x.Conta
                    });


                // RECUPERANDO OS DADOS SEM FILTRO PARA PEGAR O SALDO ANTERIOR
                var ExtratoDeCaixaAntesDaDataDeFiltro = ItensDaQuery
                    .Select(x => new
                    {
                        DataLanc = x.DataLanc,
                        DataPag = x.DataPag,
                        DataVenc = x.DataVenc,
                        Total = x.Total,
                        Conta = x.Conta
                    });

                decimal SaldoAnterior = 0;
                // APLICANDO OS FILTROS DE ACORDO COM O QUE FOI PASSADO COMO PARÂMETRO
                var Filtro = "";
                switch (TipoDeFiltro)
                {
                    case "PAGAMENTO":
                        Filtro = "Ordens Pagas";
                        ItensDaQuery = ItensDaQuery
                            .Where(x => x.DataPag != null && x.DataPag.Value.Month == DataDe.Value.Month);

                        ExtratoDeCaixaAntesDaDataDeFiltro = ExtratoDeCaixaAntesDaDataDeFiltro
                            .Where(x => x.DataPag < DataDe);
                        SaldoAnterior = ExtratoDeCaixaAntesDaDataDeFiltro.Sum(x => x.Total);
                        break;

                    case "LANÇAMENTO":
                        Filtro = "Ordens Lançadas";
                        query = query
                            .Where(x => x.DataLanc.Month == DataDe.Value.Month);

                        ExtratoDeCaixaAntesDaDataDeFiltro = ExtratoDeCaixaAntesDaDataDeFiltro
                            .Where(x => x.DataLanc < DataDe);
                        SaldoAnterior = ExtratoDeCaixaAntesDaDataDeFiltro.Sum(x => x.Total);
                        break;

                    case "VENCIMENTO":
                        Filtro = "Ordens Vencidas";
                        query = query
                            .Where(x => x.DataVenc.Month == DataDe.Value.Month);

                        ExtratoDeCaixaAntesDaDataDeFiltro = 
                            ExtratoDeCaixaAntesDaDataDeFiltro.Where(x => x.DataVenc < DataDe);

                        break;
                }


                // CRIANDO LISTA DOS PLANOS DE CONTA
                var ItensPlanoDeConta = ItensDaQuery.ToList()
                    .GroupBy(x => x.PlanoDeConta)
                    .Select(j => new RelatorioMensal()
                    {
                        Id = j.First().IdPlanoDeConta,
                        PlanoDeConta = j.First().PlanoDeConta,
                        Total = j.Sum(x => x.Total),
                        Tipo = "1"
                    });


                // CRIANDO LISTA DOS CAIXAS
                var ItensParaCaixa = ItensDaQuery.ToList()
                    .Select(j => new RelatorioMensal()
                    {
                        NomeDoCaixa = j.Conta,
                        Total = j.Total,
                        TotalDebito = (j.Total < 0 ? j.Total : 0),
                        TotalCredito = (j.Total > 0 ? j.Total : 0)
                    });

                var ItensSomadosCaixa = ItensParaCaixa.
                    ToList()
                    .GroupBy(x => x.NomeDoCaixa)
                    .Select(x => new RelatorioMensal()
                    {
                        NomeDoCaixa = x.First().NomeDoCaixa,
                        Tipo = "0",
                        TotalCredito = x.Sum(y => y.TotalCredito),
                        TotalDebito = x.Sum(y => y.TotalDebito),
                        SaldoAnterior = ExtratoDeCaixaAntesDaDataDeFiltro.ToList().Where(y=> y.Conta == x.First().NomeDoCaixa).Sum(z=> z.Total),
                        SaldoAtual = ExtratoDeCaixaAntesDaDataDeFiltro.ToList().Where(y => y.Conta == x.First().NomeDoCaixa).Sum(z => z.Total) + x.Sum(y => y.Total)
                    }).ToList();



                var Envio = ItensPlanoDeConta.Union(ItensSomadosCaixa);
                rd.Load(Path.Combine(Server.MapPath("~/Relatorio/RelatorioMensal.rpt")));
                rd.SetDataSource(Envio);
                rd.SetParameterValue("Usuario", User);
                rd.SetParameterValue("DataDe", DataDe);
                rd.SetParameterValue("Tipo", Filtro.ToString());
            }
            #endregion

            #region BALANCETE ANUAL
            else
            {
                var query = (
                  from a in db.Financeiro
                  join b in db.PlanoDeConta on a.IdPlanoDeConta equals b.IdPlanoDeConta
                  join c in db.ContaCorrente on a.IdContaCorrente equals c.IdContaCorrente

                  where a.IdUsuario == IdUsuario

                  select new
                  {
                      Descricao = b.DescricaoPlanoDeConta,
                      CategoriaPlanoDeConta = b.CategoriaPlanoDeConta,
                      DataPag = a.DataPagamento,
                      DataVenc = a.DataVencimento,
                      Total = a.TotalFinanceiro,
                      Conta = c.BancoContaCorrente
                  });

                var ItensDaQuery = query
                    .Select(x => new
                {
                    Descricao = x.Descricao,
                    DataPag = x.DataPag,
                    DataVenc = x.DataVenc,
                    Total = x.Total*(x.CategoriaPlanoDeConta=="DÉBITO"? -1:1),
                    Conta = x.Conta
                }).ToList();

                var ItensDoPlanoDeConta = ItensDaQuery
                    .GroupBy(x => x.Descricao)
                    .Select(y => new RelatorioAnual()
                {
                    Tipo = "1",
                    PlanoDeConta = y.First().Descricao,
                    Janeiro = y.Where(x=>x.DataPag.Value.Month== 1).Sum(z=> z.Total),
                    Fevereiro = y.Where(x => x.DataPag.Value.Month == 2).Sum(z => z.Total),
                    Marco = y.Where(x => x.DataPag.Value.Month == 3).Sum(z => z.Total),
                    Abril = y.Where(x => x.DataPag.Value.Month == 4).Sum(z => z.Total),
                    Maio = y.Where(x => x.DataPag.Value.Month == 5).Sum(z => z.Total),
                    Junho = y.Where(x => x.DataPag.Value.Month == 6).Sum(z => z.Total),
                    Julho = y.Where(x => x.DataPag.Value.Month == 7).Sum(z => z.Total),
                    Agosto = y.Where(x => x.DataPag.Value.Month == 8).Sum(z => z.Total),
                    Setembro = y.Where(x => x.DataPag.Value.Month == 9).Sum(z => z.Total),
                    Outubro = y.Where(x => x.DataPag.Value.Month == 10).Sum(z => z.Total),
                    Novembro = y.Where(x => x.DataPag.Value.Month == 11).Sum(z => z.Total),
                    Dezembro = y.Where(x => x.DataPag.Value.Month == 12).Sum(z => z.Total),
                    Total = y.Sum(x=> x.Total)
                });

                var ItensDoCaixa = ItensDaQuery
                    .GroupBy(x => x.Conta)
                    .Select(y => new RelatorioAnual()
                {
                    Tipo = "0",
                    Janeiro = y.Where(x => x.DataPag.Value.Month == 1).Sum(z => z.Total),
                    Fevereiro = y.Where(x => x.DataPag.Value.Month == 2).Sum(z => z.Total),
                    Marco = y.Where(x => x.DataPag.Value.Month == 3).Sum(z => z.Total),
                    Abril = y.Where(x => x.DataPag.Value.Month == 4).Sum(z => z.Total),
                    Maio = y.Where(x => x.DataPag.Value.Month == 5).Sum(z => z.Total),
                    Junho = y.Where(x => x.DataPag.Value.Month == 6).Sum(z => z.Total),
                    Julho = y.Where(x => x.DataPag.Value.Month == 7).Sum(z => z.Total),
                    Agosto = y.Where(x => x.DataPag.Value.Month == 8).Sum(z => z.Total),
                    Setembro = y.Where(x => x.DataPag.Value.Month == 9).Sum(z => z.Total),
                    Outubro = y.Where(x => x.DataPag.Value.Month == 10).Sum(z => z.Total),
                    Novembro = y.Where(x => x.DataPag.Value.Month == 11).Sum(z => z.Total),
                    Dezembro = y.Where(x => x.DataPag.Value.Month == 12).Sum(z => z.Total),
                    Total = y.Sum(x=> x.Total),
                    Conta = y.First().Conta
                });


                string Filtro = TipoDeFiltro == "PAGAMENTO" ? "Ordens Pagas" : "Ordens Lançadas";

                var Envio = ItensDoPlanoDeConta.Union(ItensDoCaixa).ToList();
                rd.Load(Path.Combine(Server.MapPath("~/Relatorio/RelatorioAnual.rpt")));
                rd.SetDataSource(Envio);
                rd.SetParameterValue("Usuario", User);
                rd.SetParameterValue("Periodo", PeriodoRelatorio);
                rd.SetParameterValue("Tipo", Filtro);
            }
            #endregion

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            str.Seek(0, SeekOrigin.Begin);
            string savedFilename = string.Format("Relatorio.pdf");
            return File(str, "application/pdf", savedFilename);
        }
        #endregion

        #region CARREGANDO INDEX
        public ActionResult Index()
        {
            // RECUPERANDO USUÁRIO PARA APRESENTAR NA VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;


            // FILTRANDO CAIXA DE ACORDO COM O USUÁRIO
            var financeiroSaida = db.Financeiro
                .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                .Include(f => f.ModoPagamento)
                .Include(f => f.Pessoa)
                .Include(f => f.PlanoDeConta)
                .Include(f => f.ContaCorrente);

            var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

            // NEGATIVANDO OS VALORES QUE SÃO SAÍDAS
            foreach (var item in lista)
            {
                var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                if (categ.CategoriaPlanoDeConta == "DÉBITO")
                {
                    item.TotalFinanceiro = item.TotalFinanceiro * -1;
                }
            }
            return View(lista);
        }
        #endregion

        #region FILTRO PARA BUSCA
        [HttpPost]
        public ActionResult Index(DateTime? DataDeFiltro, DateTime? DataAteFiltro, string CategoriaFiltro)
        {
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            if (DataDeFiltro == null && DataAteFiltro == null)
            {
                return RedirectToAction("Index");
            }
            switch (CategoriaFiltro)
            {
                #region CASO FOR FILTRO POR PAGAMENTO
                case "PAGAMENTO":
                    // CASO TENHA SOMENTE DATA "DE"
                    if (DataDeFiltro != null && DataAteFiltro == null)
                    {
                        var financeiroSaida = db.Financeiro
                            .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                            .Where(x => x.DataPagamento >= DataDeFiltro)
                            .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA SOMENTE DATA "ATE"
                    else if (DataDeFiltro == null && DataAteFiltro != null)
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataPagamento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA "DE" E "ATÉ"
                    else
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataPagamento >= DataDeFiltro && x.DataPagamento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                #endregion

                #region CASO FOR FILTRO POR LANÇAMENTO
                case "LANCAMENTO":
                    // CASO TENHA SOMENTE DATA "DE"
                    if (DataDeFiltro != null && DataAteFiltro == null)
                    {
                        var financeiroSaida = db.Financeiro
                            .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                            .Where(x => x.DataLancamento >= DataDeFiltro)
                            .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA SOMENTE DATA "ATE"
                    else if (DataDeFiltro == null && DataAteFiltro != null)
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataLancamento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA "DE" E "ATÉ"
                    else
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataLancamento >= DataDeFiltro && x.DataLancamento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                #endregion

                #region CASO FOR FILTRO POR VENCIMENTO
                case "VENCIMENTO":
                    // CASO TENHA SOMENTE DATA "DE"
                    if (DataDeFiltro != null && DataAteFiltro == null)
                    {
                        var financeiroSaida = db.Financeiro
                            .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                            .Where(x => x.DataVencimento >= DataDeFiltro)
                            .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA SOMENTE DATA "ATE"
                    else if (DataDeFiltro == null && DataAteFiltro != null)
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataVencimento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA "DE" E "ATÉ"
                    else
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataVencimento >= DataDeFiltro && x.DataVencimento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                #endregion

                #region CASO FOR FILTRO POR LANÇAMENTO/VENCIMENTO
                case "LANVENC":
                    // CASO TENHA SOMENTE DATA "DE"
                    if (DataDeFiltro != null && DataAteFiltro == null)
                    {
                        var financeiroSaida = db.Financeiro
                            .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                            .Where(x => x.DataLancamento >= DataDeFiltro)
                            .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA SOMENTE DATA "ATE"
                    else if (DataDeFiltro == null && DataAteFiltro != null)
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataVencimento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    // CASO TENHA "DE" E "ATÉ"
                    else
                    {
                        var financeiroSaida = db.Financeiro
                        .Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario)
                        .Where(x => x.DataLancamento >= DataDeFiltro && x.DataVencimento <= DataAteFiltro)
                        .Include(f => f.ModoPagamento).Include(f => f.Pessoa).Include(f => f.PlanoDeConta);

                        var lista = financeiroSaida.ToList().OrderByDescending(x => x.DataLancamento);

                        foreach (var item in lista)
                        {
                            var categ = db.PlanoDeConta.FirstOrDefault(x => x.IdPlanoDeConta == item.IdPlanoDeConta);
                            if (categ.CategoriaPlanoDeConta == "DÉBITO")
                            {
                                item.TotalFinanceiro = item.TotalFinanceiro * -1;
                            }
                        }
                        return View(lista);
                    }
                    #endregion
            }
            return View();
        }
        #endregion
    }
}