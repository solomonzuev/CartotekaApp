using CartotekaApp.Domain;
using CartotekaApp.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CartotekaApp
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            CreateAdmin();
        }

        private void CreateAdmin()
        {
            string HashPassword(string password)
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);

                using (var sha256 = SHA256.Create())
                {
                    var passwordHash = sha256.ComputeHash(passwordBytes);

                    var sb = new StringBuilder();

                    foreach (var b in passwordHash)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
            
            using (var context = new CartotekaDBEntities())
            {
                if (!context.Users.Any())
                {
                    var user = new User()
                    {
                        Imya = "Иван",
                        Familiya = "Иванов",
                        Otchestvo = "Сергеевич",
                        UserName = "admin",
                        PasswordHash = HashPassword("admin"),
                        Role = context.Roles.First(r => r.RoleName.Contains("админ"))
                    };

                    context.Users.Add(user);
                    context.SaveChanges();
                    MessageBox.Show("Создан администратор!");
                }
            }
        }

        private void ViewModel_CloseWindowRequested(object sender, System.EventArgs e)
        {
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((AuthWindowViewModel)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                DataContext = new AuthWindowViewModel(new CartotekaDBEntities()); 
            }

            ((AuthWindowViewModel)DataContext).CloseWindowRequested += ViewModel_CloseWindowRequested;
        }
    }
}
