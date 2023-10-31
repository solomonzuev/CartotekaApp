using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CartotekaApp.Domain
{
    public class DataGridViewPrinter
    {
        private const int RAND_HEIGHT_THAT_OPTIMAL_NOW = 120;
        private readonly DataGridView dgv;
        private readonly PrintDocument printDoc;
        private readonly Font font;

        private readonly int paperWidth;
        private readonly int paperHeight;
        private readonly int marginTop,
                            marginLeft,
                            marginRight,
                            marginBottom;


        private int currentRow = 0;

        public DataGridViewPrinter(DataGridView dataGridView, PrintDocument doc)
        {
            dgv = dataGridView;
            font = new Font(dgv.DefaultCellStyle.Font.FontFamily, 8, FontStyle.Regular);

            printDoc = doc;

            marginTop = printDoc.DefaultPageSettings.Margins.Top;
            marginLeft = printDoc.DefaultPageSettings.Margins.Left;
            marginRight = printDoc.DefaultPageSettings.Margins.Right;
            marginBottom = printDoc.DefaultPageSettings.Margins.Bottom;

            if (printDoc.DefaultPageSettings.Landscape)
            {
                paperWidth = printDoc.DefaultPageSettings.PaperSize.Height;
                paperHeight = printDoc.DefaultPageSettings.PaperSize.Width;
            }
            else
            {
                paperWidth = printDoc.DefaultPageSettings.PaperSize.Width;
                paperHeight = printDoc.DefaultPageSettings.PaperSize.Height;
            }
        }

        private int IndexFromEnd(string str, char symbol)
        {
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == symbol)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetTableWidth()
        {
            int result = marginLeft + marginRight;
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (dgv.Columns[i].Visible == false)
                {
                    continue;
                }

                if (result + dgv.Columns[i].Width > paperWidth)
                {
                    break;
                }

                result += dgv.Columns[i].Width;
            }
            return result - marginLeft - marginRight;
        }

        private string WrapText(Graphics g, string text, float width)
        {
            List<string> list = new List<string>
            {
                text ?? string.Empty
            };

            for (int i = 0; i < list.Count; i++)
            {
                float lineWidth = g.MeasureString(list[i], font).Width;
                while (lineWidth >= width)
                {
                    int idx = IndexFromEnd(list[i], ' ');

                    if (idx == -1)
                    {
                        for (int j = list[i].Length - 1; j > 0; j--)
                        {
                            string cuttedStr = list[i].Substring(0, j);
                            // Если обрезанная строка по ширине меньше, чем ширина ячейки
                            if (g.MeasureString(cuttedStr, font).Width < width)
                            {
                                idx = j;
                                break;
                            }
                        }
                    }
                    // Если это последняя строка и не добавлено новой
                    if (i == list.Count - 1)
                    {
                        list.Add(list[i].Substring(idx + 1));
                    }
                    else
                    {
                        list[i + 1] = list[i].Substring(idx + 1) + " " + list[i + 1];
                    }

                    list[i] = list[i].Remove(idx);

                    lineWidth = g.MeasureString(list[i], font).Width;
                }
            }

            return string.Join("\n", list.ToArray());
        }

        private void DrawHeader(Graphics g)
        {
            int startX = marginLeft;
            int startY = marginTop;

            int tableWidth = GetTableWidth();
            int diff = paperWidth - tableWidth - marginLeft - marginRight;
            int offsetX = diff / 2;

            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (dgv.Columns[i].Visible == false)
                {
                    continue;
                }

                // Если текущая колонка не поместится полностью
                if (startX + marginRight + dgv.Columns[i].Width > paperWidth)
                {
                    break;
                }

                g.FillRectangle(Brushes.LightGray, startX + offsetX, startY, dgv.Columns[i].Width, dgv.ColumnHeadersHeight);
                g.DrawRectangle(Pens.Black, startX + offsetX, startY, dgv.Columns[i].Width, dgv.ColumnHeadersHeight);

                string wrappedText = WrapText(g, dgv.Columns[i].HeaderText, dgv.Columns[i].Width - 4);

                float alignY = (dgv.ColumnHeadersHeight - g.MeasureString(wrappedText, font).Height) / 2;
                if (alignY < 0) alignY = 0;

                g.DrawString(wrappedText, font, Brushes.Black, startX + offsetX + 4, startY + alignY);
                startX += dgv.Columns[i].Width;
            }
        }

        private bool DrawRows(Graphics g)
        {
            int tableWidth = GetTableWidth();
            int diff = paperWidth - tableWidth - marginLeft - marginRight;
            int offsetX = diff / 2;

            int startX;
            int startY = marginTop + dgv.ColumnHeadersHeight;

            float alignY;

            for (int i = currentRow; i < dgv.RowCount; i++)
            {
                // Если высота вместе с текущей колонкой превышает высоту листа
                if (startY + marginBottom + dgv.Rows[i].Height > paperHeight)
                {
                    return true;
                }

                // Перевод начальной точки по X после заголовков(либо после прохода и отрисовки строки)
                startX = marginLeft;


                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    if (dgv.Columns[j].Visible == false)
                    {
                        continue;
                    }

                    // Если текущая колонка не поместится полностью
                    if (startX + marginRight + dgv.Columns[j].Width > paperWidth)
                    {
                        break;
                    }

                    //g.DrawRectangle(Pens.Black, startX + offsetX, startY, dgv.Columns[j].Width, dgv.Rows[i].Height);
                    g.DrawRectangle(Pens.Black, startX + offsetX, startY, dgv.Columns[j].Width, RAND_HEIGHT_THAT_OPTIMAL_NOW);

                    if (dgv[j, i] != null)
                    {
                        string wrappedText = WrapText(g, Convert.ToString(dgv[j, i].Value), dgv.Columns[j].Width - 2);

                        //alignY = (dgv.Rows[i].Height - g.MeasureString(wrappedText, font).Height) / 2;
                        alignY = (dgv.Rows[i].Height - g.MeasureString(wrappedText, font).Height) / 2;
                        if (alignY < 0) alignY = 0;

                        g.DrawString(wrappedText, font, Brushes.Black, startX + offsetX + 1, startY + alignY);
                    }
                    startX += dgv.Columns[j].Width;
                }

                //startY += dgv.Rows[i].Height;
                startY += RAND_HEIGHT_THAT_OPTIMAL_NOW;
                currentRow++;
            }

            // После того, как отрисовали все строки - сбрасываем счетчик строк,
            // чтобы на печать выводилось корректно
            currentRow = 0;
            return false;
        }

        public bool DrawDataGridView(Graphics g)
        {
            DrawHeader(g);
            bool isContinuePrint = DrawRows(g);
            return isContinuePrint;
        }
    }
}
