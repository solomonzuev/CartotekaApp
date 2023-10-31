using CartotekaApp.Domain;
using CartotekaApp.Models;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CartotekaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Если выполнено нажатие на MenuItem - закрываем боковое меню
            var dependencyObject = Mouse.Captured as DependencyObject;

            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((MainWindowViewModel)DataContext).CloseWindowRequested += MainWindow_CloseWindowRequested;
            }
        }

        private void MainWindow_CloseWindowRequested(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
