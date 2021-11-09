using Sistema.Data;
using Sistema.Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuiSoft.Controllers
{
    public class LancamentoController : Controller
    {
        ContextGS db = new ContextGS();

        #region .:: CARREGANDO DROPDOWN SEM PARÂMETRO ::.

        public void CarregarPlanoDeConta()
        {
            // COLETANDO DADOS DO USUÁRIO QUE ESTÁ LOGADO
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);

            // CRIANDO O DROPDOWN DE ACORDO COM O USUÁRIO
            List<PlanoDeConta> data = db.PlanoDeConta.Where(x=> x.IdUsuario == Usuario.IdUsuario && x.ATIVO == true).ToList();
            SelectList lista = new SelectList(data, "IdPlanoDeConta", "DescricaoPlanoDeConta");
            ViewBag.ListaPlanoDeConta = lista;
        }

        public void CarregarModoPag()
        {
            // COLETANDO DADOS DO USUÁRIO QUE ESTÁ LOGADO   
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);

            // CRIANDO O DROPDOWN DE ACORDO COM O USUÁRIO
            List<ModoPagamento> listModoPagamento = db.ModoPagamento.Where(x => x.IdUsuario == Usuario.IdUsuario && x.ATIVO == true).ToList();
            SelectList lista = new SelectList(listModoPagamento, "IdPagamento", "DescricaoPagamento");
            ViewBag.ListaModoPagamento = lista;
        }

        public void CarregarCaixa()
        {
            // COLETANDO DADOS DO USUÁRIO QUE ESTÁ LOGADO   
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);

            // CRIANDO O DROPDOWN DE ACORDO COM O USUÁRIO
            List<ContaCorrente> listContaCorrente = db.ContaCorrente.Where(x => x.IdUsuario == Usuario.IdUsuario && x.ATIVO == true).ToList();
            SelectList lista = new SelectList(listContaCorrente, "IdContaCorrente", "BancoContaCorrente");
            ViewBag.ListaContaCorrente = lista;
        }

        #endregion

        #region .:: CARREGANDO DROPDOWN COM PARÂMETRO ::.

        public void CarregarModoPag(Guid id)
        {
            // COLETANDO DADOS DO USUÁRIO QUE ESTÁ LOGADO
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);

            // CRIANDO O DROPDOWN DE ACORDO COM O USUÁRIO
            List<ModoPagamento> listModoPagamento = db.ModoPagamento.Where(x => x.IdUsuario == Usuario.IdUsuario && x.ATIVO == true).OrderByDescending(x => x.IdPagamento == id).ToList();
            SelectList lista = new SelectList(listModoPagamento, "IdPagamento", "DescricaoPagamento");
            ViewBag.ListaModoPagamento = lista;
        }

        public void CarregarPlanoDeConta(Guid id)
        {
            // COLETANDO DADOS DO USUÁRIO QUE ESTÁ LOGADO
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);

            // CRIANDO O DROPDOWN DE ACORDO COM O USUÁRIO
            List<PlanoDeConta> data = db.PlanoDeConta.Where(x => x.IdUsuario == Usuario.IdUsuario && x.ATIVO == true).OrderByDescending(x => x.IdPlanoDeConta == id).ToList();
            SelectList lista = new SelectList(data, "IdPlanoDeConta", "DescricaoPlanoDeConta");
            ViewBag.ListaPlanoDeConta = lista;
        }

        public void CarregarCaixa(Guid id)
        {
            // COLETANDO DADOS DO USUÁRIO QUE ESTÁ LOGADO
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);

            // CRIANDO O DROPDOWN DE ACORDO COM O USUÁRIO
            List<ContaCorrente> data = db.ContaCorrente.Where(x => x.IdUsuario == Usuario.IdUsuario && x.ATIVO == true).OrderByDescending(x => x.IdContaCorrente == id).ToList();
            SelectList lista = new SelectList(data, "IdContaCorrente", "BancoContaCorrente");
            ViewBag.ListaContaCorrente = lista;
        }
        #endregion

        #region .:: NOVO LANÇAMENTO ::.

        #region --> METODO GET <--
        public ActionResult NovoLancamento()
        {
            // CONSULTANDO O USUÁRIO QUE ESTÁ LOGANDO E PASSANDO PARA A VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            CarregarModoPag();
            CarregarPlanoDeConta();
            CarregarCaixa();
            return View();
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        [ValidateAjax]
        public JsonResult Create(Financeiro financeiro, List<FinanceiroDetalhe> ListDetalhe)
        {
            try
            {
                // RECUPERANDO USUÁRIO QUE ESTÁ LOGADO E CRIANDO FINANCEIRO COM BASE NELE
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);
                financeiro.IdUsuario = Usuario.IdUsuario;
                financeiro.IdFinanceiro = Guid.NewGuid();
                db.Financeiro.Add(financeiro);
                db.SaveChanges();

                // CRIANDO LISTA DE UNIDADE DE NEGÓCIO
                foreach (var data in ListDetalhe)
                {
                    FinanceiroDetalhe objFinanceiro = new FinanceiroDetalhe(Guid.NewGuid(), financeiro.IdFinanceiro, data.IdUnidade, data.SubTotalFinanceiroDetalhe, financeiro.IdUsuario);
                    db.FinanceiroDetalhes.Add(objFinanceiro);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

        }
        #endregion

        #endregion

        #region .:: GET E POST PARA EDITAR LANÇAMENTO ::.

        #region --> METODO GET <--
        [HttpGet]
        public ActionResult EditarLancamento(Guid? id)
        {
            // CONSULTANDO SE EXISTE ALGUM USUÁRIO LOGADO E PASSANDO PARA A VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            // RECUPERANDO A LISTA DE UNIDADE DE NEGÓCIO
            var listaFinanceiro = db.FinanceiroDetalhes.Where(x => x.IdFinanceiro == id).Include(f => f.Financeiro).Include(f => f.Unidade).ToList();
            ViewBag.ListaFinanceiro = listaFinanceiro;

            // RECUPERANDO O DROPDOWN DE ACORDO COM O LANÇAMENTO
            var financeiro = db.Financeiro.FirstOrDefault(x => x.IdFinanceiro == id);
            CarregarModoPag(financeiro.IdPagamento);
            CarregarPlanoDeConta(financeiro.IdPlanoDeConta);

            //RECUPERANDO PESSOA
            var cliente = db.Pessoas.FirstOrDefault(x => x.IdPessoa == financeiro.IdPessoa);
            ViewBag.Cliente = cliente.NomePessoa;
            ViewBag.Identificacao = cliente.IdentificacaoPessoa;

            return View(financeiro);
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        [ValidateAjax]
        public JsonResult EditarLancamento (Financeiro financeiro, List<FinanceiroDetalhe> ListDetalhe)
        {

            try
            {
                // RECUPERA O USUÁRIO LOGADO
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);
                financeiro.IdUsuario = Usuario.IdUsuario;

                // FAZENDO A COMPARAÇÃO PARA SABER SE ALGUM ELEMENTO DO LANÇAMENTO FOI EXCLUIDO, E SE CASO FOI, ESTÁ REMOVENDO DO FINANCEIRO DETALHE
                List<FinanceiroDetalhe> ListaComparar = db.FinanceiroDetalhes.Where(x => x.IdFinanceiro == financeiro.IdFinanceiro).ToList();
                foreach (var item in ListaComparar)
                {
                    var Detalhe = ListDetalhe.FirstOrDefault(x => x.IdUnidade == item.IdUnidade && x.IdFinanceiro == item.IdFinanceiro);
                    if (Detalhe == null)
                    {
                        db.FinanceiroDetalhes.Remove(item);
                        db.SaveChanges();
                    }
                }


                // FAZENDO A COMPARAÇÃO PARA SABER SE ALGUM ELEMENTO TEVE O SEU VALOR MODIFICADO, E CASO TENHA SIDO, ELE FAZ A ALTERAÇÃO
                // OU CASO FOR UM ELEMENTO NOVO, ELE INSERE NA NOVA LISTA DE UNIDADE DE NEGÓCIO.
                foreach (var item in ListDetalhe)
                {
                    var Comparar = ListaComparar.FirstOrDefault(x => x.IdUnidade == item.IdUnidade && x.IdFinanceiro == item.IdFinanceiro);
                    if (Comparar == null)
                    {
                        FinanceiroDetalhe objFinanceiro = new FinanceiroDetalhe(Guid.NewGuid(), financeiro.IdFinanceiro, item.IdUnidade, item.SubTotalFinanceiroDetalhe, Usuario.IdUsuario);
                        db.FinanceiroDetalhes.Add(objFinanceiro);
                        db.SaveChanges();
                    }
                    else
                    {
                        Comparar.SubTotalFinanceiroDetalhe = item.SubTotalFinanceiroDetalhe;
                    }
                }



                // SALVANDO AS MODIFICAÇÕES GERAIS DA ORDEM
                db.Entry(financeiro).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }

        }
        #endregion

        #endregion

        #region .:: VIEW PARA OBTER PESSOA E FILTRO PARA PROCURAR PESSOA ::.

        #region --> METODO GET <--
        [HttpGet]
        public ActionResult ObterPessoa()
        {
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

        // OBTENDO LISTA DE PESSOA DE ACORDO COM O USUÁRIO LOGADO
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            List<Pessoa> lista = db.Pessoas.Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario== ConsultarUsuario).IdUsuario).ToList();

            return View(lista);
        }
        #endregion

        #region --> METODO POST (PARA FILTRAR) <--
        [HttpPost]
        public ActionResult ObterPessoa(string txtnome)
        {
            if (txtnome == "")
            {
                List<Pessoa> lista = db.Pessoas.ToList();
                return View(lista);
            }

            //FILTRANDO LISTA DE PESSOAS COM OS PARÂMETROS QUE FORAM PASSADOS
            //List<Pessoa> pessoa = db.Pessoas.Where(x => x.NomePessoa == txtnome).ToList();
            List<Pessoa> pessoa = db.Pessoas.Where(x => x.NomePessoa.Contains(txtnome)).ToList();
            return View(pessoa);
        }
        #endregion

        #endregion

        #region .:: VIEW PARA OBTER UNIDADE E FILTRO PARA PROCURAR UNIDADE ::.

        [HttpGet]
        public ActionResult ObterUnidade()
        {
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var ConsultarUsuario = Session["LoginUsuario"].ToString();

            List<UnidadeNeg> lista = db.UnidadeNeg.Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario== ConsultarUsuario).IdUsuario).ToList();

            return View(lista);
        }

        [HttpPost]
        public ActionResult ObterUnidade(string txtnome)
        {
            if (txtnome == "")
            {
                List<UnidadeNeg> lista = db.UnidadeNeg.ToList();
                return View(lista);
            }

            //FILTRANDO LISTA DE PESSOAS COM OS PARÂMETROS QUE FORAM PASSADOS
            List<UnidadeNeg> unidade = db.UnidadeNeg.Where(x => x.NomeUnidadeNeg.Contains(txtnome)).ToList();
            return View(unidade);
        }

        #endregion

        #region .:: METODO DELETE
        [HttpPost]
        public JsonResult Delete (Guid id)
        {
            // REMOVENDO TODOS OS ITENS DO FINANCEIRO
            var lista = db.FinanceiroDetalhes.Where(x => x.IdFinanceiro == id).ToList();
            foreach(var item in lista)
            {
                db.FinanceiroDetalhes.Remove(item);
            }

            // REMOVENDO O FINANCEIRO
            var Finan = db.Financeiro.Find(id);
            db.Financeiro.Remove(Finan);
            db.SaveChanges();
            return Json(new { success = true });
        }
        #endregion

    }


}