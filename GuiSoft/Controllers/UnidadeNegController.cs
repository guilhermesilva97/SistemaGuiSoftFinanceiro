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
    public class UnidadeNegController : Controller
    {
        private ContextGS db = new ContextGS();

        #region .:: LISTANDO UNIDADE DE NEGÓCIO COM BASE NO USUÁRIO LOGADO ::.
        public ActionResult Index()
        {

            // CONSULTANDO QUAL USUÁRIO E PASSANDO PARA A VIEW
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            return View(db.UnidadeNeg.Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario));
        }
        #endregion

        #region .:: CADASTRAR ::.

        #region --> METODO GET <--
        public ActionResult Create()
        {
            // CONSULTANDO O USUÁRIO QUE ESTÁ LOGADO E PASSANDO PARA A VIEW
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
        public JsonResult Create(UnidadeNeg unidadeNeg)
        {
            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO E INSERINDO EM UNIDADE DE NEGÓCIO
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);

                // INSERINDO USUÁRIO, CRIANDO NOVO ID E ADICIONANDO INFORMAÇÕES
                unidadeNeg.IdUsuario = Usuario.IdUsuario;
                unidadeNeg.IdUnidade = Guid.NewGuid();
                db.UnidadeNeg.Add(unidadeNeg);
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
            UnidadeNeg unidadeNeg = db.UnidadeNeg.Find(id);
            if (unidadeNeg == null)
            {
                return HttpNotFound();
            }
            return View(unidadeNeg);
        }
        #endregion

        #region --> METODO POST<--
        [HttpPost]
        [ValidateAjax]
        public JsonResult Edit(UnidadeNeg unidadeNeg)
        {
            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO E PASSANDO PARA UNIDADE DE NEGÓCIO
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);
                unidadeNeg.IdUsuario = Usuario.IdUsuario;

                // SALVANDO OS DADOS MODIFICADOS
                db.Entry(unidadeNeg).State = EntityState.Modified;
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

        #region .:: DELETE ::.

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


            // CONSULTANDO SE EXISTE O ID QUE FOI PASSADO E RETORNANDO PARA A VIEW
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadeNeg unidadeNeg = db.UnidadeNeg.Find(id);
            if (unidadeNeg == null)
            {
                return HttpNotFound();
            }
            return View(unidadeNeg);
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            var objetoFinanceiro = db.FinanceiroDetalhes.FirstOrDefault(x => x.IdUnidade == id);

            if (objetoFinanceiro == null)
            {
                UnidadeNeg unidadeNeg = db.UnidadeNeg.Find(id);
                db.UnidadeNeg.Remove(unidadeNeg);
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
