using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Ticketmaster.Utilities;

namespace TicketmasterDesktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            // 🔐 1. Validate Inputs
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email.", "Input Error");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Input Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Input Error");
                return;
            }

            // ✅ 2. Attempt Login (now that inputs are good)
            var employee = await App.DbContext.Employee.FirstOrDefaultAsync(e => e.Email == email);
            if (employee != null)
            {
                
                var result = EmployeePasswordHasher.VerifyPassword( employee.Pword, password);
                if (result)
                {
                    MessageBox.Show($"✅ Welcome, {employee.FirstName}!", "Login Successful");
                    var projectList = new ProjectListWindow(employee.Id);
                    projectList.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("❌ Incorrect password.", "Login Failed");
                }
            }
            else
            {
                MessageBox.Show("❌ No user found with that email.", "Login Failed");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}