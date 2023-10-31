using PdfiumViewer;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CartotekaApp
{
    /// <summary>
    /// Логика взаимодействия для UserGuidePage.xaml
    /// </summary>
    public partial class UserGuidePage : Page
    {
        public UserGuidePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string pathToPdf = $"{Environment.CurrentDirectory}\\Resources\\UserGuide.pdf";
            PdfViewer.Document = PdfDocument.Load(pathToPdf);
        }
    }
}
