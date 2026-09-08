using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System_Design
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter username and password.",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Sample login credentials
            if (username == "admin" && password == "12345")
            {
                MessageBox.Show("Login Successful!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Dashboard dashboard = new Dashboard();
                dashboard.Show();

                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password!",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
        

        