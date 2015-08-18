using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Services
{
    public partial class Principal : Form
    {
        private Conexao conexao;
        private Panel[] panels;
        public Principal()
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
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
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
            {
                Point loc = new Point(usuTrocaPn.Parent.Width / 2 - usuTrocaPn.Width / 2, usuTrocaPn.Parent.Height / 2 - usuTrocaPn.Height / 2);
                usuTrocaPn.Location = loc;
            }
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
    }
}
