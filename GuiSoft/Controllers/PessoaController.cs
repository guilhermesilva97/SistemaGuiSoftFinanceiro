using Sistema.Data;
using Sistema.Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GuiSoft.Controllers
{
    public class PessoaController : Controller
    {
        ContextGS db = new ContextGS();

        #region .:: LISTANDO PESSOAS DE ACORDO COM O USUÁRIO QUE ESTÁ LOGADO ::.

        public ActionResult Index()
        {

            // CASO NÃO ESTIVER USUÁRIO LOGADO, REDIRECIONA PARA A HOME
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            // RECUPERANDO USUÁRIO PARA MOSTRAR NA VIEW
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            return View(db.Pessoas.Where(x => x.IdUsuario == db.Usuario.FirstOrDefault(y => y.LoginUsuario == User).IdUsuario));
        }

        #endregion

        #region .:: CRIAR NOVA PESSOA ::.

        #region --> METODO GET <-- 
        public ActionResult Create()
        {
            // CASO NÃO ESTIVER USUÁRIO LOGADO, REDIRECIONA PARA A HOME
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            // RECUPERANDO USUÁRIO PARA MOSTRAR NA VIEW
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            return View();
        }
        #endregion

        #region --> METODO POST <-- 
        [HttpPost]
        [ValidateAjax]
        public JsonResult Create(Pessoa pessoa)
        {
            // CONSULTANDO SE JA EXISTE UMA PESSOA COM O MESMO CPF OU COM O MESMO CNPJ
            var ConsultarPessoa = db.Pessoas.FirstOrDefault(x=> x.IdentificacaoPessoa == pessoa.IdentificacaoPessoa);
            if (ConsultarPessoa != null)
            {
                return Json(new { success = false });
            }

            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);

                // CRIANDO NOVO ID E INSERINDO ID DO USUÁRIO NO CADASTRO
                pessoa.IdUsuario = Usuario.IdUsuario;
                pessoa.IdPessoa = Guid.NewGuid();

                // INSERINDO OS DADOS
                db.Pessoas.Add(pessoa);
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

        #region .:: EDITAR PESSOAS ::.

        #region --> METODO GET <--
        public ActionResult Edit(Guid? id)
        {

            // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            // RETORNANDO PARA VIEW O USUÁRIO QUE ESTÁ LOGADO
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Pessoa pessoa = db.Pessoas.Find(id);

            if (pessoa == null)
            {
                return HttpNotFound();
            }

            return View(pessoa);
        }
        #endregion

        #region --> METODO POST <--

        [HttpPost]
        [ValidateAjax]
        public JsonResult Edit(Pessoa pessoa)
        {
            try
            {
                // CONSULTAR O USUÁRIO QUE ESTÁ LOGADO
                var ConsultarUsuario = Session["LoginUsuario"].ToString();
                var Usuario = db.Usuario.FirstOrDefault(x => x.LoginUsuario == ConsultarUsuario);
                pessoa.IdUsuario = Usuario.IdUsuario;

                // ALTERANDO OS VALORES DA TABELA PESSOA
                pessoa.IdUsuario = Usuario.IdUsuario;
                db.Entry(pessoa).State = EntityState.Modified;
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

        #region .:: DELETANDO PESSOA ::.

        #region --> METODO GET <--
        public ActionResult Delete(Guid? id)
        {
            // CONSULTANDO SE EXISTE UM USUÁRIO LOGADO
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            // PASSANDO USUÁRIO LOGADO PARA A VIEW
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            // CONSULTANDO O ID E EXCLUINDO
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pessoa pessoa = db.Pessoas.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
        }
        #endregion

        #region --> METODO POST <--
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            var objetoFinanceiro = db.Financeiro.FirstOrDefault(x => x.IdPessoa == id);

            if (objetoFinanceiro == null)
            {
                // CONFIRMANDO A EXCLUSÃO
                Pessoa pessoa = db.Pessoas.Find(id);
                db.Pessoas.Remove(pessoa);
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

        #region .:: DETALHES PESSOAS ::.

        public ActionResult Details(Guid? id)
        {
            // CASO NÃO ESTIVER USUÁRIO LOGADO, REDIRECIONA PARA A HOME
            if (Session["LoginUsuario"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            // RECUPERANDO USUÁRIO QUE ESTÁ LOGADO PARA A VIEW
            var User = Session["LoginUsuario"].ToString();
            ViewBag.UsuarioConectado = User;

            // CONSULTANDO SE FOI PASSADO UM ID DE PESSOA
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // RETORNANDO PESSOA
            Pessoa pessoa = db.Pessoas.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
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