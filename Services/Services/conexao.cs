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
                    string[] aux = Arquivos.getDataFromArq();
                    if (aux.Length == 3)
                    {
                        _setServer(aux[0]);
                        _setUser(aux[1]);
                        _setPassword(aux[2]);
                        _setDatabase("noivabd");
                        _setConStr("server=" + this.server + ";user id=" + this.user + ";password=" + this.pass + ";database=" + this.dataBase);
                    }
                }
                if (checkStrings() == "")
                    setLastLoadState(true);
                else
                    setLastLoadState(false);
            }
            catch (Exception ex)
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
                Arquivos arc = new Arquivos();
                string log = arc.criaLog(query, "comandoMysql");
                if (log != "")
                    return "Erro de Log: " + log;
                else
                    return "";
            }
            catch (Exception es)
            {
                this.conexao.Close();
                return es.Message;
            }
        }
        /*
        public static Dictionary<string, string> getDataFromReg()
        { //host, user, pass
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                string aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem", "nm", null);
                if (aux != null)
                {
                    string decr = Arquivos.Decrypt(aux, true);
                    string[] s = decr.Split(' ');
                    result.Add("confHostTb", s[0]);
                    result.Add("confUserTb", s[1]);
                    result.Add("confPassTb", s[2]);
                }
                else
                {
                    string[] s = { "localhost", "", "" };
                    string valor = Arquivos.Encrypt(s[0] + " " + s[1] + " " + s[2], true);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem", "nm", valor, RegistryValueKind.String);
                    result.Add("confHostTb", s[0]);
                    result.Add("confUserTb", s[1]);
                    result.Add("confPassTb", s[2]);
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confAoFecharCh", null);
                if (aux != null)
                {
                    result.Add("confAoFecharCh", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confAoFecharCh", "1", RegistryValueKind.String);
                    result.Add("confAoFecharCh", "1");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confAoAbrirCh", null);
                if (aux != null)
                {
                    result.Add("confAoAbrirCh", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confAoAbrirCh", "0", RegistryValueKind.String);
                    result.Add("confAoAbrirCh", "0");
                }

                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confFantasiaTb", null);
                if (aux != null)
                {
                    result.Add("confFantasiaTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confFantasiaTb", "Nome Fantasia da Empresa", RegistryValueKind.String);
                    result.Add("confFantasiaTb", "Nome Fantasia da Empresa");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEmpresaTb", null);
                if (aux != null)
                {
                    result.Add("confEmpresaTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEmpresaTb", "", RegistryValueKind.String);
                    result.Add("confEmpresaTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confCnpjTb", null);
                if (aux != null)
                {
                    result.Add("confCnpjTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confCnpjTb", "", RegistryValueKind.String);
                    result.Add("confCnpjTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEnderTb", null);
                if (aux != null)
                {
                    result.Add("confEnderTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEnderTb", "", RegistryValueKind.String);
                    result.Add("confEnderTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confNumeroTb", null);
                if (aux != null)
                {
                    result.Add("confNumeroTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confNumeroTb", "", RegistryValueKind.String);
                    result.Add("confNumeroTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confCepTb", null);
                if (aux != null)
                {
                    result.Add("confCepTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confCepTb", "", RegistryValueKind.String);
                    result.Add("confCepTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confBairroTb", null);
                if (aux != null)
                {
                    result.Add("confBairroTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confBairroTb", "", RegistryValueKind.String);
                    result.Add("confBairroTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confCidadeTb", null);
                if (aux != null)
                {
                    result.Add("confCidadeTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confCidadeTb", "", RegistryValueKind.String);
                    result.Add("confCidadeTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confUfTb", null);
                if (aux != null)
                {
                    result.Add("confUfTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confUfTb", "", RegistryValueKind.String);
                    result.Add("confUfTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confTel1Tb", null);
                if (aux != null)
                {
                    result.Add("confTel1Tb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confTel1Tb", "", RegistryValueKind.String);
                    result.Add("confTel1Tb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confTel2Tb", null);
                if (aux != null)
                {
                    result.Add("confTel2Tb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confTel2Tb", "", RegistryValueKind.String);
                    result.Add("confTel2Tb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEmailTb", null);
                if (aux != null)
                {
                    result.Add("confEmailTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEmailTb", "", RegistryValueKind.String);
                    result.Add("confEmailTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confSiteTb", null);
                if (aux != null)
                {
                    result.Add("confSiteTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confSiteTb", "", RegistryValueKind.String);
                    result.Add("confSiteTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEstadualTb", null);
                if (aux != null)
                {
                    result.Add("confEstadualTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confEstadualTb", "", RegistryValueKind.String);
                    result.Add("confEstadualTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confMunicipalTb", null);
                if (aux != null)
                {
                    result.Add("confMunicipalTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confMunicipalTb", "", RegistryValueKind.String);
                    result.Add("confMunicipalTb", "");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confBkpCh", null);
                if (aux != null)
                {
                    result.Add("confBkpCh", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confBkpCh", "0", RegistryValueKind.String);
                    result.Add("confBkpCh", "0");
                }
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confBkpPathTb", null);
                if (aux != null)
                {
                    result.Add("confBkpPathTb", aux);
                }
                else
                {
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confBkpPathTb", @"c:\backup", RegistryValueKind.String);
                    result.Add("confBkpPathTb", @"c:\backup");
                }
                string padrao;
                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confDesistenciaTb", null);
                if (aux != null)
                {
                    result.Add("confDesistenciaTb", aux);
                }
                else
                {
                    padrao = @"                                          T E R M O   DE   D E S I S T Ê N C I A"
                        + @"\\\Eu, <nome>, prorador(a) do CPF <cpf> e do RG <rg>, declaro que"
                        + @"\estou DESISTINDO do aluguel de:"
                        + @"\<desc1>" + @"\" + @"\<desc2>" + @"\<desc3>" + @"\<desc4>" + @"\<desc5>" + @"\<desc6>"
                        + @"\<desc7>" + @"\<desc8>" + @"\<desc9>" + @"\<desc10>" + @"\<desc11>" + @"\<desc12>" + @"\<desc13>"
                        + @"\\descriminado(s) no contrato numero <contrato>, que foi assinado no dia <hoje> e seria usado"
                        + @"\no dia <usa>."
                        + @"\Declaro ainda estar ciente que os valores pagos não serão devolvidos a mim e nem transferidos"
                        + @"\a outra pessoa ou outro aluguel."
                        + @"\\\\<Cidade>,  <agora>"
                        + @"\                                                                               _____________________________________"
                        + @"\                                      <nome>";
                    /*padrao = @"LEIA COM ATENÇÃO:\MARIA HELENA DE CARVALHO CONFECÇÕES-ME  acima identificada, e a seguir denominada LOCADORA, e de outro lado "
                        + @"\o Cliente acima identificado, e a seguir denominado LOCATÁRIO, celebram o presente contrato de locação mediante as seguintes"
                        + @"\Cláusulas e Comdições:\Cláusula 1a - O objeto do presente contrato é a Locação de artigos do vestuário, acima especificado."
                        + @"\Cláusula 2a - As datas, para retirada das mercadorias, bem como da sua utilização e devolução encontram-se acima especificadas."
                        + @"\Cláusula 3a - A LOCADORA não se responsabilizará pelas mercadorias que não forem retiradas até a data de uso estabelecida "
                        + @"\                       neste contrato.\Cláusula 4a - A mercadoria alugada veverá ser devolvida até as 18:00 (Dezoito) horas da data estabelecida neste contrato, completa,"
                        + @"\                       tal como foi retirada.\Parágrafo Primeiro: Caso as mercadorias não sejam devolvidas na data prevista neste contrato sofrerão um acréscimo de R$ 10,00"
                        + @"\                       (Dez Reais) por dia útil de atraso, se esta condicão perdurar por 07 (sete) dias será cobrado uma nova taxa de"
                        + @"\                       Locação da mesma de acordo com o valor pago pelo LOCATÁRIO.\Parágrafo Segundo: Caso as mercadorias sejam devolvidas com manchas de gordura, graxa, tinta ou qualquer outro produto que as"
                        + @"\                       danifique, será cobrado uma taxa de R$ 30,00 (Trinta Reais).\Cláusula 5a - Em caso de troca das mercadorias objeto do presente contrato será cobrado taxa de R$ 20,00 referente"
                        + @"\                       aos Serviços Executados.\Parágrafo Único: Não é permitido a troca das Mercadorias, após 05 (cinco) dias corridos da data de locação."
                        + @"\Cláusula 6a - Em caso de desistência por parte do LOCATÁRIO, o valor pago a título de Locação não será devolvido nem transferido"
                        + @"\                       a outras pessoas.\Cláusula 7a - As mercadorias, objeto da referida locação não poderão ser emprestadas ou tranferidas a outras pessoas.\Cláusula 8a - O LOCATÁRIO se compromete a ressarcir a LOCADORA pelo valor de tabela de Mercado, vigente na data do evento,"
                        + @"\                       pelo extravio ou dano nas mercadorias objeto deste contrato, bem como seu uso indevido.\Cláusula 9a - A LOCADORA se compromete a entregar a Mercadoria lavada e passada, com os devidos ajustes solicitados pelo"
                        + @"\                       LOCATÁRIO e em perfeito estado de conservação e uso, de acordo como foi requirido durante a prova na data deste contrato.\Cláusula 10a - Caso seja constatada alguma denificação na Mercadoria locada no momento da retirada pelo LOCATÁRIO, a"
                        + @"\                       LOCADORA se compromete  a efetuar a substituição, ou a troca, independente do seu preço de locação, ou  a devida\                       devolução do valor pago, tudo conforme a disponibilidade do produto ou conveniência da LOCADORA."
                        + @"\Cláusula 11a - E por estarem juntos e acordados, firma o presente contrato, ficando eleito o Fórum desta Comarca para dirimir\                       quaisquer dúvidas que possam surgir.";
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confDesistenciaTb", padrao, RegistryValueKind.String);
                    result.Add("confDesistenciaTb", padrao);
                }

                aux = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confClausulasTb", null);
                if (aux != null)
                {
                    result.Add("confClausulasTb", aux);
                }
                else
                {
                    padrao = @"                                                                                <Nome Fantasia>                                                                                      N.  <contrato>"
                        + @"\<linha>"
                        + @"\<Nome da Empresa>. CNPJ: <CNPJ>, Inscrição Estadual: <Inscrição Estadual>"
                    + @"\e Instrição Municipal: <Inscrição Municipal>, Empresa especializada em Aluguéis de Noivas, Trajes a Rigor e"
                    + @"\Artigos do vestuário em geral. Telefone: <Telefone1> ou <Telefone2>, E-Mail: <email>"
                    + @"\Acesse: <Site>"
                    + @"\\Cliente: <ccli> - <nome>"
                    + @"\CPF: <cpf>"
                    + @"\Endereço: <endereco>, Bairro: <bairro>"
                    + @"\Cidade: <cidade>/<uf>    Telefone: (<ddd1>)<fone1> ou (<ddd2>)<fone2>"
                    + @"\<linha>"
                    + @"\RETIRADA: <retirada>                                             USA DIA: <usa>                                                      Devolução: <devolucao>"
                    + @"\<linha>"
                    + @"\Codigo                                                           Descrição                                                                                                   Valor"
                    + @"\<descreve1>" + @"\<descreve2>" + @"\<descreve3>" + @"\<descreve4>" + @"\<descreve5>" + @"\<descreve6>"
                    + @"\<descreve7>" + @"\<descreve8>" + @"\<descreve9>" + @"\<descreve10>" + @"\<descreve11>" + @"\<descreve12>" + @"\<descreve13>"
                    + @"\<linha>"
                    + @"\                                                                                                                                                                          Total R$ <total>,00"
                    + @"\<linha>" + @"\FORMAS DE PAGAMENTO" + @"\VALOR R$                                                                Vencimento                                                                             Carimbo"
                    + @"\<pagamentos0>" + @"\<pagamentos1>" + @"\<pagamentos2>" + @"\<pagamentos3>" + @"\<pagamentos4>" + @"\<pagamentos5>"
                    + @"\<pagamentos6>" + @"\<pagamentos7>" + @"\<pagamentos8>" + @"\<pagamentos9>"
                    + @"\<linha>"
                    + @"\\LEIA COM ATENÇÃO:"
                    + @"\MARIA HELENA DE CARVALHO CONFECÇÕES-ME  acima identificada, e a seguir denominada LOCADORA, e de outro lado "
                    + @"\o Cliente <nome> (acima identificado), e a seguir denominado LOCATÁRIO,"
                    + @"\celebram o presente contrato de locação mediante as seguintes Cláusulas e Comdições:"
                    + @"\Cláusula 1a - O objeto do presente contrato é a Locação de artigos do vestuário, acima especificado."
                    + @"\Cláusula 2a - As datas, para retirada das mercadorias, bem como da sua utilização e devolução encontram-se acima especificadas."
                    + @"\Cláusula 3a - A LOCADORA não se responsabilizará pelas mercadorias que não forem retiradas até a data de uso estabelecida "
                    + @"\neste contrato."
                    + @"\Cláusula 4a - A mercadoria alugada veverá ser devolvida até as 18:00 (Dezoito) horas da data estabelecida neste contrato, completa,"
                    + @"\                       tal como foi retirada."
                    + @"\Parágrafo Primeiro: Caso as mercadorias não sejam devolvidas na data prevista neste contrato sofrerão um acréscimo de R$ 10,00"
                    + @"\                       (Dez Reais) por dia útil de atraso, se esta condicão perdurar por 07 (sete) dias será cobrado uma nova taxa de"
                    + @"\                       Locação da mesma de acordo com o valor pago pelo LOCATÁRIO."
                    + @"\Parágrafo Segundo: Caso as mercadorias sejam devolvidas com manchas de gordura, graxa, tinta ou qualquer outro produto que as"
                    + @"\                       danifique, será cobrado uma taxa de R$ 30,00 (Trinta Reais)."
                    + @"\Cláusula 5a - Em caso de troca das mercadorias objeto do presente contrato será cobrado taxa de R$ 20,00 referente"
                    + @"\                       aos Serviços Executados."
                    + @"\Parágrafo Único: Não é permitido a troca das Mercadorias, após 05 (cinco) dias corridos da data de locação."
                    + @"\Cláusula 6a - Em caso de desistência por parte do LOCATÁRIO, o valor pago a título de Locação não será devolvido nem transferido"
                    + @"\                       a outras pessoas."
                    + @"\Cláusula 7a - As mercadorias, objeto da referida locação não poderão ser emprestadas ou tranferidas a outras pessoas."
                    + @"\Cláusula 8a - O LOCATÁRIO se compromete a ressarcir a LOCADORA pelo valor de tabela de Mercado, vigente na data do evento,"
                    + @"\                       pelo extravio ou dano nas mercadorias objeto deste contrato, bem como seu uso indevido."
                    + @"\Cláusula 9a - A LOCADORA se compromete a entregar a Mercadoria lavada e passada, com os devidos ajustes solicitados pelo"
                    + @"\                      LOCATÁRIO e em perfeito estado de conservação de acordo como foi requirido durante a prova na data deste contrato."
                    + @"\Cláusula 10a - Caso seja constatada alguma denificação na Mercadoria locada no momento da retirada pelo LOCATÁRIO, a"
                    + @"\                       LOCADORA se compromete  a efetuar a substituição, ou a troca, independente do seu preço de locação, ou  a devida"
                    + @"\                       devolução do valor pago, tudo conforme a disponibilidade do produto ou conveniência da LOCADORA."
                    + @"\Cláusula 11a - E por estarem juntos e acordados, firma o presente contrato, ficando eleito o Fórum desta Comarca para dirimir"
                    + @"\                       quaisquer dúvidas que possam surgir."
                    + @"\<linha>"
                    + @"\<Cidade>, <hoje>"
                    + @"\"
                    + @"\                                                                                                         ______________________________________"
                    + @"\                                                                                                         <nome>"
                    + @"\"
                    + @"\É OBRIGATÓRIO APRESENTAR ESTE CONTRATO NA RETIRADA!";
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\nmsystem\settings", "confClausulasTb", padrao, RegistryValueKind.String);
                    result.Add("confClausulasTb", padrao);
                }
                return result;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }

        }
        */
        
    }
}
