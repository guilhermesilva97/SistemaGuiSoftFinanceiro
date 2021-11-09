using Sistema.Data;
using Sistema.Dominio;
using Sistema.Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuiSoft.Controllers
{
    public class UsuarioController : Controller
    {
        ContextGS db = new ContextGS();
        // GET: Usuario

        #region .:: LOGIN ::.
        public ActionResult Login()
        {
            return View();
        }

        // AUTENTICANDO LOGIN E SENHA
        [HttpPost]
        public JsonResult Login(Usuario usuario)
        {
            var user = db.Usuario.FirstOrDefault(x=> x.LoginUsuario== usuario.LoginUsuario);
            
            if (user == null)
            {
                return Json(new { success = false });
            }

            else
            {
                if (user.Senha != usuario.Senha)
                {
                    return Json(new { success = false });
                }
                else
                {
                    Session["LoginUsuario"] = user.LoginUsuario;
                    return Json(new { success = true });
                }
            }
        }
        #endregion

        #region .:: LOGOFF .::
        
        public ActionResult Sair()
        {
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else {
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;
            var UsuarioSair = db.Usuario.FirstOrDefault(x => x.LoginUsuario== User);
            return View(UsuarioSair);
            }
        }

        [HttpPost]
        public JsonResult Logoff()
        {
            Session["LoginUsuario"] = null;
            return Json(new { success = true });
        }
        #endregion

        #region .:: REGISTRAR ::.
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult Registrar(Usuario usuario)
        {
            var consultarUsuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario== usuario.LoginUsuario);
            if (consultarUsuario == null) { 
            usuario.IdUsuario = Guid.NewGuid();
            db.Usuario.Add(usuario);
            db.SaveChanges();
            return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
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