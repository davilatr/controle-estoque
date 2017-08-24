using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleEstoque.Web.Models
{
    public class GrupoProdutoModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public static List<GrupoProdutoModel> Obter(int pagina, int tamPagina)
        {
            var retorno = new List<GrupoProdutoModel>();
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        //USAR NO MSSQL 2012+  //"select * from grupo_produto order by nome offset {0} rows fetch next {1} rows only",
                        //USAR NO MSSQL 2012+  //pos > 0 ? pos - 1 : 0, tamPagina);
                        "WITH Linha AS  (SELECT *, ROW_NUMBER() OVER(ORDER BY nome) AS LinhaNumero " +
                            "FROM grupo_produto) SELECT * FROM Linha WHERE LinhaNumero BETWEEN {0} AND {1}",
                    pos > 0 ? pos+1 : 0,
                    pos > 0 ? (tamPagina * pagina) : tamPagina);
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        retorno.Add(new Models.GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }
            return retorno;
        }

        public static int ObterQuantidade()
        {
            var retorno = 0;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    comando.CommandText = "select count (*) from grupo_produto";
                    retorno = (int)comando.ExecuteScalar();
                }
            }
            return retorno;
        }

        public static GrupoProdutoModel ObterId(int id)
        {
            GrupoProdutoModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from grupo_produto where (id = @id)";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        retorno = new Models.GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        };
                    }
                }
            }
            return retorno;
        }

        public static bool ExcluirId(int id)
        {
            var retorno = false;
            if (ObterId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();
                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "delete from grupo_produto where (id = @id)";
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        retorno = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }
            return retorno;
        }


        public int Salvar()
        {
            var retorno = 0;
            var model = ObterId(this.Id);
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    if (model == null)
                    {
                        comando.CommandText = "insert into grupo_produto (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        retorno = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "update grupo_produto set nome=@nome, ativo=@ativo where id=@id";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            retorno = this.Id;
                        }
                    }
                }
            }
            return retorno;
        }

    }
}