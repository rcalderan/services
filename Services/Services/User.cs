using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    class User
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private Privilegio privilegio;
        public Privilegio Privilegio
        {
            get { return privilegio; }
            set { privilegio = value; }
        }
        private Setor setor;
        public Setor Setor
        {
            get { return setor; }
            set { setor = value; }
        }

        private string login;
        public string Login
        {
            get { return login; }
            set { login = value; }
        }
        private string pass;
        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }
        private string nome;
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }
        private DateTime ultimoAcesso;
        public DateTime UltimoAcesso
        {
            get { return ultimoAcesso; }
            set { ultimoAcesso = value; }
        }

        private Conexao conexao;

        public User()
        {
        }
        public static User Load(int id)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    User u = new User();
                    u.conexao = con;
                    System.Data.DataTable dt = con.Query("SELECT * FROM user where id=" + id.ToString());
                    if (dt != null)
                    {
                        DateTime auxDt;
                        u.id = id;
                        u.nome = dt.Rows[0]["nome"].ToString();
                        u.login = dt.Rows[0]["login"].ToString();
                        u.setor = new Setor(int.Parse(dt.Rows[0]["setor"].ToString()));
                        u.pass = dt.Rows[0]["pass"].ToString();
                        u.privilegio = new Privilegio(int.Parse(dt.Rows[0]["privilegio"].ToString()));
                        u.conexao = con;
                        if (DateTime.TryParse(dt.Rows[0]["ultimoAcesso"].ToString(), out auxDt))
                            u.ultimoAcesso = auxDt;
                        return u;
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

        public static bool New(Privilegio priv,Setor setor,string login ,string pass,string nome)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {//sqlite
                        string query = "SELECT id FROM user ORDER BY id limit 1";
                        System.Data.DataTable dt = con.Query(query);
                        query = "INSERT INTO user (id,privilegio,setor,login,pass,nome,ultimoAcesso) VALUES(null," +
                            priv.id.ToString() + "," +
                            setor.Id.ToString() + ",'" +
                            login + "','" +
                            pass + "','" +
                            nome + "','" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        if (con.Comando(query) != "")
                            return false;
                    }
                    else
                    {//mysql

                    }
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
        /*
        public bool Update(User Novo)
        {
            try
            {
                if (conexao.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {//sqlite
                        {//update
                            string query = "UPDATE user SET id="+this.id.ToString();

                            if (this.privilegio.id != Novo.privilegio.id)
                                query += ",privilegio=" + Novo.privilegio.id.ToString();
                            if (login != Novo.login)
                                query += ",login='" + Novo.ToString() + "'";
                            if (pass != Novo.pass)
                                query += ",pass='" + Novo.pass + "'";
                            if (nome != Novo.nome)
                                query += "',nome='" + Novo.nome + "'";
                            if (setor != Novo.setor)
                                query += "',setor='" + Novo.setor.Id.ToString() + "'";
                            if (ultimoAcesso != Novo.ultimoAcesso)
                                query += "',ultimoAcesso='" + Novo.ultimoAcesso.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            query+="' WHERE id=" + this.id;
                            if (conexao.Comando(query) != "")
                                return false;
                        }
                    }
                    else
                    {//mysql

                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception es)
            {
                string s = es.Message;
                return false;
            }
        }
        */
        public static bool SetLastAcess(int id, DateTime last)
        {
            try
            {
                Conexao con = new Conexao();
                if (con.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {
                        System.Data.DataTable dt = con.Query("select * from user where id=" + id.ToString());
                        if (dt != null)
                        {
                            if ("" != con.Comando("UPDATE user SET ultimoAcesso='" + last.ToString("yyyy-MM-dd HH:mm:ss")))
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                    {//mysql

                    }
                    return true;
                }
                else
                    return false;
            }
            catch { return false; }
        }
    }



    struct Privilegio
    {
        public int id;
        public string nome;

        public Privilegio(int nivel)
        {
            id = nivel;
            switch (nivel)
            {
                case 0:
                    nome = "Adminstrador";
                    break;
                case 1:
                    nome = "Avançado";
                    break;
                case 2:
                    nome = "Comum";
                    break;
                default:
                    id = 2;
                    nome = "Comum";
                    break;
            }
        }
        

        public static List<Privilegio> All = new List<Privilegio>() 
        {
            Privilegio.Administrador, Privilegio.Avançado, Privilegio.Comum 
        };


        public static Privilegio Administrador
        {
            get 
            {
                Privilegio p =  new Privilegio();
                p.id =0;
                p.nome = "Adminstrador";
                return p; 
            }
        }
        public static Privilegio Avançado
        {
            get 
            {
                Privilegio p =  new Privilegio();
                p.id =1;
                p.nome = "Avançado";
                return p; 
            }
        }
        public static Privilegio Comum
        {
            get 
            {
                Privilegio p =  new Privilegio();
                p.id =2;
                p.nome = "Comum";
                return p; 
            }
        }

        
    }
}
