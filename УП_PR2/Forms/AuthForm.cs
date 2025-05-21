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
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        private void buttonAuth_Click(object sender, EventArgs e)
        {
            string login = txtUsername.Text;
            string password = txtPassword.Text;

            AuthService authService = new AuthService();
            int userId = authService.AuthUser(login, password);

            if (userId == -1)
            {
                MessageBox.Show("Неверный логин или пароль.");
                return;
            }

            MainForm mainForm = new MainForm(userId);
            mainForm.Show();
            this.Hide();
        }
    }
}
