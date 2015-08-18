using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace Services
{
    class Conexao
    {
        private string user;
        private string server;
        private string pass;
        private string dataBase;
        private string conectString;
        private bool sucessedOnLastLoad = false;
        MySqlConnection conexao;
        
        public bool getLastLoadState()
        {
            return sucessedOnLastLoad;
        }
        public void setLastLoadState(bool state)
        {
            sucessedOnLastLoad = state;
        }

        public void _setUser(string novoUser)
        {
            this.user = novoUser;
            //_setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
        }
        public void _setServer(string novoServ)
        {
            this.server = novoServ;
            //_setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
        }
        public void _setPassword(string novoPass)
        {
            this.pass = novoPass;
            //_setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
        }
        public void _setDatabase(string novoDb)
        {
            this.dataBase = novoDb;
            //_setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
        }
        private void _setConStr(string novoConStr)
        {
            this.conexao = null;
            conectString = novoConStr;
        }

        public string getUser()
        {
            return this.user;
        }

        public MySqlConnection getConnection()
        {
            return this.conexao;
        }

        public string getPass()
        {
            return this.pass;
        }

        public string getServer()
        {
            return this.server;
        }

        public string getDatabase()
        {
            return this.dataBase;
        }

        public Conexao()
        {
            try
            {
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
                if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                {
                    Dictionary<string, string> data = Arquivos.getDataFromReg();

                    _setServer(data["confHostTb"]);
                    _setUser(data["confUserTb"]);
                    _setPassword(data["confPassTb"]);
                    _setDatabase("noivabd");
                    _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                }
                else
                {
                    _setServer("192.168.0.10");
                    _setUser("root");
                    _setPassword("33722363");
                    _setDatabase("project14");
                    _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                }
                if (checkStrings() == "")
                    setLastLoadState(true);
                else
                    setLastLoadState(false);
            }
            catch
            {
                setLastLoadState(false);
            }
        }

        public Conexao(string Host,string User,string Password)
        {
            try
            {
                _setServer(Host);
                _setUser(User);
                _setPassword(Password);
                _setDatabase("noivabd");
                _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                string erro = checkStrings();
                if (erro == "")
                {
                    setLastLoadState(true);
                    
                }
                else
                {
                    setLastLoadState(false);
                System.Windows.Forms.MessageBox.Show(erro);
                }
            }
            catch (Exception ex)
            {
                setLastLoadState(false);

                System.Windows.Forms.MessageBox.Show("Catch: "+ ex.Message);
                //System.Windows.Forms.MessageBox.Show("Erro no processo de conexão!:\n" + ex.Message + "\n\nContate o programador!");
                //System.Windows.Forms.Application.Exit();
            }
        }

        public Dictionary<string,string> get_cep(string cep)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();
            try
            {
                Encoding iso = Encoding.GetEncoding("ISO-8859-1");
                Encoding utf8 = Encoding.UTF8;
                _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=ceps");
                string cep5 = cep.Substring(0, 5);
                DataTable dt = new DataTable("uf");
                this.conexao = new MySqlConnection(this.conectString);
                this.conexao.Open();
                MySqlDataAdapter mAdapter = new MySqlDataAdapter("select * from cep_log_index where cep5=\"" + cep5 + "\"", conexao);
                this.conexao.Close();
                mAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string uf = dt.Rows[0]["uf"].ToString();
                    DataTable dados = this.Query("select * from " + uf + " where cep=\"" + cep + "\"");
                    if (dados != null)
                    {
                        resultado.Add("id", iso.GetString(Encoding.Convert(utf8, iso, utf8.GetBytes(dados.Rows[0]["id"].ToString()))));
                        resultado.Add("cidade", iso.GetString(Encoding.Convert(utf8, iso, utf8.GetBytes(dados.Rows[0]["cidade"].ToString()))));
                        resultado.Add("logradouro", iso.GetString(Encoding.Convert(utf8, iso, utf8.GetBytes(dados.Rows[0]["logradouro"].ToString()))));
                        resultado.Add("bairro", iso.GetString(Encoding.Convert(utf8, iso, utf8.GetBytes(dados.Rows[0]["bairro"].ToString()))));
                        resultado.Add("cep", iso.GetString(Encoding.Convert(utf8, iso, utf8.GetBytes(dados.Rows[0]["cep"].ToString()))));
                        resultado.Add("tp_logradouro", iso.GetString(Encoding.Convert(utf8, iso, utf8.GetBytes(dados.Rows[0]["tp_logradouro"].ToString()))));
                        resultado.Add("uf", uf);
                    }
                }
                _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                return resultado;
            }
            catch
            {
                _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                return resultado;
            }
        }

        public DataRowCollection get_cep2(int cep)
        {
            try
            {
                DataTable dt = new DataTable("ceps");
                _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=cep");
                this.conexao = new MySqlConnection(this.conectString);
                this.conexao.Open();
                MySqlDataAdapter mAdapter = new MySqlDataAdapter("select * from cep where cep='"+cep.ToString()+"'", conexao);
                this.conexao.Close();
                mAdapter.Fill(dt);
                _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                if (dt.Rows.Count == 0)
                    return null;
                else
                    return dt.Rows;
            }
            catch
            {
                _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                return null;
            }
        }


        public static bool testaUmaConexao(MySqlConnection conexao)
        {
            try
            {
                if (conexao == null)
                {
                    MySqlConnection testaCon = new MySqlConnection();
                    testaCon.Open();
                    testaCon.Close();
                    return true;
                }
                else
                {
                    conexao.Open();
                    conexao.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public string checkStrings()
        {
            try
            {
                MySqlConnection testaCon = new MySqlConnection(this.conectString);
                testaCon.Open(); 
                testaCon.Close();
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public DataTable Query(string query)
        {
            try
            {
                string tableName = "",conMessErro =this.checkStrings();
                if (conMessErro != "")
                    System.Windows.Forms.MessageBox.Show(conMessErro);
                int index = query.IndexOf("FROM ");
                if (index != -1)
                {
                    tableName = query.Substring(index + 5, query.Length - 5 - index);
                    tableName = tableName.Substring(0, tableName.IndexOf(" "));
                }
                DataTable dt = new DataTable();
                dt.TableName = tableName;
                if (conexao==null)
                    conexao = new MySqlConnection("server=" + server + ";user id=" + user + ";password=" + pass + ";database=" + dataBase);
                conexao.Open();
                //query = MySqlHelper.EscapeString(query);
                MySqlDataAdapter mAdapter = new MySqlDataAdapter(query, conexao);
                mAdapter.Fill(dt);
                conexao.Close();
                if (dt.Rows.Count == 0)
                    return null;
                else
                {
                    if (dt.Columns.IndexOf("codigo") != -1)
                    {
                        DataColumn[] key = {dt.Columns[dt.Columns.IndexOf("codigo")]};
                        dt.PrimaryKey = key;
                    }
                    else
                        if (dt.Columns.IndexOf("id") != -1)
                        {
                            DataColumn[] key = { dt.Columns[dt.Columns.IndexOf("id")] };
                            dt.PrimaryKey = key;
                        }
                    return dt;
                }
            }
            catch
            {
                this.conexao.Close();
                return null;
            }
        }

        public string QueryMultiple(string[] queries)
        ///Select into List S1 and List S2 from  Database (2 fields)
        {
            try
            {
                using (var connection = new MySqlConnection(conectString))
                using (var command = connection.CreateCommand())
                {
                    for (int i = 0; i < queries.Length; i++)
                        queries[i] += "; ";

                    connection.Open();
                    command.CommandText = string.Concat(queries);
                    int aux=0;
                    using (var reader = command.ExecuteReader())
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                //aux = reader.GetInt32(0);
                            }
                            //System.Windows.Forms.MessageBox.Show(aux.ToString());
                        } while (reader.NextResult());

                    }
                    connection.Close();
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        public static int proximoCodigo(string tabela, string chavePrimaria)
        {
            Conexao con = new Conexao();
            if (con.getLastLoadState())
            {
                DataTable tb = con.Query("select " + chavePrimaria + " from " + tabela );
                if (tb != null)
                {
                    DataColumn colCod = tb.Columns[chavePrimaria];
                    DataColumn[] keys = { colCod };
                    tb.PrimaryKey = keys;
                    int aux = 1;
                    DataRow row;
                    for (int i = 1; i <= tb.Rows.Count; i++)
                    {
                        aux = i;
                        row = tb.Rows.Find(i);
                        if (row == null)
                        {
                            return i;
                        }
                    }
                    aux++;
                    return aux;
                }
                else
                {
                    return 1;
                }
            }
            else
                return 1;
        }

        public string comandoMysql(string query)
        {
            try
            {
                this.checkStrings();
                if (this.conexao == null)
                    this.conexao = new MySqlConnection(this.conectString);
                this.conexao.Open();
                //query = MySqlHelper.EscapeString(query);
                MySqlCommand comando = new MySqlCommand(query, conexao);
                //comando.Parameters.Add()
                comando.ExecuteNonQuery();
                this.conexao.Close();
                return "";
                /*Arquivos arc = new Arquivos();
                string log = arc.criaLog(query, "comandoMysql");
                if (log != "")
                    return "Erro de Log: " + log;
                else
                    return "";*/
            }
            catch (Exception es)
            {
                this.conexao.Close();
                return es.Message;
            }
        }
    }
    class DataBaseCheck
    {
        private string dataBaseName = "project14";

        private string table_Service;

        public DataBaseCheck()
        {
            Conexao con = new Conexao();
            if (con.getLastLoadState())
            {
                table_Service = "CREATE TABLE `" + dataBaseName + "`.`service` (" +
            "`id` INT NOT NULL," +
            "`type` TINYINT NULL," +
            "`prioridade` TINYINT NULL," +
            "`hoje` DATETIME NULL," +
            "`prazo` DATETIME NULL," +
            "`status` TINYINT NULL," +
            "`conteudo` TEXT NULL," +
            "`recebido` TINYINT NULL," +
            "PRIMARY KEY (`id`))";
            }
        }

    }
}
