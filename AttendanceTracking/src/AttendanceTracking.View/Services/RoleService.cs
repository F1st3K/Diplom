using AttendanceTracking.View.Data;
using System.Linq;

namespace AttendanceTracking.View.Services
{
    class RoleService
    {
        public int GetGroupIdByCurator(int curatorId)
        {
            return int.Parse(
                DataContext.GetInstance().QueryReturn(
                    "SELECT group_curators.group_id " +
                    "FROM group_curators " +
                    "WHERE group_curators.id = @0",
                    curatorId
                )?.SingleOrDefault()
                ?.SingleOrDefault() ?? "-1"
            );
        }

        public int GetGroupIdByStudent(int studentId)
        {
            return int.Parse(
                DataContext.GetInstance().QueryReturn(
                    "SELECT students.group_id " +
                    "FROM students " +
                    "WHERE students.id = @0",
                    studentId
                )?.SingleOrDefault()
                ?.SingleOrDefault() ?? "-1"
            );
        }

        public void AddToAdministrators(int uid)
        {
            DataContext.GetInstance().QueryExecute(
                "INSERT " +
                "INTO administrators (id)" +
                "VALUES (@0)",
                uid
            );
        }

        public void AddToSecretary(int uid)
        {
            DataContext.GetInstance().QueryExecute(
                "INSERT " +
                "INTO secretaries (id)" +
                "VALUES (@0)",
                uid
            );
        }
    }
}
