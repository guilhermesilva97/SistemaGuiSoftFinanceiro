using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sistema.Data;
using Sistema.Dominio.Entities;

namespace GuiSoft.Controllers
{
    public class ModoPagamentoController : Controller
    {
        private ContextGS db = new ContextGS();

        #region .:: LISTANDO MODO DE PAGAMENTO DE ACORDO COM O USUÁRIO LOGADO ::.

        public ActionResult Index()
        {
            // CONSULTANDO SE EXISTE ALGUM USUÁRIO LOGADO
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            // RETORNANDO USUÁRIO PARA A VIEW
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            // RETORNANDO A LISTA DE MODO DE PAGAMENTO
            var Usuario = db.Usuario.FirstOrDefault(y => y.LoginUsuario== User);
            return View(db.ModoPagamento.Where(x => x.IdUsuario == Usuario.IdUsuario));
        }

        #endregion

        #region .:: CADASTRANDO MODO DE PAGAMENTO ::.

        #region --> METODO GET <--
        public ActionResult Create()
        {
            // CONSULTANDO SE EXISTE USUÁRIO LOGADO
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            // RETORNANDO PARA A VIEW O USUÁRIO LOGADO
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;
            return View();
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        [ValidateAjax]
        public JsonResult Create(ModoPagamento modoPagamento)
        {
            try 
            {
            // OBTENDO USUÁRIO E INSERINDO NO MODO DE PAGAMENTO
            var ConsultarUsuario = Session["LoginUsuario"].ToString();
            var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);
            modoPagamento.IdUsuario = Usuario.IdUsuario;
            modoPagamento.ATIVO = true;

            // GERANDO NOVO ID E INSERINDO INFORMAÇÕES
                modoPagamento.IdPagamento = Guid.NewGuid();
                db.ModoPagamento.Add(modoPagamento);
                db.SaveChanges();
                return Json(new { success = true });
            }

            catch (Exception)
            {
                return Json(new { success = false });
            }
        }
        #endregion

        #endregion

        #region .:: EDITAR ::.

        #region --> METODO GET <--
        public ActionResult Edit(Guid? id)
        {
            // COLETANDO INFORMAÇÕES DO USUÁRIO QUE ESTÁ CONECTANDO E RETORNANDO PARA A VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            // VALIDANDO ID E RETORNANDO OBJETO PARA A VIEW
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModoPagamento modoPag = db.ModoPagamento.Find(id);
            if (modoPag == null)
            {
                return HttpNotFound();
            }
            return View(modoPag);
        }
        #endregion

        #region --> METODO POST<--
        [HttpPost]
        [ValidateAjax]
        public JsonResult Edit(ModoPagamento modoPag)
        {
            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO E PASSANDO PARA UNIDADE DE NEGÓCIO
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== ConsultarUsuario);
                modoPag.IdUsuario = Usuario.IdUsuario;

                // SALVANDO OS DADOS MODIFICADOS
                db.Entry(modoPag).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }
        #endregion

        #endregion

        #region .:: REMOVER ::. 

        #region --> METODO GET <--
        public ActionResult Delete(Guid? id)
        {
            // CONSULTANDO SE EXISTE UM USUÁRIO LOGADO E RETORNANDO PARA A VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            // CONSULTANDO SE O ID É VÁLIDO E RETORNANDO OBJETO PARA A VIEW
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModoPagamento modoPagamento = db.ModoPagamento.Find(id);
            if (modoPagamento == null)
            {
                return HttpNotFound();
            }
            return View(modoPagamento);
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            var objetoFinanceiro = db.Financeiro.FirstOrDefault(x => x.IdPagamento == id);

            if (objetoFinanceiro == null) { 
            // EXECUTANDO A EXCLUSÃO
            ModoPagamento modoPagamento = db.ModoPagamento.Find(id);
            db.ModoPagamento.Remove(modoPagamento);
            db.SaveChanges();
            return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        #endregion

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
