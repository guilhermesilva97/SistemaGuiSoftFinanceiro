using Sistema.Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuiSoft.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

           return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}