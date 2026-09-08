using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System_Design;

namespace StudentSystem
{
    public partial class Form1 : Form
    {
        private bool rememberMe;

        public Form1()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            string username = textBox2.Text.Trim();
            string password = textBox1.Text;

            if (username.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Please enter both username and password.",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User user = new UserRepository().ValidateCredentials(username, password);

                if (user == null)
                {
                    MessageBox.Show("Invalid username or password.",
                        "Login",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                Session.Start(user);
                if (rememberMe)
                {
                    RememberMe.Save(user.Username);
                }

                this.Hide();
                new Dashboard().Show();
                Application.Exit();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Unable to reach the database. Please make sure MySQL is running and try again.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error occurred. Please try again.",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            rememberMe = !rememberMe;

            if (rememberMe)
            {
                label11.BackColor = Color.LightSkyBlue;
                label11.ForeColor = Color.Black;
            }
            else
            {
                label11.BackColor = SystemColors.ButtonHighlight;
                label11.ForeColor = SystemColors.ControlText;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.UseSystemPasswordChar = true;

            string rememberedUsername = RememberMe.Load();
            if (!string.IsNullOrEmpty(rememberedUsername))
            {
                textBox2.Text = rememberedUsername;
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Registration is not implemented yet.",
                "Register",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void label7_Click_1(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }
    }
}