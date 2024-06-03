using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Entities
{
    public class Prepod
    {
        public int Id;
        public string FullName;
        public Prepod(int id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}
