using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControleEstoque.Web.Models
{
    public class UnidadeMedidaModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }
        [Required]
        [MaxLength(3, ErrorMessage = "O campo sigla deve possuir no máximo 3 caracteres.")]
        public string Sigla { get; set; }

        public bool Ativo { get; set; }

        public static List<UnidadeMedidaModel> Obter(int pagina, int tamPagina)
        {
            var retorno = new List<UnidadeMedidaModel>();
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        //USAR NO MSSQL 2012+  //"select * from unidade_medida order by nome offset {0} rows fetch next {1} rows only",
                        //USAR NO MSSQL 2012+  //pos > 0 ? pos - 1 : 0, tamPagina);
                        "WITH Linha AS  (SELECT *, ROW_NUMBER() OVER(ORDER BY nome) AS LinhaNumero " +
                            "FROM unidade_medida) SELECT * FROM Linha WHERE LinhaNumero BETWEEN {0} AND {1}",
                    pos > 0 ? pos + 1 : 0,
                    pos > 0 ? (tamPagina * pagina) : tamPagina);
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        retorno.Add(new Models.UnidadeMedidaModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Sigla = (string)reader["sigla"],
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
                    comando.CommandText = "select count (*) from unidade_medida";
                    retorno = (int)comando.ExecuteScalar();
                }
            }
            return retorno;
        }

        public static UnidadeMedidaModel ObterId(int id)
        {
            UnidadeMedidaModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from unidade_medida where (id = @id)";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        retorno = new Models.UnidadeMedidaModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Sigla = (string)reader["sigla"],
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
                        comando.CommandText = "delete from unidade_medida where (id = @id)";
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
                        comando.CommandText = "insert into unidade_medida (nome, sigla, ativo) values (@nome, @sigla, @ativo); select convert(int, scope_identity())";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@sigla", SqlDbType.VarChar).Value = this.Sigla;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        retorno = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "update unidade_medida set nome=@nome, sigla=@sigla, ativo=@ativo where id=@id";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@sigla", SqlDbType.VarChar).Value = this.Sigla;
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