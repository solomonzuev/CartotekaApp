using CartotekaApp.Domain;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CartotekaApp
{
    public partial class OrderEditorUC : UserControl
    {
        public OrderEditorUC()
        {
            InitializeComponent();
        }

        private void PreviewNumbericInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"\d");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                // Загрузка BookEditorUC в DialogHost.DialogContent
                var bookEditorUC = new BookEditorUC
                {
                    DataContext = ((OrderEditorViewModel)DataContext).BookEditorDataContext
                };
                DHostMain.DialogContent = bookEditorUC;
            }
        }
    }
}
