using AttendanceTracking.View.Data;
using AttendanceTracking.View.Entities;
using System.Collections.Generic;
using System.Linq;

namespace AttendanceTracking.View.Services
{
    class GroupService
    {
        public string GetGroupName(int groupId)
        {
            return DataContext.GetInstance().QueryReturn(
                "SELECT groups.name " +
                "FROM groups " +
                "WHERE groups.id = @0",
                groupId
            )?.SingleOrDefault()
            ?.SingleOrDefault() ?? string.Empty;
        }

        public IEnumerable<Student> GetStudentsByGroup(int groupId)
        {
            return DataContext.GetInstance().QueryReturn(
                "SELECT p.id, p.last_name, p.first_name, p.patronomic " +
                "FROM students " +
                "JOIN peoples AS p ON students.id = p.id " +
                "WHERE students.group_id = @0 " +
                "ORDER BY p.last_name, p.first_name, p.patronomic",
                groupId
            ).ToList()
            .ConvertAll(row => new Student(int.Parse(row[0]), $"{row[1]} {row[2]} {row[3]}"));
        }

        public int GetLeaderIdByGroup(int groupId)
        {
            return int.Parse(
                DataContext.GetInstance().QueryReturn(
                    "SELECT group_leaders.id " +
                    "FROM group_leaders " +
                    "JOIN students AS s ON s.id = group_leaders.id " +
                    "WHERE s.group_id = @0",
                    groupId
                )?.SingleOrDefault()
                ?.SingleOrDefault() ?? "-1"
            );
        }

        public int GetLeaderIndexInGroup(int groupId)
        {
            var uid = GetLeaderIdByGroup(groupId);
            var studentsGroup = GetStudentsByGroup(groupId);

            return studentsGroup.ToList().FindIndex(s => s.Id == uid);
        }

        public void EditLeaderIdByGroup(int leaderId, int groupId)
        {
            var uid = GetLeaderIdByGroup(groupId);
            DataContext.GetInstance().QueryExecute(
                "DELETE " +
                "FROM group_leaders " +
                "WHERE id = @0",
                uid
            );
            DataContext.GetInstance().QueryExecute(
                "INSERT " +
                "INTO group_leaders (id)" +
                "VALUES (@0)",
                leaderId
            );
        }

        public IEnumerable<Group> GetAllGroups()
        {
            return DataContext.GetInstance().QueryReturn(
                "SELECT groups.id, COALESCE(c.id, -1) AS c_id, groups.name " +
                "FROM groups " +
                "LEFT JOIN group_curators AS c ON groups.id = c.group_id "
            ).ToList()
            .ConvertAll(row => new Group(int.Parse(row[0]), int.Parse(row[1]), row[2]));
        }

        public IEnumerable<Prepod> GetAllPrepods()
        {
            return DataContext.GetInstance().QueryReturn(
                "SELECT p.id, p.first_name, p.last_name, p.patronomic " +
                "FROM teachers " +
                "LEFT JOIN peoples AS p ON teachers.id = p.id "
            ).ToList()
            .ConvertAll(row => new Prepod(int.Parse(row[0]), $"{row[1]} {row[2]} {row[3]}"));
        }

        public void EditCuratorGroup(int groupId, int prepodId)
        {
            DataContext.GetInstance().QueryExecute(
                "DELETE " +
                "FROM group_curators " +
                "WHERE group_id = @0",
                groupId
            );
            DataContext.GetInstance().QueryExecute(
                "INSERT " +
                "INTO group_curators (group_id, id)" +
                "VALUES (@0, @1)",
                groupId, prepodId
            );
        }
    }
}
