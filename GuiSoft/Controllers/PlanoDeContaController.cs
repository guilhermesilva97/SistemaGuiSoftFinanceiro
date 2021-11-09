using Sistema.Data;
using Sistema.Dominio.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GuiSoft.Controllers
{
    public class PlanoDeContaController : Controller
    {
        private ContextGS db = new ContextGS();

        #region .:: LISTANDO O Plano De Conta DE ACORDO COM O USUÁRIO QUE ESTÁ LOGADO ::.

        public ActionResult Index()
        {
            // BUSCANDO USUÁRIO QUE ESTÁ LOGADO E RETORNANDO NA VIEW A LISTA
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            return View(db.PlanoDeConta.Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario));
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
        public JsonResult Create(PlanoDeConta PlanoDeConta)
        {
          try
          {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO E INSERINDO EM Plano De Conta
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);

                // PASSANDO USUÁRIO, CRIANDO NOVO ID E ADICIONANDO INFORMAÇÕES
                PlanoDeConta.IdUsuario = Usuario.IdUsuario;
                PlanoDeConta.IdPlanoDeConta = Guid.NewGuid();
                PlanoDeConta.ATIVO = true;
                db.PlanoDeConta.Add(PlanoDeConta);
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
            PlanoDeConta PlanoDeConta = db.PlanoDeConta.Find(id);
            if (PlanoDeConta == null)
            {
                return HttpNotFound();
            }
            return View(PlanoDeConta);
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        [ValidateAjax]
        public JsonResult Edit(PlanoDeConta PlanoDeConta)
        {
            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO E INSERINDO EM Plano De Conta
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);
                PlanoDeConta.IdUsuario = Usuario.IdUsuario;

                // PASSANDO INFORMAÇÕES MODIFICADAS PARA O Plano De Conta
                db.Entry(PlanoDeConta).State = EntityState.Modified;
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
            PlanoDeConta PlanoDeConta = db.PlanoDeConta.Find(id);
            if (PlanoDeConta == null)
            {
                return HttpNotFound();
            }

            return View(PlanoDeConta);
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            var objetoFinanceiro = db.Financeiro.FirstOrDefault(x => x.IdPlanoDeConta == id);

            if (objetoFinanceiro == null)
            {
                // BUSCANDO Plano De Conta PELO ID E CONFIRMANDO EXCLUSÃO
                PlanoDeConta PlanoDeConta = db.PlanoDeConta.Find(id);
                db.PlanoDeConta.Remove(PlanoDeConta);
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
