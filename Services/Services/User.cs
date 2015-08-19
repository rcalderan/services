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
            //set { id = value; }
        }

        private Permissao permissao;
        public Permissao Permissao
        {
            get { return permissao; }
            set { permissao = value; }
        }

        private string login;
        public string Login
        {
            get { return login; }
            set { login = value; }
        }
        private string pass;
        public string Senha
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
                    System.Data.DataTable dt = con.Query("SELECT * FROM user where id=" + id.ToString());
                    if (dt != null)
                    {
                        DateTime auxDt;
                        u.id = id;
                        u.nome = dt.Rows[0]["nome"].ToString();
                        u.login = dt.Rows[0]["login"].ToString();
                        u.pass = dt.Rows[0]["senha"].ToString();
                        u.Permissao = new Permissao(int.Parse(dt.Rows[0]["permissao"].ToString()));
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

        public bool New()
        {
            try
            {
                if (conexao.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {//sqlite
                        string query = "SELECT id FROM user ORDER BY id limit 1";
                        System.Data.DataTable dt = conexao.Query(query);
                        int next = 0;
                        if (dt != null)
                            next = int.Parse(dt.Rows[0]["id"].ToString());
                        next++;
                        query = "INSERT INTO user (id,permissao,login,pass,nome,ultimoAcesso) VALUES(" +
                            next.ToString() + "," +
                            this.Permissao.id.ToString() + ",'" +
                            this.login + "','" +
                            this.pass + "','" +
                            this.nome + "','" +
                            this.ultimoAcesso.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        if (conexao.Comando(query) != "")
                            return false;
                        else
                            return true;

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

        public bool Save()
        {
            try
            {
                if (conexao.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {//sqlite
                        string query = "SELECT id FROM user where id=" + this.id.ToString();
                        System.Data.DataTable dt = conexao.Query(query);
                        if (dt==null)
                        {//novo
                            query = "INSERT INTO user (id,permissao,login,pass,nome,ultimoAcesso) VALUES("+
                                this.id.ToString()+","+
                                this.Permissao.id.ToString()+",'"+
                                this.login+"','"+
                                this.pass+"','"+
                                this.nome+"','"+
                                this.ultimoAcesso.ToString("yyyy-MM-dd HH:mm:ss")+"')";
                            if (conexao.Comando(query) != "")
                                return false;
                            else
                                return true;
                        }
                        else
                        {//update
                            query = "UPDATE user SET " +
                                "id=" + this.id.ToString() +
                            ",permisao=" + this.permissao.id.ToString() +
                            ",login='" + this.ToString() +
                            "',pass='" + this.pass +
                            "',nome='" + this.nome +
                            "',ultimoAcesso='" + this.ultimoAcesso.ToString("yyyy-MM-dd HH:mm:ss") +
                            "' WHERE id=" + this.id;
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
            catch
            {
                return false;
            }
        }
    }



    struct Permissao : User
    {
        public int id;
        public string nome;

        public Permissao(int nivel)
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

        public static int Administrador
        {
            get { return 0; }
        }
        public static int Avançado
        {
            get { return 1; }
        }
        public static int Comum
        {
            get { return 2; }
        }

        
    }
}
