using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Entities
{
    public class Student
    {
        public int Id { get; private set; }
        public string FullName { get; private set; }
        public Student(int id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}
