using CartotekaApp.Models;
using System.Windows;

namespace CartotekaApp
{
    /// <summary>
    /// Логика взаимодействия для BookPrintView.xaml
    /// </summary>
    public partial class BookPrintView : Window
    {
        public BookPrintView(Book book)
        {
            InitializeComponent();
            DataContext = book;
        }
    }
}
