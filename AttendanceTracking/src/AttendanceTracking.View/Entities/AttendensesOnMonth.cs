using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Entities
{
    public class AttendensesOnMonth
    {
        public int Excused { get; private set; }
        public int Unexcused { get; private set; }
        public AttendensesOnMonth(int excused, int unxcused)
        {
            if (excused < 0)
                throw new ArgumentException("excused must be more zero");
            if (unxcused < 0)
                throw new ArgumentException("uxcused must be more zero");
            Excused = excused;
            Unexcused = unxcused;
        }
    }
}
