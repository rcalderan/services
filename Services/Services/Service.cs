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
        
        public int getId()
        {
            return id;
        }
        public void setId(int ID)
        {
            this.id =ID;
        }

        private int type;
        public int getType()
        {
            return type;
        }

        private Prioridade prioridade;
        public Prioridade getPrioridade()
        {
            return prioridade;
        }
        private DateTime hoje;
        public DateTime getHoje()
        {
            return hoje;
        }

        private DateTime prazo;
        public DateTime getPrazo()
        {
            return prazo;
        }

        private Status status;
        public Status getStatus()
        {
            return status;
        }

        private string[] declarado;
        public string[] getDeclarado()
        {
            return declarado;
        }

        private string[] encontrado;
        public string[] getEncontrado()
        {
            return encontrado;
        }

        private string[] solucao;
        public string[] getSolucao()
        {
            return solucao;
        }

        private int usuSol;
        public int getUsuSol()
        {
            return usuSol;
        }

        private int setorSol;
        public int getSetorSol()
        {
            return setorSol;
        }

        private int usuResp;
        public int getUsuResp()
        {
            return usuResp;
        }
        private int setorResp;
        public int getSetorResp()
        {
            return setorResp;
        }

        private Conexao conexao;

        private System.Windows.Forms.ListViewItem[] items;
        public System.Windows.Forms.ListViewItem[] getItems()
        {
            return items;
        }

        private System.Windows.Forms.ListViewItem[] orcamento;
        public System.Windows.Forms.ListViewItem[] getOrcamento()
        {
            return orcamento;
        }


        /*
         * id
         * type
         * prioridade
         * hoje
         * prazo
         * status
         * declarado
         * encontrado
         * solucao
         * usuSol
         * setorSol
         * usuResp
         * setorResp
         */
        public Service()
        {

        }

        public static string[] Split(string str, string chars)
        {
            List<string> splited = new List<string>();
            int i = str.IndexOf(chars);
            string piece = str;
            if (str.Length > 0)
            {
                if (i==-1)
                    splited.Add(str);
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
            else 
                return new string[0];
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

        public static string SerializeItens(System.Windows.Forms.ListViewItem[] items)
        {
            try
            {
                if (items.Length > 0)
                {
                    string serialized = "",scaped;
                    for (int i = 0; i < items.Length; i++)
                    {
                        serialized += "[";
                        for (int j = 0; j < items[i].SubItems.Count; j++)
                        {
                            scaped = items[i].SubItems[j].Text.Replace('[', '|');
                            scaped = scaped.Replace(']', '&');
                            scaped = scaped.Replace(',', ';');
                            serialized += "[" + scaped + "],";
                        }
                        serialized = serialized.Substring(0, serialized.Length - 1);
                        serialized += "],";
                    }
                    serialized = serialized.Substring(0, serialized.Length - 1);
                    return serialized;
                }
                else
                    return "";
            }
            catch
            { return ""; }
        }

        public static System.Windows.Forms.ListViewItem[] UnserializeItens(string serialized)
        {
            try
            {
                List<System.Windows.Forms.ListViewItem> list = new List<System.Windows.Forms.ListViewItem>();
                if (serialized.Length > 0)
                {/*   
                      *   [[x],[y]], <-lv1 2subs
                      *   [[e],[d],[g]], <-lv2 3 subs
                      *   [[a]] <lv3 1 sub 
                      *  
                      */
                    string auxS = serialized,unScape;
                    System.Windows.Forms.ListViewItem auxLi;
                    List<string> subs = new List<string>();
                    int indexAbre = auxS.IndexOf("["), indexFecha;
                    while (auxS.IndexOf("[") != -1)
                    {
                        if (auxS[indexAbre + 1] == '[')
                        {//subs
                            indexAbre = indexAbre + 2;
                        }
                        else
                            indexAbre++;
                        indexFecha = auxS.IndexOf("]");
                        unScape = auxS.Substring(indexAbre, indexFecha - indexAbre);
                        subs.Add(unScape);
                        if (auxS[indexFecha + 1] == ',')
                        {
                            auxS = auxS.Substring(indexFecha + 1, auxS.Length - (indexFecha + 1));
                        }
                        else
                        {
                            //subs.RemoveAt(subs.Count - 1);
                            foreach(string str in subs)
                            {
                                unScape = str.Replace('|', '[');
                                unScape = unScape.Replace('&', ']');
                                unScape = unScape.Replace(';', ',');
                            }
                            auxLi = new System.Windows.Forms.ListViewItem(subs.ToArray());
                            subs.Clear();
                            list.Add(auxLi);
                            auxS = auxS.Substring(indexFecha + 1, auxS.Length - (indexFecha + 1));
                            indexAbre = auxS.IndexOf(',');
                            if (indexAbre != -1)
                                auxS = auxS.Substring(indexAbre+1, auxS.Length - (indexAbre+1));
                        }
                        indexAbre = auxS.IndexOf("[");
                    }
                    System.Windows.Forms.ListViewItem[] ar = list.ToArray();
                    if (ar == null)
                        ar = new System.Windows.Forms.ListViewItem[0];
                    return ar;
                }
                else
                    return new System.Windows.Forms.ListViewItem[0];
            }
            catch
            { return null; }
        }

        public static bool New(int type, int prioridade, DateTime hoje, DateTime prazo, Status status, 
            System.Windows.Forms.ListViewItem[] items, System.Windows.Forms.ListViewItem[] orcamento,string[] declarado, string[] encontrado, string[] solucao, 
            int usuSol, int setorSol, int usuResp, int setorResp)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    string declaradoJoined = Join(declarado, "<*>"),
                        encontradoJoined = Join(declarado, "<*>"),
                        solucaoJoined = Join(declarado, "<*>"),
                        its = SerializeItens(items),
                        orc = SerializeItens(orcamento);
                    int nextId = 0;
                    
                    DataTable dt = con.Query("Select id from service order by id limit 1");
                    if (dt != null)
                        if (int.TryParse(dt.Rows[0]["id"].ToString(), out nextId))
                            nextId++;
                    if ("" != con.Comando("INSERT INTO service values(" + nextId + "," + type.ToString() + "," + prioridade.ToString() + ",'" + hoje.ToString("yyyy-MM-dd HH:mm:ss") + "','" + prazo.ToString("yyyy-MM-dd HH:mm:ss") + "'," + status.Id.ToString() + ",'"
                        + declaradoJoined + "','" + encontradoJoined + "','" + solucaoJoined + "'," + usuSol.ToString() + "," + setorSol.ToString() + "," + usuResp.ToString() + "," + setorResp.ToString() + ",'" +
                        its + "','" + orc + "')"))
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
                        s.setId(int.Parse(dt.Rows[0]["id"].ToString()));
                        s.type = int.Parse(dt.Rows[0]["type"].ToString());
                        if (DateTime.TryParse(dt.Rows[0]["hoje"].ToString(),out auxDt))
                            s.hoje = auxDt;
                        if (DateTime.TryParse(dt.Rows[0]["prazo"].ToString(),out auxDt))
                            s.prazo = auxDt;
                        s.declarado = Split(dt.Rows[0]["declarado"].ToString(), "<*>");
                        s.encontrado = Split(dt.Rows[0]["encontrado"].ToString(), "<*>");
                        s.solucao = Split(dt.Rows[0]["solucao"].ToString(), "<*>");
                        s.prioridade = new Prioridade(int.Parse(dt.Rows[0]["prioridade"].ToString()));
                        s.status = new Status(int.Parse(dt.Rows[0]["status"].ToString()));
                        s.usuSol = int.Parse(dt.Rows[0]["usuSol"].ToString());
                        s.setorSol = int.Parse(dt.Rows[0]["setorSol"].ToString());
                        s.usuResp = int.Parse(dt.Rows[0]["usuResp"].ToString());
                        s.setorResp = int.Parse(dt.Rows[0]["setorResp"].ToString());
                        s.items = Service.UnserializeItens(dt.Rows[0]["items"].ToString());
                        s.orcamento = Service.UnserializeItens(dt.Rows[0]["orcamento"].ToString());

                        s.conexao = con;
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
                        "',declarado='" + Join(this.declarado, "<*>") +
                        "',encontrado='" + Join(this.encontrado, "<*>") +
                        "',solucao='" + Join(this.solucao, "<*>") +
                        "',prioridade=" + this.prioridade.Id.ToString() +
                        ",status=" + status.Id.ToString()+
                        ",usuSol=" + this.usuSol.ToString() +
                        ",setorSol=" + this.setorSol.ToString() +
                        ",usuResp=" + this.usuResp.ToString() +
                        ",setorResp=" + this.setorResp.ToString();
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
                    if (this.declarado != NewOne.declarado)
                        query += ",declarado='" + Join(NewOne.declarado, "<*>") + "'";
                    if (this.encontrado != NewOne.encontrado)
                        query += ",encontrado='" + Join(NewOne.encontrado, "<*>") + "'";
                    if (this.solucao != NewOne.solucao)
                        query += ",solucao='" + Join(NewOne.solucao, "<*>") + "'";
                    if (this.prioridade.Id != NewOne.prioridade.Id)
                        query += ",prioridade=" + NewOne.prioridade.Id.ToString();
                    if (this.usuSol != NewOne.usuSol)
                        query += ",usuSol=" + NewOne.usuSol.ToString();
                    if (this.setorSol != NewOne.setorSol)
                        query += ",setorSol=" + NewOne.setorSol.ToString();
                    if (this.usuResp != NewOne.usuResp)
                        query += ",usuResp=" + NewOne.usuResp.ToString();
                    if (this.setorResp != NewOne.setorResp)
                        query += ",setorResp=" + NewOne.setorResp.ToString();

                    if (this.status.Id != NewOne.status.Id)
                        query += ",status=" + NewOne.status.Id.ToString();

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
    public class Service_Item
    {
        private int id;
        private int service;
        private int quant;
        private string descricao;
        private string marca;
        private string modelo;

        public static bool New(int service, int quantidade, string descricao,string marca,string modelo)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    if ("" != con.Comando("INSERT INTO service_item values(null," + quantidade.ToString() + ",'" + descricao + "','" + marca +"','"+modelo+"')"))
                        return false;
                    else
                    {
                        return true;
                    }
                }
                else
                    return false;

            }
            catch { return false; }
        }
            /* 
         * service_item
         * id
         * quant
         * descricao
         * marca
         * modelo*/
    }

    public struct Prioridade
    {
        private int id;
        public int Id
        {
            get { return id; }
        }

        private string nome;
        public string Nome
        {
            get{return nome;}
        }
        
        public Prioridade(int ID)
        {
            this.id = ID;
            switch (ID)
            {
                case 0:
                    nome = "Baixa";
                    break;
                case 1:
                    nome = "Media";
                    break;
                case 2:
                    nome = "Alta";
                    break;
                case 3:
                    nome = "Urgente";
                    break;
                default:
                    id = 0;
                    nome = "Baixa";
                    break;
            }

        }

        public static List<Prioridade> All = new List<Prioridade>() 
        {
            Prioridade.Baixa , 
            Prioridade.Media,
            Prioridade.Alta,
            Prioridade.Urgente
        };

        public static Prioridade Baixa
        {
            get 
            {
                Prioridade s = new Prioridade();
                s.id = 0;
                s.nome = "Baixa";
                return s; 
            }
        }
        public static Prioridade Media
        {
            get
            {
                Prioridade s = new Prioridade();
                s.id = 1;
                s.nome = "Media";
                return s;
            }
        }
        public static Prioridade Alta
        {
            get
            {
                Prioridade s = new Prioridade();
                s.id = 2;
                s.nome = "Alta";
                return s;
            }
        }
        public static Prioridade Urgente
        {
            get
            {
                Prioridade s = new Prioridade();
                s.id = 3;
                s.nome = "Urgente";
                return s;
            }
        }
    }

    public struct TipoDeOrdem
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

    public struct Status
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
                    nome = "Em Andamento";
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
            Status.EmAndamento,
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
        public static Status EmAndamento
        {
            get
            {
                Status s = new Status();
                s.id = 5;
                s.nome = "Em Andamento";
                return s;
            }
        }
        
    }
}
