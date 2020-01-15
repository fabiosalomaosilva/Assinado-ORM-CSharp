using ArqOrm.Model;
using Arquivarnet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArqOrm.WFTeste
{
    public partial class Form1 : Form
    {
        string conexao = string.Empty;
        Guid id = Guid.Empty;
        PessoaFisica pes = new PessoaFisica();
        Telefone tel = new Telefone();

        public Form1()
        {
            InitializeComponent();/**
            using (Session db = new Session(Conexao.ConStr))
            {
                //List<PessoaFisica> lista = new List<PessoaFisica>();
                //lista = MontarLista();
               //db.EditarLista<PessoaFisica>(lista);
                Parametros p = new Parametros();
                //p.AddJoin("Telefones");
                //var pessoas = db.ListarTudo<PessoaFisica>();
                //gvPessoas.DataSource = pessoas;
                //var con = pes.Primeiro(s => s.Nome, "62");
                //MessageBox.Show(con.ToString());
            }
                                   * **/
        }

        private List<PessoaFisica> MontarLista()
        {
            List<PessoaFisica> lista = new List<PessoaFisica>();
            PessoaFisica pes1 = new PessoaFisica() 
            {
                Nome = "Adriana Salomão",
                Idade = 34,
                DataCriacao = DateTime.Now,
            };
            PessoaFisica pes2 = new PessoaFisica()
            {
                Nome = "Karina Vogth",
                Idade = 25,
                DataCriacao = DateTime.Now,
            };

            lista.Add(pes1);
            lista.Add(pes2);
            return lista;
        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            PessoaFisica pf = new PessoaFisica();
            pf.Nome = txtNome.Text;
            pf.NaturezaJuridica = (NaturezaJuridica)txtNaturezaJuridica.SelectedItem;
            if (txtIdade.Text != "")
            {
                pf.Idade = Convert.ToInt32(txtIdade.Text);
            }
            if (txtDataCriacao.Text != "")
            {
                pf.DataCriacao = Convert.ToDateTime(txtDataCriacao.Text);
            }


            using (Session db = new Session(Conexao.ConStr))
            {
                db.Inserir(pf);
                gvPessoas.DataSource = db.ListarTudo<PessoaFisica>();
            }
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            PessoaFisica pf = new PessoaFisica();
            pf.ID = int.Parse(txtID.Text);
            pf.Nome = txtNome.Text;
            pf.Idade = Convert.ToInt32(txtIdade.Text);
            pf.DataCriacao = Convert.ToDateTime(txtDataCriacao.Text);
            pf.NaturezaJuridica = (NaturezaJuridica)txtNaturezaJuridica.SelectedItem;

            using (Session db = new Session(Conexao.ConStr))
            {
                db.Editar<PessoaFisica>(pf);
                gvPessoas.DataSource = db.ListarTudo<PessoaFisica>();
            }
        }

        private void gvPessoas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = gvPessoas.Rows[e.RowIndex].Cells[6].Value.ToString();
            txtNome.Text = gvPessoas.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtIdade.Text = gvPessoas.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtDataCriacao.Text = gvPessoas.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtNaturezaJuridica.SelectedItem = gvPessoas.Rows[e.RowIndex].Cells[3].Value;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            PessoaFisica pf = new PessoaFisica();
            pf.ID = int.Parse(txtID.Text);
            pf.Nome = txtNome.Text;
            pf.Idade = Convert.ToInt32(txtIdade.Text);
            pf.DataCriacao = Convert.ToDateTime(txtDataCriacao.Text);

            using (Session db = new Session(Conexao.ConStr))
            {
                db.Excluir<PessoaFisica>(pf);
                gvPessoas.DataSource = db.ListarTudo<PessoaFisica>();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (Session db = new Session(Conexao.ConStr))
            {
                //Parametros param = new Parametros();
                //param.AddCriterio("Numero", 99891959);

                var pess = db.ListarTudo<PessoaFisica>();
                gvPessoas.DataSource = pess;

                //var tel = pess.PessoaFisica.Nome;
                //MessageBox.Show(tel.ToString());
            }
            var listaNat = Enum.GetValues(typeof(NaturezaJuridica));
            txtNaturezaJuridica.DataSource = listaNat;
        }

        private void btnPesquisa_Click(object sender, EventArgs e)
        {
            using (Session db = new Session(Conexao.ConStr))
            {
                Parametros c = new Parametros();
                var pess = db.PesquisaFullTextCriterios<PessoaFisica>(txtPesquisa.Text, c);
                gvPessoas.DataSource = pess;

                var tel = pess.FirstOrDefault();
                MessageBox.Show(tel.ToString());
            }
        }
    }
}
