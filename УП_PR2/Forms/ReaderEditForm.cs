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
using УП_PR2.Forms;

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

        private void ReaderEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void dataGridViewReaders_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewReaders.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewReaders.SelectedRows[0];

                txtName.Text = row.Cells["name"].Value.ToString();
                dateTimePicker1.Text = row.Cells["date_of_birth"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                txtPhone.Text = row.Cells["phone"].Value.ToString();
                txtAddress.Text = row.Cells["address"].Value.ToString();
                txtWorkPlace.Text = row.Cells["work_place"].Value.ToString();
                txtPost.Text = row.Cells["post"].Value.ToString();
            }
        }

        private void buttonEditReader_Click(object sender, EventArgs e)
        {
            if (dataGridViewReaders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите партнёра для редактирования!");
                return;
            }

            int readerId = Convert.ToInt32(dataGridViewReaders.SelectedRows[0].Cells["reader_id"].Value);

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE reader 
                        SET 
                            name = @Name,
                            date_of_birth = @DateOfBirth,
                            work_place = @WorkPlace,
                            post = @Post,
                            email = @Email,
                            phone = @Phone,
                            address = @Address
                          WHERE reader_id = @ReaderId";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReaderId", readerId);
                        command.Parameters.AddWithValue("@Name", txtName.Text);
                        command.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@WorkPlace", txtWorkPlace.Text);
                        command.Parameters.AddWithValue("@Post", txtPost.Text);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);
                        command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                        command.Parameters.AddWithValue("@Address", txtAddress.Text);

                        int rowsAffected = command.ExecuteNonQuery();

                        foreach (var textBox in requiredTextBoxes)
                        {
                            textBox.Focus();
                            this.ValidateChildren();
                        }

                        bool hasErrors = requiredTextBoxes.Any(t => !string.IsNullOrEmpty(errorProvider1.GetError(t)));

                        if (hasErrors)
                        {
                            MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные обновлены!", "Успех");
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorProvider1.SetError(textBox, "Поле обязательно для заполнения");
            }
            else
            {
                errorProvider1.SetError(textBox, "");
            }
        }

        private void ReaderEditForm_Load(object sender, EventArgs e)
        {
            requiredTextBoxes.Add(txtName);

            foreach (var textBox in requiredTextBoxes)
            {
                textBox.Validating += TextBox_Validating;
            }
        }

        private void buttonAddReader_Click(object sender, EventArgs e)
        {
            ReaderAddForm readerAddForm = new ReaderAddForm();
            readerAddForm.Show();
            this.Hide();
        }

        private void buttonMenu_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm(userId);
            mainForm.Show();
            this.Hide();
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Length >= 16 && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
    }
}