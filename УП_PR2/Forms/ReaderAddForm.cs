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
using System.Windows.Input;

namespace УП_PR2.Forms
{
    public partial class ReaderAddForm : Form
    {
        private readonly string connectionString = "Host=localhost;Database=library;Username=postgres;Password=123;Port=5433";
        private readonly int userId;
        private readonly List<TextBox> requiredTextBoxes = new List<TextBox>();

        public ReaderAddForm()
        {
            InitializeComponent();
        }

        private void buttonAddReader_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO reader
                        (name, date_of_birth, work_place, post, email, phone, address) 
                        VALUES 
                        (@Name, @DateOfBirth, @WorkPlace, @Post, @Email, @Phone, @Address)";


                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", txtName.Text);
                        command.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@WorkPlace", txtWorkPlace.Text);
                        command.Parameters.AddWithValue("@Post", txtPost.Text);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);
                        command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                        command.Parameters.AddWithValue("@Address", txtAddress.Text);

                        command.ExecuteNonQuery();

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
                    }
                }

                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка: Заполните данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtName.Text = "";
            dateTimePicker1.Value = DateTime.Today; txtWorkPlace.Text = "";
            txtPost.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
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

        private void ReaderAddForm_Load(object sender, EventArgs e)
        {
            requiredTextBoxes.Add(txtName);

            foreach (var textBox in requiredTextBoxes)
            {
                textBox.Validating += TextBox_Validating;
            }
        }

        private void buttonMenu_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm(userId);
            mainForm.Show();
            this.Hide();
        }

        private void ReaderAddForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
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
