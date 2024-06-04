using AttendanceTracking.View.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Services
{
    class GroupService
    {
        public string GetGroupName(int groupId)
        {
            return string.Empty;
        }

        public IEnumerable<Student> GetStudentsByGroup(int groupId)
        {
            return Enumerable.Empty<Student>();
        }

        public int GetLeaderIdByGroup(int groupId)
        {
            return 0;
        }

        public void EditLeaderIdByGroup(int leaderId, int groupId)
        {

        }

        public IEnumerable<Group> GetAllGroups()
        {
            return Enumerable.Empty<Group>();
        }

        public IEnumerable<Prepod> GetAllPrepods()
        {
            return Enumerable.Empty<Prepod>();
        }

        public void EditPrepodGroup(int groupId, int prepodId)
        {

        }
    }
}
