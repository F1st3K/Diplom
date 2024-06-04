using AttendanceTracking.View.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Services
{
    class AccountService
    {
        public IEnumerable<Account> GetAccounts()
        {
            return Enumerable.Empty<Account>();
        }

        public void EditAccount(Account account)
        {
            
        }

        public void DeleteAccount(Account account)
        {

        }

        public Account CreateAccount(Account account)
        {
            return account;
        }

        public IEnumerable<People> GetPeoples()
        {
            return Enumerable.Empty<People>();
        }
    }
}
