using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Data.SQLite;

namespace Services
{
    class Conexao
    {
        public string sqliteConectString = @"Data Source=services;Version=3;";

        private SQLiteConnection sqliteCon;
        MySqlConnection conexao;

        public static bool noServerDatabase = true;

        private string user;
        private string server;
        private string pass;
        private string dataBase;
        private string conectString;
        private bool sucessedOnLastLoad = false;
        
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
                if (noServerDatabase)
                {
                    sqliteCon = new SQLiteConnection(sqliteConectString);
                    setLastLoadState(true);
                }
                else
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
                if (noServerDatabase)
                {

                }
                else
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
            }
            catch
            {
                setLastLoadState(false);
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
                if (noServerDatabase)
                {
                    sqliteCon = new SQLiteConnection(sqliteConectString);
                    sqliteCon.Open();
                    sqliteCon.Close();
                    return "";
                }
                else
                {
                    MySqlConnection testaCon = new MySqlConnection(this.conectString);
                    testaCon.Open();
                    testaCon.Close();
                    return "";
                }
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
                    return null;
                int index = query.IndexOf("FROM ");
                if (index != -1)
                {
                    tableName = query.Substring(index + 5, query.Length - 5 - index);
                    if (tableName.IndexOf(" ")!=-1)
                        tableName = tableName.Substring(0, tableName.IndexOf(" "));
                }
                DataTable dt = new DataTable();
                dt.TableName = tableName;

                if (noServerDatabase)
                {
                    sqliteCon = new SQLiteConnection(sqliteConectString);
                    sqliteCon.Open();
                    SQLiteDataAdapter adap = new SQLiteDataAdapter(query, sqliteCon);
                    adap.Fill(dt);
                    sqliteCon.Close();
                }
                else
                {
                    if (conexao == null)
                        conexao = new MySqlConnection("server=" + server + ";user id=" + user + ";password=" + pass + ";database=" + dataBase);
                    conexao.Open();
                    //query = MySqlHelper.EscapeString(query);
                    MySqlDataAdapter mAdapter = new MySqlDataAdapter(query, conexao);
                    mAdapter.Fill(dt);
                    conexao.Close();
                }
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
            catch(Exception er)
            {
                System.Windows.Forms.MessageBox.Show(er.Message);
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

        public string Comando(string query)
        {
            try
            {
                if (noServerDatabase)
                {
                    sqliteCon = new SQLiteConnection(sqliteConectString);
                    sqliteCon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(query, sqliteCon);
                    cmd.ExecuteNonQuery();
                    sqliteCon.Close();
                    return "";
                }
                else
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
                }
                /*Arquivos arc = new Arquivos();
                string log = arc.criaLog(query, "comandoMysql");
                if (log != "")
                    return "Erro de Log: " + log;
                else
                    return "";*/
            }
            catch (Exception es)
            {
                if (noServerDatabase)
                    sqliteCon.Close();
                else
                    this.conexao.Close();
                return es.Message;
            }
        }
        public bool Check()
        {
            try
            {
                if (this.getLastLoadState())
                {
                    if (Conexao.noServerDatabase)
                    {//sqlite
                        string erro;
                        DataTable dt = Query("SELECT name FROM sqlite_master WHERE type='table'");
                        if (dt != null)
                        {
                            DataColumn[] key = { dt.Columns["name"] };
                            dt.PrimaryKey = key;
                            foreach (KeyValuePair<string, string> pair in DataBaseCheck.sqlite_tables)
                            {
                                if (!dt.Rows.Contains(pair.Key))
                                {
                                    erro = Comando(pair.Value);
                                    if (erro != "")
                                        System.Windows.Forms.MessageBox.Show(erro);
                                    else
                                        if (DataBaseCheck.sqlite_FirstInsert[pair.Key]!="")
                                            if (Comando(DataBaseCheck.sqlite_FirstInsert[pair.Key]) != "")
                                                System.Windows.Forms.MessageBox.Show(erro);
                                        
                                }
                            }
                        }
                        else
                            foreach (KeyValuePair<string, string> pair in DataBaseCheck.sqlite_tables)
                            {
                                erro = Comando(pair.Value);
                                if (erro != "")
                                    System.Windows.Forms.MessageBox.Show(erro);
                                else
                                    if (DataBaseCheck.sqlite_FirstInsert[pair.Key] != "")
                                        if (Comando(DataBaseCheck.sqlite_FirstInsert[pair.Key]) != "")
                                            System.Windows.Forms.MessageBox.Show(erro);
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
    class DataBaseCheck
    {
        public static Dictionary<string, string> sqlite_tables = new Dictionary<string, string>(){
            {"service","CREATE TABLE 'service' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'type' INTEGER, 'prioridade' INTEGER, 'hoje' DATETIME, 'prazo' DATETIME, 'status' INTEGER, 'declarado' TEXT, 'encontrado' TEXT, 'solucao' TEXT, 'usuSol' INT, 'setorSol' INT, 'usuResp' INT, 'setorResp' INT, 'items' TEXT, 'orcamento' TEXT)"},
            {"user","CREATE TABLE 'user' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'privilegio' INTEGER, 'setor' INTEGER, 'login' TEXT, 'pass' TEXT, 'nome' TEXT, 'ultimoAcesso' DATETIME)"},
            {"setor","CREATE TABLE 'setor' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'type' INTEGER, 'nome' TEXT)"},
            {"service_item","CREATE TABLE 'service_item' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'quant' INTEGER, 'descricao' TEXT, 'marca' TEXT, 'modelo' TEXT)"},
            {"service_orca","CREATE TABLE 'service_orca' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'quant' INTEGER, 'descricao' TEXT, 'valor' FLOAT)"}
        };

        public static Dictionary<string, string> sqlite_FirstInsert = new Dictionary<string, string>(){
            {"service",""},
            {"user","INSERT INTO user (id,privilegio,setor,login,pass,nome,ultimoAcesso) VALUES (NULL,0,1,'admin','admin','Administrador',NULL)"},
            {"setor","INSERT INTO setor (nome) VALUES('Escritorio')"},
            {"service_item",""},
            {"service_orca",""}
        };

        public static Dictionary<string, string> mysql_tables = new Dictionary<string, string>(){
            {"service",""},
            {"user","INSERT INTO user (privilegio,login,pass,nome) VALUES(1,'admin','admin','Admnistrador')"},
            {"setor",""}
        };
        public static Dictionary<string, string> mysql_FirstInsert = new Dictionary<string, string>(){
            {"service",""},
            {"user","INSERT INTO user (id,privilegio,setor,login,pass,nome,ultimoAcesso) VALUES (NULL,0,1,'admin','admin','Administrador',NULL)"},
            {"setor","INSERT INTO setor (nome) VALUES('Escritorio')"},
            {"service_item",""},
            {"service_orca",""}
        };
        /* 
         * service_item
         * id
         * quant
         * descricao
         * marca
         * modelo
         * 
         * service_orca
         * id
         * quant
         * descricao
         * valor
         * 
         * service
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
         * items
         * orcamento
         * 
         * itens??
         * fin???
         * 
         * user
         * id
         * privilegio
         * setor
         * login 
         * pass
         * nome
         * ultimoAcesso
         * 
         * setor
         * id
         * nome
         * 
         *
         */

        public DataBaseCheck()
        {
            
                /*
                string table_Service = "CREATE TABLE `" + dataBaseName + "`.`service` (" +
            "`id` INT NOT NULL," +
            "`type` TINYINT NULL," +
            "`prioridade` TINYINT NULL," +
            "`hoje` DATETIME NULL," +
            "`prazo` DATETIME NULL," +
            "`status` TINYINT NULL," +
            "`conteudo` TEXT NULL," +
            "`recebido` TINYINT NULL," +
            "PRIMARY KEY (`id`))";

                string sqlite_com = "CREATE TABLE 'service' ('id' INTEGER PRIMARY KEY NOT NULL, 'type' INTEGER, 'prioridade' INTEGER, 'hoje' DATETIME, 'prazo' DATETIME, 'status' INTEGER, 'conteudo' TEXT, 'resposta' TEXT, 'recebido' BOOLEAN)";
                string query = "CREATE TABLE 'service' ('id' INTEGER PRIMARY KEY NOT NULL, 'type' INTEGER, 'prioridade' INTEGER, 'hoje' DATETIME, 'prazo' DATETIME, 'status' INTEGER, 'conteudo' TEXT, 'resposta' TEXT, 'recebido' BOOLEAN)";
                query = "INSERT INTO service (id,type,prioridade,hoje,prazo,status,conteudo,resposta,recebido) VALUES(2,0,0,'2015-08-17','2015-09-02',0,'nada','nada tb',1)";
                
                 * sqliteCon = new SQLiteConnection(Conexao.sqliteConectString);
                        sqliteCon.Open();
                        string query = "CREATE TABLE 'service' ('id' INTEGER PRIMARY KEY NOT NULL, 'type' INTEGER, 'prioridade' INTEGER, 'hoje' DATETIME, 'prazo' DATETIME, 'status' INTEGER, 'conteudo' TEXT, 'resposta' TEXT, 'recebido' BOOLEAN)";
                        SQLiteCommand cmd = new SQLiteCommand(query, sqliteCon);
                        cmd.ExecuteNonQuery();
                        query = "INSERT INTO service (id,type,prioridade,hoje,prazo,status,conteudo,resposta,recebido) VALUES(2,0,0,'2015-08-17','2015-09-02',0,'nada','nada tb',1)"; 
                        cmd = new SQLiteCommand(query, sqliteCon);
                        cmd.ExecuteNonQuery();
                        System.Data.DataTable dt = new System.Data.DataTable();
                        SQLiteDataAdapter adap = new SQLiteDataAdapter("Select * from service", sqliteCon);
                        adap.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            MessageBox.Show(dt.Rows[0]["id"].ToString());
                        }
                        else
                            MessageBox.Show("nada");
                        sqliteCon.Close();
                 */
            
        }
        

    }
}
