using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    class Setor
    {
        private Conexao con;

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string nome;
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }
        public Setor(int id)
        {
            try
            {
                Conexao conexao = new Conexao();
                if (conexao.getLastLoadState())
                {
                    System.Data.DataTable dt = conexao.Query("select * from setor where id="+id.ToString());
                    if (dt!=null)
                    {
                        this.con = conexao;
                        this.id = int.Parse(dt.Rows[0]["id"].ToString());
                        this.nome = dt.Rows[0]["nome"].ToString();
                    }
                }

            }
            catch
            {
            }
        }
        
        public static Setor Load(int id)
        {
            try
            {
                Setor s = new Setor(id);
                if (s.id == null)
                    return null;
                else
                    return s;
            }
            catch
            {
                return null;
            }

        }

        public static string New(string nome)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {
                        string query;
                        System.Data.DataTable dt = con.Query("select * from setor where nome='" + nome + "'");
                        if (dt == null)
                        {
                            query = "INSERT INTO setor (id,nome) VALUES(null,'" +
                                nome + "')";
                            if (con.Comando(query) != "")
                                return "Não foi possível adicionar novo setor";
                        }
                        else
                            return "Já existe um setor com este nome.";


                    }
                    else
                    {// mysql

                    }
                    return "";
                }
                else
                    return "Não foi possível adicionar novo setor (erro de conexão)";
            }
            catch 
            {
                return "Não foi possível adicionar novo setor";
            }
        }
    }
}
