using AttendanceTracking.View.Components;
using AttendanceTracking.View.Forms;
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

namespace AttendanceTracking.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var form = new AttendanceAccountingForm(
                "ПС-20Б",
                new[] { "Иванов Иван Иванович", "Петров Петр Петрович", "Сидоров Александр Сергеевич", "Козлов Дмитрий Александрович", "Ильин Андрей Васильевич", "Морозов Сергей Николаевич", "Никитин Владимир Игоревич", "Захаров Алексей Дмитриевич", "Орлов Егор Владимирович", "Семенов Виктор Павлович", "Макаров Игорь Степанович", "Андреев Максим Викторович", "Лебедев Артем Алексеевич", "Архипов Даниил Сергеевич", "Степанов Григорий Антонович", "Кузнецов Роман Александрович", "Тимофеев Артур Дмитриевич", "Павлов Павел Олегович", "Жуков Николай Валерьевич", "Беляев Василий Васильевич", "Щербаков Валентин Степанович", "Григорьев Георгий Петрович", "Комаров Вячеслав Евгеньевич", "Прокофьев Федор Арсеньевич", "Власов Владислав Игнатьевич" },
                d => new MonthTable.Value[] { new MonthTable.Value(0, 8, 8, true) },
                e => MessageBox.Show($"({e.RowIndex},{e.Day}): {e.Hours} ->{e.IsExcused}")
            );
            form.Show();
            new AuthForm().Show();
        }
    }
}
