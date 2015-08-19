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
    public partial class Principal : Form
    {

        private Conexao conexao;
        private SQLiteConnection sqliteCon;
        private Panel[] panels;

        private bool noServerDatabase = true;

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
                if (noServerDatabase)
                {//usar sqlite
                    sqliteCon = new SQLiteConnection(Conexao.sqliteConectString);
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
                    habilitaControles(true);
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

        private void conPn_VisibleChanged(object sender, EventArgs e)
        {
            if (conPn.Visible)
                conPn.Location = new Point(conPn.Parent.Width / 2 - conPn.Width / 2, conPn.Parent.Height / 2 - conPn.Height / 2);
        }
    }
}
