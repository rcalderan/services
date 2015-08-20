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
            ordPn.Hide();
        }

        private void passarOrdemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mostraPainel(ordPn);
        }

        private void usuTrocaPn_VisibleChanged(object sender, EventArgs e)
        {
            if (usuTrocaPn.Visible)
                centralizarControl(usuTrocaPn);
        }

        private void usuXLb_Click(object sender, EventArgs e)
        {
            usuTrocaPn.Hide();
        }

        private void usuUserTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                usuSenhaTb.Select();
            }
        }

        private void usuSenhaTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                
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
                if (cad1PrivilegiosCb.Items.Count == 0)
                    foreach (Privilegio p in Privilegio.All)
                        cad1PrivilegiosCb.Items.Add(p.nome);

            }
            catch { }
        }

        private void cad1LocalizarTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
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
            
        }
        private void limpaCad1()
        {
            cad1IdLb.Text = "";
            cad1NomeTb.Clear();
            cad1Pass1Tb.Clear();
            cad1Pass2Tb.Clear();
        }
    }
}
