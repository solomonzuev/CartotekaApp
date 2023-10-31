using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CartotekaApp
{
    /// <summary>
    /// Логика взаимодействия для AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionTextBlock.Text = $"Версия: {version}";
        }
    }
}
