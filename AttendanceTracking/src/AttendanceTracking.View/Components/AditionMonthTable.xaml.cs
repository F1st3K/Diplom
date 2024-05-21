using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace AttendanceTracking.View.Components
{
    /// <summary>
    /// Interaction logic for AditionMonthTable.xaml
    /// </summary>
    public partial class AditionMonthTable : UserControl
    {
        public class Value
        {
            public int Excused { get; private set; }
            public int Unexcused { get; private set; }
            public Value(int excused, int unxcused)
            {
                if (excused < 0)
                    throw new ArgumentException("excused must be more zero");
                if (unxcused < 0)
                    throw new ArgumentException("uxcused must be more zero");
                Excused = excused;
                Unexcused = unxcused;
            }
        }

        public AditionMonthTable(string month, IEnumerable<Value> attendenses)
        {
            InitializeComponent();
            MonthName.Text = month;
                
                
            AditionalTable.ItemsSource = attendenses
                .Concat(new Value[] { new Value(
                    attendenses.Sum(s => s.Excused),
                    attendenses.Sum(s => s.Unexcused)) 
                })
                .Select(a => new
                {
                    Excused = a.Excused != 0 ? a.Excused.ToString() : "",
                    Unexcused = a.Unexcused != 0 ? a.Unexcused.ToString() : "",
                });
        }

        private void AditionalTable_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AditionalTable.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)AditionalTable.ItemContainerGenerator.ContainerFromIndex(i);
                {
                    DataGridCell cell = AditionalTable.Columns[0].GetCellContent(row).Parent as DataGridCell;
                    if (((TextBlock)cell.Content).Text != string.Empty)
                    {
                        Style style = new Style(typeof(DataGridCell));
                        style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.CadetBlue));
                        cell.Style = style;
                    }
                }
                {
                    DataGridCell cell = AditionalTable.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    if (((TextBlock)cell.Content).Text != string.Empty)
                    {
                        Style style = new Style(typeof(DataGridCell));
                        style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.PaleVioletRed));
                        cell.Style = style;
                    }
                }
                if (i == AditionalTable.Items.Count - 1)
                {
                    {
                        DataGridCell cell = AditionalTable.Columns[0].GetCellContent(row).Parent as DataGridCell;
                        if (((TextBlock)cell.Content).Text != string.Empty)
                        {
                            Style style = new Style(typeof(DataGridCell));
                            style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Green));
                            cell.Style = style;
                        }
                    }
                    {
                        DataGridCell cell = AditionalTable.Columns[1].GetCellContent(row).Parent as DataGridCell;
                        if (((TextBlock)cell.Content).Text != string.Empty)
                        {
                            Style style = new Style(typeof(DataGridCell));
                            style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Red));
                            cell.Style = style;
                        }
                    }
                }
            }
        }
    }
}
