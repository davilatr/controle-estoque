using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class CadastroUnidadeMedidaController : Controller
    {
        private const int _maxLinhasPorPagina = 6;


        [Authorize]
        // public ActionResult ObterUnidadeMedida()
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _maxLinhasPorPagina, 10, 15, 20, 30 }, _maxLinhasPorPagina);
            ViewBag.MaxLinhasPorPagina = _maxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = UnidadeMedidaModel.Obter(ViewBag.PaginaAtual, _maxLinhasPorPagina);
            var quantidade = UnidadeMedidaModel.ObterQuantidade();


            ViewBag.QuantPaginas = (quantidade / ViewBag.MaxLinhasPorPagina);
            if ((quantidade % ViewBag.MaxLinhasPorPagina) > 0)
                ViewBag.QuantPaginas += 1;

            return View(lista);
        }

        [Authorize]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ObterUnidadeMedidaGrid(int pagina, int tamPag)
        {
            var lista = UnidadeMedidaModel.Obter(pagina, tamPag);

            return Json(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ObterUnidadeMedidaId(int id)
        {
            return Json(UnidadeMedidaModel.ObterId(id));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirUnidadeMedidaId(int id)
        {
            return Json(UnidadeMedidaModel.ExcluirId(id));
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarUnidadeMedida(UnidadeMedidaModel model)
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