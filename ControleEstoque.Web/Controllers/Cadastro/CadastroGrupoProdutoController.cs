using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente, Administrativo, Operador")]
    public class CadastroGrupoProdutoController : Controller
    {
        private const int _maxLinhasPorPagina = 6;


        
        // public ActionResult ObterGrupoProduto()
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _maxLinhasPorPagina, 10, 15, 20, 30 }, _maxLinhasPorPagina);
            ViewBag.MaxLinhasPorPagina = _maxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = GrupoProdutoModel.Obter(ViewBag.PaginaAtual, _maxLinhasPorPagina);
            var quantidade = GrupoProdutoModel.ObterQuantidade();


            ViewBag.QuantPaginas = (quantidade / ViewBag.MaxLinhasPorPagina);
            if ((quantidade % ViewBag.MaxLinhasPorPagina) > 0)
                ViewBag.QuantPaginas += 1;

            return View(lista);
        }

        
        
        public JsonResult ObterGrupoProdutoGrid(int pagina, int tamPag)
        {
            var lista = GrupoProdutoModel.Obter(pagina, tamPag);

            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ObterGrupoProdutoId(int id)
        {
            return Json(GrupoProdutoModel.ObterId(id));
        }

        [HttpPost]
        [Authorize(Roles = "Gerente")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirGrupoProdutoId(int id)
        {
            return Json(GrupoProdutoModel.ExcluirId(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarGrupoProduto(GrupoProdutoModel model)
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