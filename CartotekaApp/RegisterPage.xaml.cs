using CartotekaApp.Domain;
using System.Windows;
using System.Windows.Controls;

namespace CartotekaApp
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((RegisterViewModel)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((RegisterViewModel)DataContext).ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
