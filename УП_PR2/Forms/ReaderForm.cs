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
    public partial class ReaderForm : Form
    {
        public ReaderForm()
        {
            InitializeComponent();
        }

        private void ReaderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
