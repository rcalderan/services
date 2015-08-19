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
        private Status status;
        private DateTime hoje;
        private DateTime prazo;
        private string[] conteudo;
        private bool recebido;
        private Conexao conexao;


        public Service()
        {

        }

        public static string[] Split(string str, string chars)
        {
            List<string> splited = new List<string>();
            int i = str.IndexOf(chars);
            string piece = str;
            while (i != -1)
            {
                piece = str.Substring(0, i);
                splited.Add(piece);
                if (str.Length > 3)
                {
                    str = str.Substring(i + chars.Length, str.Length - (piece.Length + chars.Length));
                    i = str.IndexOf(chars);
                    if (i == -1)
                        splited.Add(str);
                }
                else
                    i = -1;
            }
            return splited.ToArray();
        }
        public static string Join(string[] array, string chars)
        {
            string res = "";
            if (array.Length > 0)
            {
                res = array[0];
                for (int i = 1; i < array.Length; i++)
                    res += chars + array[i];
            }
            return res;
        }

        public static Service New(int type,int prioridade,DateTime hoje,DateTime prazo,Status status,string[] conteudo)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    string joined = Join(conteudo,"<*>");
                    int nextId = 0;
                    DataTable dt = con.Query("Select id from service order by id limit 1");
                    if (dt != null)
                        if (int.TryParse(dt.Rows[0]["id"].ToString(), out nextId))
                            nextId++;
                    if ("" != con.Comando("INSERT INTO service values("+nextId+","+type.ToString()+","+prioridade.ToString()+",'"+hoje.ToString("yyyy-MM-dd-HH-mm-ss")+"','"+prazo.ToString("yyyy-MM-dd-HH-mm-ss")+"',"+status.Id.ToString()+",'"+joined +"',0)"))
                        return null;
                    else
                    {
                        return Load(nextId);
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

        public static Service Load(int id)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    DataTable dt = con.Query("Select * from service where id=" + id.ToString());
                    if (dt != null)
                    {
                        DateTime auxDt;
                        int aux;
                        Service s = new Service();
                        s.id = int.Parse(dt.Rows[0]["id"].ToString());
                        s.type = int.Parse(dt.Rows[0]["type"].ToString());
                        if (DateTime.TryParse(dt.Rows[0]["hoje"].ToString(),out auxDt))
                            s.hoje = auxDt;
                        if (DateTime.TryParse(dt.Rows[0]["prazo"].ToString(),out auxDt))
                            s.prazo = auxDt;
                        s.conteudo = Split(dt.Rows[0]["conteudo"].ToString(), "<*>");
                        s.prioridade = int.Parse(dt.Rows[0]["prioridade"].ToString());
                        s.status = new Status(int.Parse(dt.Rows[0]["status"].ToString()));
                        s.conexao = con;
                        if (dt.Rows[0]["recebido"].ToString() == "1")
                            s.recebido = true;
                        else
                            s.recebido = false;
                        return s;
                    }
                    else
                        return null;

                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public bool Save()
        {
            try
            {
                if (conexao.getLastLoadState())
                {
                    string query = "UPDATE service SET type=" + this.type.ToString() +
                        ",hoje='" + this.hoje.ToString("yyyy-MM-DD HH:mm:ss") +
                        "',prazo='" + this.prazo.ToString("yyyy-MM-DD HH:mm:ss") +
                        "',conteudo='" + Join(this.conteudo, "<*>") +
                        "',prioridade=" + this.prioridade.ToString() +
                        ",status=" + status.Id.ToString();
                    if (this.recebido)
                        query += ",recebido=1";
                    else
                        query += ",recebido=0";
                    query += " WHERE id=" + this.id.ToString();

                    if ("" != conexao.Comando(query))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Service NewOne)
        {
            try
            {
                if (conexao.getLastLoadState())
                {
                    string query = "UPDATE service SET id=" + this.id;
                    if (this.type != NewOne.type)
                        query += ",type=" + NewOne.type.ToString();
                    if (this.hoje != NewOne.hoje)
                        query += ",hoje='" + NewOne.hoje.ToString("yyyy-MM-DD HH:mm:ss") + "'";
                    if (this.prazo != NewOne.prazo)
                        query += ",prazo='" + NewOne.prazo.ToString("yyyy-MM-DD HH:mm:ss") + "'";
                    if (this.conteudo != NewOne.conteudo)
                        query += ",conteudo='" + Join(NewOne.conteudo, "<*>")+"'";
                    if (this.prioridade != NewOne.prioridade)
                        query += ",prioridade=" + NewOne.prioridade.ToString();

                    if (this.status.Id != NewOne.status.Id)
                        query += ",status=" + NewOne.status.Id.ToString();

                    if (this.recebido!=NewOne.recebido)
                        if (NewOne.recebido)
                            query += ",recebido=1";
                        else
                            query += ",recebido=0";
                    query += " WHERE id=" + this.id.ToString();

                    if ("" != conexao.Comando(query))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }

    struct Status
    {
        private int id;
        private string nome;

        public string Nome
        {
            get{return nome;}
        }
        public int Id
        {
            get { return id; }
        }

        public Status(int Id)
        {
            switch (Id)
            {
                case 0:
                    id = Pendente;
                    nome = "Pendente";
                    break;
                case 1:
                    id = Completo;
                    nome = "Completo";
                    break;
                case 2:
                    id = EmTransito;
                    nome = "EmTransito";
                    break;
                case 3:
                    id = Negado;
                    nome = "Negado";
                    break;
                case 4:
                    id = Resolvido;
                    nome = "Resolvido";
                    break;
                default:
                    id = NA;
                    nome = "N/A";
                    break;
            }

        }
        public static int Desconhecido
        {
            get { return -1; }
        }
        public static int Pendente
        {
            get { return 0; }
        }
        public static int Completo
        {
            get { return 1; }
        }
        public static int EmTransito
        {
            get { return 2; }
        }
        public static int Negado
        {
            get { return 3; }
        }
        public static int Resolvido
        {
            get { return 4; }
        }
        public static int NA
        {
            get { return -1; }
        }

    }
}
