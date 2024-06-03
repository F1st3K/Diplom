using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Entities
{
    public class People
    {
        public int Id { get; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronomic { get; set; }
        public string FullName => $"{LastName} {FirstName} {Patronomic}";


        public IEnumerable<string> Roles { get; }
        public string TextRoles => string.Join(", ", Roles);


        public People(int id, string firstName, string lastName, string patronomic, IEnumerable<string> roles)
        {
            Id = id;

            FirstName = firstName;
            LastName = lastName;
            Patronomic = patronomic;

            Roles = roles;
        }
    }
}
