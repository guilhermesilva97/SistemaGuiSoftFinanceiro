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
    public class ContaCorrenteController : Controller
    {
        private ContextGS db = new ContextGS();

        #region .:: INDEX ::.
        public ActionResult Index()
        {
            // BUSCANDO USUÁRIO QUE ESTÁ LOGADO E RETORNANDO NA VIEW A LISTA
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            return View(db.ContaCorrente.ToList().Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario));
        }
        #endregion

        #region .:: CADASTRAR ::.

        #region --> METODO GET <--
        public ActionResult Create()
        {
            // CONSULTANDO SE EXISTE USUÁRIO LOGADO E PASSANDO PARA A VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            return View();
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        [ValidateAjax]
        public JsonResult Create(ContaCorrente contaCorrente)
        {
            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO E INSERINDO EM CONTA CORRENTE
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);

                // PASSANDO USUÁRIO, CRIANDO NOVO ID E ADICIONANDO INFORMAÇÕES
                contaCorrente.IdUsuario = Usuario.IdUsuario;
                contaCorrente.IdContaCorrente = Guid.NewGuid();
                contaCorrente.ATIVO = true;
                db.ContaCorrente.Add(contaCorrente);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }
        #endregion

        #endregion

        #region .:: EDITAR ::.

        #region --> METODO GET <--
        public ActionResult Edit(Guid? id)
        {
            // CONSULTANDO USUÁRIO QUE ESTÁ LOGADO E RETORNANDO PARA A VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            // RETORNANDO OBJETO COM O ID QUE FOI PASSADO
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContaCorrente contaCorrente = db.ContaCorrente.Find(id);
            if (contaCorrente == null)
            {
                return HttpNotFound();
            }
            return View(contaCorrente);
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        [ValidateAjax]
        public JsonResult Edit(ContaCorrente ContaCorrente)
        {
            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO E INSERINDO EM Plano De Conta
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);
                ContaCorrente.IdUsuario = Usuario.IdUsuario;

                // PASSANDO INFORMAÇÕES MODIFICADAS PARA O Plano De Conta
                db.Entry(ContaCorrente).State = EntityState.Modified;
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

            // COLETANDO INFORMAÇÕES DO USUÁRIO PELO ID
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContaCorrente contaCorrente = db.ContaCorrente.Find(id);
            if (contaCorrente == null)
            {
                return HttpNotFound();
            }
            return View(contaCorrente);
        }
        #endregion

        #region --> METODO POST <--

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            var objetoFinanceiro = db.Financeiro.FirstOrDefault(x => x.IdContaCorrente == id);


            if (objetoFinanceiro == null)
            {
                // BUSCANDO Plano De Conta PELO ID E CONFIRMANDO EXCLUSÃO
                ContaCorrente ContaCorrente = db.ContaCorrente.Find(id);
                db.ContaCorrente.Remove(ContaCorrente);
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
