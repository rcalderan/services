﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Services
{
    public partial class PrincipalForm : Form
    {
        private bool DebuMode = true;

        private Conexao conexao;

        private Panel[] panels;

        private User activeUser;

        public PrincipalForm()
        {
            InitializeComponent();
            List<Panel> p = new List<Panel>();
            foreach (Control c in conPn.Parent.Controls)
                if (c is Panel)
                    p.Add((Panel)c);
            panels = p.ToArray();
            DatabaseCheck();
        }

        private void menuAccess(Privilegio pr)
        {
            Dictionary<ToolStripMenuItem, int> menusPorPriv = new Dictionary<ToolStripMenuItem, int>()
            {
                //Admin
                {cad1Mi,0},{confMi,0},
                //Adv
                {statMi,1},{statOrdMi,1},{EquipMi,1},{aceMi,1},{pecasMi,1},
                //comun->todo
                {ordSetMi,2},{pessMi,2},{usuMi,2},{locaisMi,2},{setoresMi,2},{servicosMi,2},{emitirOrdemMi,2},{appMi,2},{sobreMi,2}
            };
            foreach (KeyValuePair<ToolStripMenuItem, int> pair in menusPorPriv)
            {
                if (pair.Value >= pr.id)
                    pair.Key.Visible = true;
                else
                    pair.Key.Visible = false;
            }
        }

        private void mostraPainel(Panel pn)
        {
            foreach (Panel p in panels)
                if (pn == p)
                {
                    pn.Show();
                    pn.BringToFront();
                }
                else
                    p.Hide();
        }

        private void DatabaseCheck()
        {
            try
            {
                if (Conexao.noServerDatabase)
                {//usar sqlite
                    conexao = new Conexao();
                    if (conexao.getLastLoadState())
                    {
                        if (conexao.Check())
                        {
                            if (activeUser == null)
                            {
                                usuTrocaPn.Show();
                            }
                            else
                                habilitaControles(true);
                        }
                        else
                            habilitaControles(false);

                    }
                    else
                    {
                        habilitaControles(false);
                        MessageBox.Show("nao foi possivel conectar ao banco de dados");
                    }
                }
                else
                {
                    conexao = new Conexao();
                    if (!conexao.getLastLoadState())
                    {
                        habilitaControles(false);
                        conPn.Show();
                        conPn.BringToFront();
                    }
                    else
                        habilitaControles(true);
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.Message);
                habilitaControles(false);
                conPn.Show();
            }
        }

        private void habilitaControles(bool state)
        {
            menuPrincipal.Enabled = state;
            if (!state)
            {
                foreach (Panel p in panels)
                    p.Hide();
            }
        }

        private void conConectaBt_Click(object sender, EventArgs e)
        {
            conexao = new Conexao(conServTb.Text,conUserTb.Text,conPass1Tb.Text);
            if (!conexao.getLastLoadState())
            {
                MessageBox.Show("Não foi possível conectar. Tente de novo.");
            }
            else
            {
                conPn.Hide();
                habilitaControles(true);
            }
        }

        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mostraPainel(usuTrocaPn);
        }

        private void pesXLb_Click(object sender, EventArgs e)
        {
            cad1Pn.Hide();
        }

        private void setXLb_Click(object sender, EventArgs e)
        {
            setPn.Hide();
        }

        private void setoresToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mostraPainel(setPn);
        }

        private void ordXLb_Click(object sender, EventArgs e)
        {
            emiPn.Hide();
        }

        private void passarOrdemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mostraPainel(emiPn);
        }

        private void usuTrocaPn_VisibleChanged(object sender, EventArgs e)
        {
            if (usuTrocaPn.Visible)
                centralizarControl(usuTrocaPn);
        }

        private void usuXLb_Click(object sender, EventArgs e)
        {
            if (menuPrincipal.Enabled)
                usuTrocaPn.Hide();
        }

        private void usuUserTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                usuPassTb.Select();
            }
        }

        private bool Logout()
        {
            try
            {
                activeUser = null;
                habilitaControles(false);
                usuTrocaPn.Show();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Login(string user, string pass)
        {
            try
            {
                DataTable dt = conexao.Query("select * from user where login='" + user + "' and pass='" + pass + "'");
                if (dt == null)
                    return false;
                else
                {
                    habilitaControles(true);
                    activeUser = new User(conexao);
                    activeUser.Id = int.Parse(dt.Rows[0]["id"].ToString());
                    activeUser.Login = dt.Rows[0]["login"].ToString();
                    activeUser.Nome = dt.Rows[0]["nome"].ToString();
                    activeUser.Pass = dt.Rows[0]["pass"].ToString();
                    activeUser.UltimoAcesso = DateTime.Now;
                    activeUser.Setor = new Setor(int.Parse(dt.Rows[0]["setor"].ToString()));
                    activeUser.Privilegio = new Privilegio(int.Parse(dt.Rows[0]["privilegio"].ToString()));
                    menuAccess(activeUser.Privilegio);
                    if (activeUser.Save())
                    {
                        emiUsuSolLb.Text = activeUser.Nome;
                        return true;
                    }
                    else
                    {
                        activeUser = null;
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void usuPassTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                if (Login(usuUserTb.Text,usuPassTb.Text))
                {
                    usuTrocaPn.Hide();
                    usuUserTb.Clear();
                    usuPassTb.Clear();
                    mostraPainel(servPn);
                }
                else
                    MessageBox.Show("Usuario ou senha incorreta.");
            }
        }

        private void conPn_VisibleChanged(object sender, EventArgs e)
        {
            if (conPn.Visible)
                centralizarControl(conPn);
        }

        private void cad1Pn_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                centralizarControl(cad1Pn);
                if (cad1PrivilegiosCb.Items.Count == 0)
                    foreach (Privilegio p in Privilegio.All)
                        cad1PrivilegiosCb.Items.Add(p.id.ToString() + " - " + p.nome);
                cad1SetorCb.Items.Clear();
                DataTable dt = conexao.Query("select * from setor");
                if (dt!=null)
                {
                    foreach (DataRow r in dt.Rows)
                        cad1SetorCb.Items.Add(r["id"].ToString()+" - " +r["nome"].ToString());
                }

            }
            catch { }
        }

        private void cad1LocalizarTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                MessageBox.Show("Não Implementado...");
            }
        }

        private void cad1LocXLb_Click(object sender, EventArgs e)
        {
            cad1LocalizarPn.Hide();
            cad1LocalizarTb.Clear();
        }

        private void cad1LocalizarLb_Click(object sender, EventArgs e)
        {
            cad1LocalizarPn.Show();
        }

        private void cad1LocalizarPn_VisibleChanged(object sender, EventArgs e)
        {
            if (cad1LocalizarPn.Visible)
                centralizarControl(cad1LocalizarPn);
        }
        private void centralizarControl(Control c)
        {
            c.Location = new Point(c.Parent.Width / 2 - c.Width / 2, c.Parent.Height / 2 - c.Height / 2);
        }

        private void cad1LpLb_Click(object sender, EventArgs e)
        {
            limpaCad1();
        }
        private void limpaCad1()
        {
            cad1NomeTb.Clear();
            cad1LoginTb.Clear();
            cad1LoginTb.Enabled = true;
            cad1Pass1Tb.Clear();
            cad1Pass1Tb.Enabled = true;
            cad1Pass2Tb.Clear();
            cad1Pass2Tb.Enabled = true;
            cad1IdLb.Text = "";
            cad1PrivilegiosCb.SelectedIndex = 2;
        }

        private void cad1Mi_Click(object sender, EventArgs e)
        {
            mostraPainel(cad1Pn);
        }

        private void carregaUsuario(int id)
        {
            User novo = User.Load(id);
            cad1NomeTb.Text = novo.Nome;
            cad1PrivilegiosCb.SelectedItem = novo.Privilegio.id;
            cad1LoginTb.Text = novo.Login;
            cad1LoginTb.Enabled = false;
            cad1Pass1Tb.Text = novo.Pass;
        }

        private void cad1SaveBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (cad1Pass1Tb.Text!=cad1Pass2Tb.Text)
                {
                    MessageBox.Show("Senhas não conferem. Digite as senhas iguais!");
                    return;
                }
                User novo = new User(conexao);
                novo.Nome = cad1NomeTb.Text;
                int i,index =cad1PrivilegiosCb.Text.IndexOf(" - ");

                if (index != -1)
                {
                    if (int.TryParse(cad1PrivilegiosCb.Text.Substring(0, index), out i))
                        novo.Privilegio = new Privilegio(i);
                }else
                {
                    MessageBox.Show("Selecione um privilégio válido.");
                    return;
                }
                novo.Login = cad1LoginTb.Text;
                novo.Pass = cad1Pass1Tb.Text;
                novo.UltimoAcesso = DateTime.Now;
                if (novo.Save())
                    MessageBox.Show("Novo usuario: \"" + novo.Nome + "\" adicionado!");
                else
                    MessageBox.Show("Não foi possível adicionar usuario.");
            }
            catch
            {

            }
        }

        private void statMi_Click(object sender, EventArgs e)
        {

        }

        private void confMi_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Não Implementado...");
        }

        private void sobreMi_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Richard Calderan: Freelancer.\nContato: www.workana.com/w/richard-calderan ");
        }

        private void cad1ExcluirBt_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Deseja exluir Usuario? \"" + cad1NomeTb.Text + "\" Será apagado do sistema. Continuar?", "Excluir", MessageBoxButtons.YesNo))
            {
                MessageBox.Show("Ainda não implementado.");
            }
        }

        private void cad1PrivilegiosCb_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void setPn_VisibleChanged(object sender, EventArgs e)
        {
            if (setPn.Visible)
            {
                centralizarControl(setPn);
                DataTable dt = conexao.Query("select * from setor");
                if (dt!=null)
                {
                    setCb.Items.Clear();
                    foreach(DataRow r in dt.Rows)
                    {
                        setCb.Items.Add(r["id"].ToString() + " - " + r["nome"].ToString());
                    }
                }
            }
        }

        private void setExcluiBt_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Não Implementado.");
        }

        private void setSaveBt_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = Setor.New(setNomeTb.Text);
                if (msg == "")
                {
                    MessageBox.Show("Novo setor adicionado com sucesso");
                }
                else
                    MessageBox.Show(msg);
            }
            catch
            {

            }
        }
        private void limpaSetor()
        {
            try
            {
            setNomeTb.Clear();
            setCb.Items.Clear();
            DataTable dt = conexao.Query("select * from setor");
            if (dt != null)
                setCb.Items.Add(dt.Rows[0]["id"].ToString() + " - " + dt.Rows[0]["nome"].ToString());
            }
            catch
            {}
        }

        private void setCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = setCb.Text.IndexOf(" - ");
                if (index!=-1)
                {
                    string nome = "";
                    setIdLb.Text = setCb.Text.Substring(0, index);
                    if (setCb.Text.Length>4)
                    {
                        setNomeTb.Text = setCb.Text.Substring(index + 3, setCb.Text.Length - (3 + index));
                    }

                }
                else
                {
                    int selectIndex = setCb.SelectedIndex;
                    DataTable dt = conexao.Query("select * from setor");
                    if (dt != null)
                    {
                        setCb.Items.Clear();
                        foreach (DataRow r in dt.Rows)
                        {
                            setCb.Items.Add(r["id"].ToString() + " - " + r["nome"].ToString());
                        }
                    }
                    else
                        selectIndex = 0;
                }
            }
            catch
            {

            }
        }

        private void pnXLb_Click(object sender, EventArgs e)
        {
            statOrdPn.Hide();
        }

        private void statOrdMi_Click(object sender, EventArgs e)
        {
            mostraPainel(statOrdPn);
        }

        private void statOrdPn_VisibleChanged(object sender, EventArgs e)
        {
            if (statOrdPn.Visible)
                centralizarControl(statOrdPn);
        }


        private void emiFinCh_CheckedChanged(object sender, EventArgs e)
        {
            emiOrcamentoLv.Enabled = emiFinCh.Checked;
            emiOrcaAddLb.Visible = emiFinCh.Checked;
        }

        private void emiAddItLb_Click(object sender, EventArgs e)
        {
            if (!emiaddPn.Visible)
            {
                limpaAddItem();
                emiAddBt.Text = "Adicionar";
                emiaddPn.Show();
                emiAddIdLb.Text = (emiItemLv.Items.Count + 1).ToString();
            }
            else
                emiaddPn.Hide();
        }
        private void emiOrcaAddItLb_Click(object sender, EventArgs e)
        {
            if (!emiOrcaAddPn.Visible)
            {
                limpaOrcaItem();
                emiOrcaAddBt.Text = "Adicionar";
                emiOrcaAddPn.Show();
                emiOrcaIdLb.Text = (emiOrcamentoLv.Items.Count + 1).ToString();
            }
            else
                emiOrcaAddPn.Hide();
        }

        private void emiAddBt_Click(object sender, EventArgs e)
        {
            try
            {
                //add ou remover?

                if (emiAddBt.Text == "Remover")
                {
                    emiItemLv.Items.RemoveAt(emiItemLv.FocusedItem.Index);
                    limpaAddItem();
                    emiaddPn.Hide();
                    return;
                }
                if (emiAddItNup.Value == 0)
                {
                    MessageBox.Show("Informe a quantidade.");
                    return;
                }
                if (string.IsNullOrEmpty(emiAddDescTb.Text))
                {
                    MessageBox.Show("Adicione algo a \"Descrição\" do item.");
                    return;
                }
                int id = emiItemLv.Items.Count + 1;

                string[] subs = { id.ToString(), emiAddItNup.Value.ToString(), emiAddDescTb.Text, emiAddMarcaTb.Text, emiAddModeloTb.Text };
                ListViewItem lv = new ListViewItem(subs);
                emiItemLv.Items.Add(lv);

                limpaAddItem();
                emiaddPn.Hide();
            }
            catch(Exception erro)
            {
                if (DebuMode)
                    MessageBox.Show(erro.Message);
            }
        }
        private void emiOrcaAddBt_Click(object sender, EventArgs e)
        {
            try
            {
                //add ou remover?

                if (emiOrcaAddBt.Text == "Remover")
                {
                    emiOrcamentoLv.Items.RemoveAt(emiOrcamentoLv.FocusedItem.Index);
                    limpaOrcaItem();
                    emiOrcaAddPn.Hide();
                    return;
                }
                if (emiOrcaNup.Value == 0)
                {
                    MessageBox.Show("Informe a quantidade.");
                    return;
                }
                if (string.IsNullOrEmpty(emiOrcaDescTb.Text))
                {
                    MessageBox.Show("Adicione algo a \"Descrição\" do item.");
                    return;
                }
                int id = emiItemLv.Items.Count + 1;
                string[] subs = { id.ToString(), emiOrcaNup.Value.ToString(), emiOrcaDescTb.Text, emiOrcaValorTb.Text };
                
                float auxF;
                if (float.TryParse(emiOrcaValorTb.Text, out auxF))
                    subs[3] = auxF.ToString("F2");
                else
                    subs[3] = "0,00";

                ListViewItem lv = new ListViewItem(subs);
                emiOrcamentoLv.Items.Add(lv);
                limpaOrcaItem();
                emiOrcaAddPn.Hide();
            }
            catch (Exception erro)
            {
                if (DebuMode)
                    MessageBox.Show(erro.Message);
            }
        }

        private void limpaAddItem()
        {
            emiAddDescTb.Clear();
            emiAddItNup.Value = 0;
            emiAddMarcaTb.Clear();
            emiAddModeloTb.Clear();
            emiAddDescTb.Clear();
            emiAddIdLb.Text = "0";
        }
        private void limpaOrcaItem()
        {
            emiOrcaDescTb.Clear();
            emiOrcaNup.Value = 0;
            emiOrcaValorTb.Text = "0,00";
            emiAddModeloTb.Clear();
            emiAddDescTb.Clear();
            emiOrcaIdLb.Text = "0";
        }

        private void emiAddXLb_Click(object sender, EventArgs e)
        {
            limpaAddItem();
            emiaddPn.Hide();
        }

        private void emiOrcaXLb_Click(object sender, EventArgs e)
        {
            limpaOrcaItem();
            emiOrcaAddPn.Hide();
        }

        private void emiItemLv_Click(object sender, EventArgs e)
        {
            try
            {
                limpaAddItem();
                emiAddIdLb.Text = emiItemLv.FocusedItem.SubItems[0].Text;
                int auxI;
                if (int.TryParse(emiItemLv.FocusedItem.SubItems[1].Text, out auxI))
                    emiAddItNup.Value = auxI;
                else
                    emiAddItNup.Value = 0;
                emiAddDescTb.Text = emiItemLv.FocusedItem.SubItems[2].Text;
                emiAddMarcaTb.Text = emiItemLv.FocusedItem.SubItems[3].Text;
                emiAddModeloTb.Text = emiItemLv.FocusedItem.SubItems[4].Text;
                emiaddPn.Show();
                emiAddBt.Text = "Remover";
            }
            catch (Exception erro)
            {
                if (DebuMode)
                    MessageBox.Show(erro.Message);
            }
        }

        private void emiItemLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void emiPn_VisibleChanged(object sender, EventArgs e)
        {
            if (emiPn.Visible)
            {
                centralizarControl(emiPn);
                emiSetorCb.Items.Clear();
                emiUsuSolLb.Text = activeUser.Nome;
                emiSetorSolLb.Text = activeUser.Setor.Nome;
                emiPrioridadeCb.Items.Clear();
                foreach (Prioridade p in Prioridade.All)
                    emiPrioridadeCb.Items.Add(p.Id.ToString() + " - " + p.Nome);
                DataTable dt = conexao.Query("select * from setor");
                if (dt != null)
                    foreach (DataRow r in dt.Rows)
                        emiSetorCb.Items.Add(r["id"].ToString() + " - " + r["nome"].ToString());
                else
                    emiSetorCb.Enabled = false;
            }
        }

        private string getNameFromString(string s)
        {
            try
            {
                int index = s.IndexOf(" - ");
                if (index == -1)
                    return "";
                else
                    return s.Substring(index + 3, s.Length - (index + 3)); 
            }
            catch
            {
                return "";
            }
        }

        private int getIdFromString(string s)
        {
            try
            {
                int aux, index = s.IndexOf(" - ");
                if (index == -1)
                    return -1;
                else
                {
                    if (int.TryParse(s.Substring(0, index), out aux))
                        return aux;
                    else
                        return -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        private void emiEmiBt_Click(object sender, EventArgs e)
        {
            try
            {
                string setName = getNameFromString(emiSetorCb.Text);
                if (DialogResult.Yes == MessageBox.Show("Emitindo Ordem de Serviço de \""+emiTipoCb.Text+"\" para o setor :"+setName+"\n\nDeseja continuar?","Emitir Ordem de Serviço",MessageBoxButtons.YesNo))
                {
                    string[] aux =new string[0];
                    ListViewItem[] itens;
                    foreach
                        emiItemLv.Items
                    Service newService = Service.New(getIdFromString(emiTipoCb.Text),
                        getIdFromString(emiPrioridadeCb.Text),DateTime.Now,emiPrazoDtp.Value,Status.Pendente,
                        emiItemLv.Items,emiProbTb.Lines,aux,aux,
                        activeUser.Id,activeUser.Setor.Id,0,getIdFromString(emiSetorCb.Text));
                    if (null != newService)
                    {
                        if (DialogResult.Yes == MessageBox.Show("Ordem de serviço emitida com sucesso!\n\nGostaria de imprimir a Ordem de serviço?", "Imprimir?", MessageBoxButtons.YesNo))
                        {
                            MessageBox.Show("Não implementado!");
                        }
                    }
                    else
                        MessageBox.Show("Não foi possível emitir Ordem.");

                }

            }
            catch (Exception er) { if (DebuMode) MessageBox.Show(er.Message); }
        }

        private void servPn_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (servPn.Visible)
                {
                    centralizarControl(servPn);
                    servLv.Items.Clear();
                    //DataTable dt = conexao.Query("select s.id,u.id as solicitante,s.setorSol,s.hoje,s.prioridade,s.status from service as s inner join user as u where u.id=s.usuSol order by prazo");
                    DataTable dt = conexao.Query("select * from service order by prazo");
                    if (dt != null)
                    {
                        ListViewItem lv;
                        string[] subs;
                        DateTime auxDt;
                        foreach(DataRow r in dt.Rows)
                        {

                            subs = new string[7];
                            subs[0] = r["id"].ToString();
                            subs[1] = r["usuSol"].ToString();
                            subs[2] = r["setorSol"].ToString();
                            subs[3] = Service.Split(r["declarado"].ToString(), "<*>")[0];
                            if (DateTime.TryParse(r["hoje"].ToString(), out auxDt))
                                subs[4] = auxDt.ToString("dd/MM/yyyy HH:mm:ss");
                            subs[5] = new Prioridade(int.Parse(r["prioridade"].ToString())).Nome;
                            subs[6] = new Status(int.Parse(r["status"].ToString())).Nome; 
                            lv = new ListViewItem(subs);
                            servLv.Items.Add(lv);

                        }
                    }
                }
            }catch{ }
        }

        private void ordSetMi_Click(object sender, EventArgs e)
        {
            mostraPainel(servPn);
        }

        private void label40_Click(object sender, EventArgs e)
        {
            servPn.Hide();
        }

        private void PrincipalForm_Resize(object sender, EventArgs e)
        {
            foreach(Panel p in panels)
                if (p.Visible)
                {
                    centralizarControl(p);
                    break;
                }
        }


        private void servLv_Click(object sender, EventArgs e)
        {
            carregaOrdem(servLv.SelectedItems[0]);
        }

        private void carregaOrdem(ListViewItem lvi)
        {
            try
            {
                mostraPainel(emiPn);
                emiRecePn.Show();
                int id;
                if (!int.TryParse(lvi.SubItems[0].Text,out id))
                    MessageBox.Show("Não foi possivel carregar ordem.");
                else
                {
                    Service s = Service.Load(id);

                }

            }
            catch(Exception e)
            {
                if (DebuMode)
                    MessageBox.Show(e.Message);
            }
        }

        private void limpaOrdem()
        {
            try
            {
                limpaAddItem();
                emiaddPn.Hide();
                emiItemLv.Items.Clear();
                emiOrcamentoLv.Items.Clear();
                emiProbTb.Clear();
                emiPrazoDtp.Value = DateTime.Today;
                emiUsuSolLb.Text = activeUser.Nome;
                emiSetorSolLb.Text = activeUser.Setor.Nome;
                emiSolucaoTb.Clear();
                emiEncontradoTb.Clear();
                emiSolucaoCh.Checked = false;
                emiEncontradoCh.Checked = false;
                emiOrcaAddLb.Visible = false;
                limpaOrcaItem();
            }
            catch(Exception e)
            {
                if (DebuMode)
                    MessageBox.Show(e.Message);
            }
        }

        private void emiEncontradoCh_CheckedChanged(object sender, EventArgs e)
        {
            if (emiEncontradoCh.Checked)
                emiEncontradoTb.Show();
            else
                emiEncontradoTb.Hide();

        }

        private void emiSolucaoCh_CheckedChanged(object sender, EventArgs e)
        {
            if (emiSolucaoCh.Checked)
                emiSolucaoTb.Show();
            else
                emiSolucaoTb.Hide();
        }

        private void label42_Click(object sender, EventArgs e)
        {
            limpaOrdem();
        }

        private void emiImprimirBt_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Ainda não implementado.");
            }
            catch (Exception er){if (DebuMode) MessageBox.Show(er.Message);}
        }

        private void emiOrcaValorTb_Leave(object sender, EventArgs e)
        {
            try
            {
                float f;
                if (float.TryParse(emiOrcaValorTb.Text, out f))
                    emiOrcaValorTb.Text = f.ToString("F2");
                else
                    emiOrcaValorTb.Text = "0,00";
            }
            catch (Exception er){if (DebuMode) MessageBox.Show(er.Message);}
        }

        private void emiOrcaValorTb_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    float f;
                    if (float.TryParse(emiOrcaValorTb.Text, out f))
                        emiOrcaValorTb.Text = f.ToString("F2");
                    else
                        emiOrcaValorTb.Text = "0,00";
                }
            }
            catch (Exception er) { if (DebuMode) MessageBox.Show(er.Message); }
        }
    }
}
