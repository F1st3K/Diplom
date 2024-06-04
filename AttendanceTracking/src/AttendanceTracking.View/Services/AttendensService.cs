using AttendanceTracking.View.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AttendanceTracking.View.Services
{
    class AttendensService
    {
        public IEnumerable<Attendens> GetAttendenses(DateTime month, int groupId)
        {
            return Enumerable.Empty<Attendens>();
        }

        public IEnumerable<AttendensesOnMonth> GetAttendensesOnMonth(DateTime month, int groupId)
        {
            return Enumerable.Empty<AttendensesOnMonth>();
        }

        public void EditAttendens(Attendens attendens, int groupId)
        {

        }
    }
}
