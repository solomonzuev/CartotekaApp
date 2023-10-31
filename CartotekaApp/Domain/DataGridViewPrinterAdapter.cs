using CartotekaApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace CartotekaApp.Domain
{
    public class DataGridViewPrinterAdapter
    {
        private readonly DataGridView _dataGridView;
        private readonly DataGridViewPrinter _dgvPrinter;
        private readonly PrintDocument _pDoc;

        public DataGridViewPrinterAdapter(ObservableCollection<Order> orders)
        {
            // Создание DataGrid
            _dataGridView = new DataGridView();

            _dataGridView.Columns.Add("№", "№");
            _dataGridView.Columns.Add("Отд.", "Отд.");
            _dataGridView.Columns.Add("Название, автор", "Название, автор");
            _dataGridView.Columns.Add("Категория", "Категория");
            _dataGridView.Columns.Add("Год закупки", "Год закупки");
            _dataGridView.Columns.Add("Цена", "Цена");
            _dataGridView.Columns.Add("Кол-во", "Кол-во");
            _dataGridView.Columns.Add("Группы", "Группы");

            // Установка необходимых свойств
            _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            _dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            foreach (DataGridViewColumn column in _dataGridView.Columns)
            {
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }

            // Заполнение данных
            for (int i = 0; i < orders.Count; i++)
            {
                
                _dataGridView.Rows.Add(new object[] {
                    i.ToString(),
                    orders[i].Book.Otdel,
                    orders[i].Book.FullName,
                    orders[i].Book.Category.CategoryName,
                    orders[i].OrderYear.ToString(),
                    orders[i].Price.ToString("F2"),
                    orders[i].Units.ToString(),
                    orders[i].Book.GroupsText });

                _dataGridView.Size = new System.Drawing.Size(800, 500);
            }

            // Настройка для печати
            _pDoc = new PrintDocument();

            _pDoc.DefaultPageSettings.Landscape = false;
            _pDoc.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);

            _pDoc.PrintPage += (sender, e) =>
            {
                bool more = _dgvPrinter.DrawDataGridView(e.Graphics);
                if (more)
                    e.HasMorePages = true;
            };

            _dgvPrinter = new DataGridViewPrinter(_dataGridView, _pDoc);
        }

        /// <summary>
        /// Выполняет вывод на печать данных
        /// </summary>
        public void Print()
        {
            var printPreview = new PrintPreviewDialog
            {
                Width = 600,
                Height = 800,
                Document = _pDoc
            };
            printPreview.ShowDialog();
        }
    }
}
