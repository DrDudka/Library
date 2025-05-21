using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace УП_PR2
{
    public partial class MainForm : Form
    {
        private readonly string connectionString = "Host=localhost;Database=library;Username=postgres;Password=123;Port=5433";
        private readonly int userId;

        public MainForm(int userId)
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"select name, phone, author, book_name, v.total_count, date_of_receipt from vidacha as v
                    join reader as r on r.reader_id = v.reader_id
                    join book as b on b.book_id = v.book_id
                    WHERE v.date_of_return IS NULL 
                    AND CURRENT_DATE > (v.date_of_receipt + INTERVAL '14 days')";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Panel panel = CreatePartnerPanel(reader);
                        flowLayoutPanel1.Controls.Add(panel);
                    }
                }
            }
        }

        private Panel CreatePartnerPanel(NpgsqlDataReader reader)
        {
            Panel panel = new Panel()
            {
                Size = new Size(950, 150),
                BorderStyle = BorderStyle.Fixed3D,
                Margin = new Padding(10),
                BackColor = Color.FromArgb(0x64, 0x2B, 0x01),
                Cursor = Cursors.Hand
            };

            panel.Click += (s, e) => OpenReadersEditForm();

            Label ReaderAndPhone = new Label()
            {
                Location = new Point(10, 10),
                Text = reader["name"] + "/" + reader["phone"],
                AutoSize = true,
                Font = new Font("Times New Roman", 16),
            };

            Label Author = new Label()
            {
                Location = new Point(10, 40),
                Text = reader["author"] + "/" + reader["book_name"] + "/" + reader["total_count"] + "/" + reader["date_of_receipt"],
                AutoSize = true,
                Font = new Font("Times New Roman", 16),
            };

            panel.Controls.Add(ReaderAndPhone);
            panel.Controls.Add(Author);

            return panel;
        }

        private void OpenReadersEditForm()
        {
            ReaderEditForm editForm = new ReaderEditForm();
            editForm.FormClosed += (s, args) => LoadData();
            editForm.Show();
            this.Hide();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm(userId);
            mainForm.Show();
            this.Hide();
        }
    }
}
