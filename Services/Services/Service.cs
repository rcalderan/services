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

    struct TipoDeOrdem
    {
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

        public TipoDeOrdem(int ID)
        {
            this.id = ID;
            switch(ID)
            {
                case 0:
                    this.nome = "Descritiva";
                    break;
                case 1:
                    this.nome = "Manutenção de Equipamento";
                    break;
                default:
                    this.id = 0;
                    this.nome = "Descritiva";
                    break;
            }
        }
        public static TipoDeOrdem Descritiva
        {
            get
            {
                TipoDeOrdem p = new TipoDeOrdem();
                p.id = 0;
                p.nome = "Descritiva";
                return p;
            }
        }

        public static TipoDeOrdem Manutencao
        {
            get
            {
                TipoDeOrdem p = new TipoDeOrdem();
                p.id = 1;
                p.nome = "Manutenção de Equipamento";
                return p;
            }
        }
        public static List<TipoDeOrdem> All = new List<TipoDeOrdem>() 
        {
            TipoDeOrdem.Descritiva, TipoDeOrdem.Manutencao 
        };
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
        
        public Status(int ID)
        {
            this.id = ID;
            switch (ID)
            {
                case -1:
                    nome = "Desconhecido";
                    break;
                case 0:
                    nome = "Pendente";
                    break;
                case 1:
                    nome = "Completo";
                    break;
                case 2:
                    nome = "EmTransito";
                    break;
                case 3:
                    nome = "Negado";
                    break;
                case 4:
                    nome = "Resolvido";
                    break;
                case 5:
                    nome = "Aguardando Terceiros";
                    break;
                default:
                    id = -1;
                    nome = "Desconhecido";
                    break;
            }

        }

        public static List<Status> All = new List<Status>() 
        {
            Status.Pendente , 
            Status.Completo,
            Status.EmTransito,
            Status.Negado,
            Status.Resolvido,
            Status.AguardandoTerceiros,
            Status.Desconhecido
        };
        public static Status Desconhecido
        {
            get 
            {
                Status s = new Status();
                s.id = -1;
                s.nome = "Desconhecido";
                return s; 
            }
        }
        public static Status Pendente
        {
            get
            {
                Status s = new Status();
                s.id = 0;
                s.nome = "Pendente";
                return s;
            }
        }
        public static Status Completo
        {
            get
            {
                Status s = new Status();
                s.id = 1;
                s.nome = "Completo";
                return s;
            }
        }
        public static Status EmTransito
        {
            get
            {
                Status s = new Status();
                s.id = 2;
                s.nome = "EmTransito";
                return s;
            }
        }
        public static Status Negado
        {
            get
            {
                Status s = new Status();
                s.id = 3;
                s.nome = "Negado";
                return s;
            }
        }
        public static Status Resolvido
        {
            get
            {
                Status s = new Status();
                s.id = 4;
                s.nome = "Resolvido";
                return s;
            }
        }
        public static Status AguardandoTerceiros
        {
            get
            {
                Status s = new Status();
                s.id = 5;
                s.nome = "Aguardando Terceiros";
                return s;
            }
        }
        
    }
}
