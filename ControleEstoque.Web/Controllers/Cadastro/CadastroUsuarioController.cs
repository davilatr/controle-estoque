using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class CadastroUsuarioController : Controller
    {
       
        //Senha padrão para alteração de usuário sem senha
        //Permitir a regra de negocio
        private const string _senhaPadrao = "{$127;$188}";
        
        private const int _maxLinhasPorPagina = 6;

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.SenhaPadrao = _senhaPadrao;
            
            ViewBag.ListaTamPag = new SelectList(new int[] { _maxLinhasPorPagina, 10, 15, 20, 30 }, _maxLinhasPorPagina);
            ViewBag.MaxLinhasPorPagina = _maxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = UsuarioModel.Obter(ViewBag.PaginaAtual, _maxLinhasPorPagina);
            var quantidade = UsuarioModel.ObterQuantidade();


            ViewBag.QuantPaginas = (quantidade / ViewBag.MaxLinhasPorPagina);
            if ((quantidade % ViewBag.MaxLinhasPorPagina) > 0)
                ViewBag.QuantPaginas += 1;

            return View(lista);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ObterUsuarioId(int id)
        {
            return Json(UsuarioModel.ObterId(id));
        }

        [Authorize]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ObterUsuarioGrid(int pagina, int tamPag)
        {
            var lista = UsuarioModel.Obter(pagina, tamPag);

            return Json(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUsuarioId(int id)
        {
            return Json(UsuarioModel.ExcluirId(id));
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult SalvarUsuario(UsuarioModel model)
        {
            var resultado = "Ok";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            if (!ModelState.IsValid)
            {
                resultado = "Aviso";
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                    //Alteração de usuário sem setar senha
                    if (model.Senha == _senhaPadrao)
                        model.Senha = "";

                    var id = model.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                    }
                    else
                    {
                        resultado = "Erro";
                    }
                }
                catch (Exception ex)
                {
                    resultado = "Erro";
                }
            }

            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
    }
}