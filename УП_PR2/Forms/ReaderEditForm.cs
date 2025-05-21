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
        
    public partial class ReaderEditForm : Form
    {
        private readonly string connectionString = "Host=localhost;Database=library;Username=postgres;Password=123;Port=5433";
        private readonly int userId;
        private readonly List<TextBox> requiredTextBoxes = new List<TextBox>();

        public ReaderEditForm()
        {
            InitializeComponent();
            LoadData();
            ConfigureDataGridView();
        }

        private void LoadData()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM reader ORDER BY name";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            DataTable dt = new DataTable();
                            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                            adapter.Fill(dt);

                            dataGridViewReaders.DataSource = dt;

                            dataGridViewReaders.Columns["reader_id"].Visible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Конфигурация DataGridView
        private void ConfigureDataGridView()
        {
            dataGridViewReaders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewReaders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewReaders.MultiSelect = false;
            dataGridViewReaders.ReadOnly = true;

            dataGridViewReaders.Columns["name"].HeaderText = "ФИО";
            dataGridViewReaders.Columns["date_of_birth"].HeaderText = "Дата рождения";
            dataGridViewReaders.Columns["work_place"].HeaderText = "Место работы";
            dataGridViewReaders.Columns["post"].HeaderText = "Должность";
            dataGridViewReaders.Columns["phone"].HeaderText = "Номер телефона";
            dataGridViewReaders.Columns["address"].HeaderText = "Адрес";
            dataGridViewReaders.Columns["email"].HeaderText = "Электронная почта";         
        }

        private void dataGridViewPartners_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewReaders.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewReaders.SelectedRows[0];

                txtName.Text = row.Cells["name"].Value.ToString();
                txtDateOfBirth.Text = row.Cells["date_of_birth"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                txtPhone.Text = row.Cells["phone"].Value.ToString();
                txtAddress.Text = row.Cells["address"].Value.ToString();
                txtWorkPlace.Text = row.Cells["work_place"].Value.ToString();
                txtPost.Text = row.Cells["post"].Value.ToString();
            }
        }

        private void ReaderEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
