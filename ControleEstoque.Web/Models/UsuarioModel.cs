using ControleEstoque.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {

        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required]
        public string Nome { get; set; }


        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel retorno = null;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from usuario where login=@login and senha=@senha";

                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        retorno = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Senha = (string)reader["senha"],
                            Nome = (string)reader["nome"]
                        };

                    }
                }
            }
            return retorno;
        }


        public static List<UsuarioModel> Obter(int pagina, int tamPagina)
        {
            var retorno = new List<UsuarioModel>();
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
                            "FROM usuario) SELECT * FROM Linha WHERE LinhaNumero BETWEEN {0} AND {1}",
                    pos > 0 ? pos + 1 : 0,
                    pos > 0 ? (tamPagina * pagina) : tamPagina);
                    
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        retorno.Add(new Models.UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
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
                    comando.CommandText = "select count (*) from usuario";
                    retorno = (int)comando.ExecuteScalar();
                }
            }
            return retorno;
        }
        public static UsuarioModel ObterId(int id)
        {
            UsuarioModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from usuario where (id = @id)";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        retorno = new Models.UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
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
                        comando.CommandText = "delete from usuario where (id = @id)";
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
                        comando.CommandText = "insert into usuario (nome, login, senha, id_perfil) values (@nome, @login, @senha, @id_perfil); select convert(int, scope_identity())";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;
                        comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha);
                        comando.Parameters.Add("@id_perfil", SqlDbType.Int).Value = 1;

                        retorno = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText =
                            "update usuario set nome=@nome, login=@login" +
                            (!string.IsNullOrEmpty(this.Senha) ? ", senha=@senha" : "") +
                            " where id=@id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;

                        if (!string.IsNullOrEmpty(this.Senha))
                            comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha);

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