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

        public Setor(Conexao conexao)
        {
            con = conexao;
        }

        public string New()
        {
            try
            {
                if (con.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {
                        string query;
                        System.Data.DataTable dt = con.Query("select * from setor where nome='" + this.nome + "'");
                        if (dt == null)
                        {
                            query = "INSERT INTO setor (id,nome) VALUES(null,'" +
                                this.nome + "')";
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
