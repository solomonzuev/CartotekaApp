using CartotekaApp.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartotekaApp.Domain
{
    // TODO - Сделать нормальную нумерацию строк
    // TODO - Сортировать список по отделению или по названию
    // TODO - Возможно сделать заголовок для страницы отчета

    public class WordReportGenerator
    {
        private readonly IList<Order> _orders;
        private readonly string _pathToSave;
        private const string REPORT_TITLE = "ОТЧЕТ ПО ЗАКУПКАМ КНИГ";

        public WordReportGenerator(IList<Order> orders, string pathToSave)
        {
            _orders = OrderedCopyFrom(orders);
            _pathToSave = pathToSave;
        }

        private IList<Order> OrderedCopyFrom(IList<Order> orders)
        {
            var orderedByFullName = orders.OrderBy(o => o.Book.FullName).ToList();
            return orderedByFullName;
        }

        public void GenerateReport()
        {
            using (var wordDocument = CreateWordprocessingDocument())
            {
                var mainPart = AddMainDocumentPart(wordDocument);
                var body = AppendBody(mainPart);

                SetPageMargins(mainPart);

                var title = CreateTitle();
                var table = CreateReportTable();

                body.Append(title);
                body.Append(table);
            }
        }

        public Paragraph CreateTitle()
        {
            var run = CreateRun();
            var paragraph = new Paragraph(run);

            var justification = CreateCenteredJustification();
            var paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(justification);

            paragraph.PrependChild(paragraphProperties);

            return paragraph;
        }

        private Run CreateRun()
        {
            var run = new Run(new Text(REPORT_TITLE));

            var runProp = CreateRunProperties();
            run.PrependChild(runProp);

            return run;
        }

        private RunProperties CreateRunProperties()
        {
            var runProp = new RunProperties();

            var size = CreateFontSize();
            var bold = CreateBoldStyle();

            runProp.Append(size, bold);

            return runProp;
        }

        private Justification CreateCenteredJustification()
        {
            return new Justification() { Val = JustificationValues.Center };
        }

        private FontSize CreateFontSize()
        {
            return new FontSize() { Val = "28" }; // Размер шрифта в половинных точках (14pt = 28 half-points)
        }

        private Bold CreateBoldStyle()
        {
            return new Bold();
        }


        private WordprocessingDocument CreateWordprocessingDocument()
        {
            return WordprocessingDocument.Create(_pathToSave, WordprocessingDocumentType.Document);
        }

        private MainDocumentPart AddMainDocumentPart(WordprocessingDocument wordDocument)
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            return mainPart;
        }

        private Body AppendBody(MainDocumentPart mainPart)
        {
            return mainPart.Document.AppendChild(new Body());
        }

        private void SetPageMargins(MainDocumentPart mainPart)
        {
            var sectionProps = new SectionProperties();
            var pageMargin = new PageMargin()
            {
                Top = 1440,
                Right = (UInt32Value)720U,
                Bottom = 1440,
                Left = (UInt32Value)720U,
                Header = (UInt32Value)720U,
                Footer = (UInt32Value)720U,
                Gutter = (UInt32Value)0U
            };
            sectionProps.Append(pageMargin);
            mainPart.Document.Body.Append(sectionProps);
        }

        private Table CreateReportTable()
        {
            var table = new Table();
            table.AppendChild(CreateTableProperties());
            table.Append(CreateHeaderRow());
            CreateDataRows(table);

            return table;
        }

        private void CreateDataRows(Table table)
        {
            foreach (var order in _orders)
            {
                table.Append(CreateDataRow(order));
            }
        }

        private TableProperties CreateTableProperties()
        {
            return new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 },
                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 2 }
                ),
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }
            );
        }

        private TableRow CreateHeaderRow()
        {
            var headerRow = new TableRow();
            var headers = new string[] { "№", "Отд.", "Автор, название", "Категория", "Год", "Цена", "Кол-во", "Группы" };
            CreateHeaderColumns(headerRow, headers);

            return headerRow;
        }

        private void CreateHeaderColumns(TableRow headerRow, string[] headers)
        {
            foreach (var header in headers)
            {
                headerRow.Append(CreateTableCell(header));
            }
        }

        private TableRow CreateDataRow(Order order)
        {
            var dataRow = new TableRow();
            dataRow.Append(CreateTableCell(order.Id.ToString()));
            dataRow.Append(CreateTableCell(order.Book.Otdel));
            dataRow.Append(CreateTableCell(order.Book.FullName));
            dataRow.Append(CreateTableCell(order.Book.Category.CategoryName));
            dataRow.Append(CreateTableCell(order.OrderYear.ToString()));
            dataRow.Append(CreateTableCell(order.Price.ToString("F2")));
            dataRow.Append(CreateTableCell(order.Units.ToString()));
            dataRow.Append(CreateTableCell(order.Book.GroupsText));

            return dataRow;
        }

        private TableCell CreateTableCell(string text)
        {
            var cell = new TableCell();
            var cellProps = new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Auto });
            cell.Append(cellProps);

            var cellRun = new Run(new Text(text));
            var cellPara = new Paragraph(cellRun);
            cell.Append(cellPara);

            return cell;
        }
    }
}
