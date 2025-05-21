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
    public partial class MainForm : Form
    {
        private readonly string connectionString = "Host=localhost;Database=library;Username=postgres;Password=123;Port=5433";
        private readonly int userId;

        public MainForm(int userId)
        {
            InitializeComponent();
            LoadData();
        }

        public static DateTime CalculateDueDate(DateTime issueDate)
        {
            return issueDate.AddDays(14); // Стандартный срок - 14 дней
        }

        public static int CalculateOverdueDays(DateTime dueDate)
        {
            return dueDate < DateTime.Today ? (DateTime.Today - dueDate).Days : 0;
        }

        private void LoadData()
        {
            try
            {
                flowLayoutPanel1.Controls.Clear();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT r.reader_id, r.name, r.phone, 
                                           b.author, b.book_name, v.total_count, 
                                           v.date_of_receipt
                                    FROM reader r
                                    JOIN vidacha v ON r.reader_id = v.reader_id
                                    JOIN book b ON v.book_id = b.book_id
                                    WHERE v.date_of_return IS NULL 
                                    AND CURRENT_DATE > (v.date_of_receipt + INTERVAL '14 days')
                                    ORDER BY r.name, v.date_of_receipt";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var currentReaderId = -1;
                        Panel currentPanel = null;

                        while (reader.Read())
                        {
                            int readerId = reader.GetInt32(0);
                            DateTime receiptDate = reader.GetDateTime(6);
                            DateTime dueDate = CalculateDueDate(receiptDate);
                            int overdueDays = CalculateOverdueDays(dueDate);

                            if (readerId != currentReaderId)
                            {
                                // Создаем новую панель для читателя
                                currentReaderId = readerId;
                                currentPanel = CreateReaderPanel(
                                    reader.GetString(1),
                                    reader.GetString(2));
                                flowLayoutPanel1.Controls.Add(currentPanel);
                            }

                            // Добавляем информацию о книге
                            AddBookInfo(currentPanel,
                                reader.GetString(3),
                                reader.GetString(4),
                                reader.GetString(5),
                                receiptDate,
                                overdueDays);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateReaderPanel(string readerName, string phone)
        {
            Panel panel = new Panel()
            {
                Size = new Size(950, 150),
                BorderStyle = BorderStyle.Fixed3D,
                Margin = new Padding(10),
                BackColor = Color.FromArgb(0x64, 0x2B, 0x01),
                Cursor = Cursors.Hand,
                Tag = readerName
            };

            panel.Click += (s, e) => OpenReaderEditForm();

            Label lblReader = new Label()
            {
                Location = new Point(10, 10),
                Text = $"{readerName}/{phone}",
                AutoSize = true,
                Font = new Font("Times New Roman", 16, FontStyle.Bold),
                ForeColor = Color.White
            };

            panel.Controls.Add(lblReader);
            return panel;
        }

        private void AddBookInfo(Panel panel, string author, string bookName,
                               string count, DateTime receiptDate, int overdueDays)
        {
            int y = panel.Controls.Count * 30 + 10;

            Label lblBook = new Label()
            {
                Location = new Point(20, y),
                Text = $"{author}/{bookName}/{count}/{receiptDate:dd.MM.yyyy}/{overdueDays} дней",
                AutoSize = true,
                Font = new Font("Times New Roman", 14),
                ForeColor = Color.White
            };

            panel.Height += 30; 
            panel.Controls.Add(lblBook);
        }

        private void OpenReaderEditForm()
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

        private void buttonAddPartner_Click(object sender, EventArgs e)
        {
            ReaderAddForm readerAddForm = new ReaderAddForm();
            readerAddForm.Show();
            this.Hide();
        }
    } 
}