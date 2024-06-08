using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Entities
{
    public class Account
    {
        public string Login { get; set; }

        public bool IsActive { get; set; }
        public string Status => IsActive ? "Активирована" : "Не активирована";

        public string Hash { get; set; }

        public People People { get; }
        public int Id => People.Id;
        public string FullName => People.FullName;
        public string TextRoles => People.TextRoles;



        public Account(People people, string login, string hash, bool isActive)
        {
            People = people;

            Login = login;
            IsActive = isActive;
            Hash = hash;
        }
    }
}
