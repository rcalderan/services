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

        public Principal()
        {
            InitializeComponent();
        }

        private void DatabaseCheck()
        {
            try
            {
                conexao = new Conexao();
                if (!conexao.getLastLoadState())
                {
                    habilitaControles(false);
                    conexaoPn.Show();
                }
                else
                    habilitaControles(true);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                habilitaControles(false);
                conexaoPn.Show();
            }
        }

        private void habilitaControles(bool state)
        {

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
                conexaoPn.Hide();
                habilitaControles(true);
                
            }
        }
    }
}
