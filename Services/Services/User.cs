﻿using System;
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

        private Privilegio privilegio;
        public Privilegio Privilegio
        {
            get { return privilegio; }
            set { privilegio = value; }
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
                        query = "INSERT INTO user (id,privilegio,login,pass,nome,ultimoAcesso) VALUES(" +
                            next.ToString() + "," +
                            this.privilegio.id.ToString() + ",'" +
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
                            query = "INSERT INTO user (id,privilegio,login,pass,nome,ultimoAcesso) VALUES("+
                                this.id.ToString()+","+
                                this.privilegio.id.ToString()+",'"+
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
                            ",privilegio=" + this.privilegio.id.ToString() +
                            ",login='" + this.ToString() +
                            "',pass='" + this.pass +
                            "',nome='" + this.nome +
                            "',ultimoAcesso='" + this.ultimoAcesso.ToString("yyyy-MM-dd HH:mm:ss") +
                            "' WHERE id=" + this.id;
                            if (conexao.Comando(query) != "")
                                return false;
                            else
                                return true;
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
