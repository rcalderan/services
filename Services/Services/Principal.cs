using System;
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
                {cad1Mi,0},{statMi,0},{confMi,0},
                //Adv
                //comun->todo
                {pessMi,2},{usuMi,2},{locaisMi,2},{setoresMi,2},{servicosMi,2},{emitirOrdemMi,2},{appMi,2},{sobreMi,2}
            };
            string s,s2;
            ToolStripMenuItem aux;
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
                    activeUser.Privilegio = new Privilegio(int.Parse(dt.Rows[0]["privilegio"].ToString()));
                    menuAccess(activeUser.Privilegio);
                    if (activeUser.Save())
                    {
                        emiSolicitanteLb.Text = activeUser.Nome;
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

            MessageBox.Show("Não Implementado...");
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
                Setor set = new Setor(conexao);
                set.Nome = setNomeTb.Text;
                string msg = set.New();
                if (msg=="")
                    MessageBox.Show("Novo setor adicionado com sucesso");
                else
                    MessageBox.Show(msg);
            }
            catch
            {

            }
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
    }
}
