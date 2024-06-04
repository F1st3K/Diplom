using System.Collections.Generic;
using System.Linq;

namespace AttendanceTracking.View.Entities
{
    public class People
    {
        public int Id { get; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronomic { get; set; }
        public string FullName => $"{LastName} {FirstName} {Patronomic}";


        public IEnumerable<Roles.Role> Roles { get; }
        public string TextRoles => string.Join(", ", Roles.Select(r => r.ToTextRole()));


        public People(int id, string firstName, string lastName, string patronomic, IEnumerable<Roles.Role> roles)
        {
            Id = id;

            FirstName = firstName;
            LastName = lastName;
            Patronomic = patronomic;

            Roles = roles;
        }
    }
}
