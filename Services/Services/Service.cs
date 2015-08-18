using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Services
{
    class Service
    {
        private int id;
        private int type;
        private int prioridade;
        private int status;
        private DateTime hoje;
        private DateTime prazo;
        private string[] conteudo;
        private bool recebido;
        private Conexao conexao;


        public Service()
        {

        }

        public static Service New(int type,int prioridade,DateTime hoje,DateTime prazo,int status,string[] conteudo)
        {
            try
            {
                /*"`id` INT NOT NULL," +
            "`type` TINYINT NULL," +
            "`prioridade` TINYINT NULL," +
            "`hoje` DATETIME NULL," +
            "`prazo` DATETIME NULL," +
            "`status` TINYINT NULL," +
            "`conteudo` TEXT NULL," +
            "`recebido` TINYINT NULL," +
            "PRIMARY KEY (`id`))";*/
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    string joined = "";
                    if ("" == con.comandoMysql("INSERT INTO service values(0,"+type.ToString()+","+prioridade.ToString()+",'"+hoje.ToString("yyyy-MM-dd-HH-mm-ss")+"','"+prazo.ToString("yyyy-MM-dd-HH-mm-ss")+"',"+status.ToString()+",'"+joined +"',0)"))
                        return null;
                    else
                    {
                        return Load()
                    }
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static Service Load()
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {

                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
