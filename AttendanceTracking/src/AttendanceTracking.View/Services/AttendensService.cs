using AttendanceTracking.View.Data;
using AttendanceTracking.View.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AttendanceTracking.View.Services
{
    class AttendensService
    {
        private GroupService _groupService = new GroupService();

        public IEnumerable<Attendens> GetAttendenses(DateTime month, int groupId)
        {
            var startMonth = new DateTime(month.Year, month.Month, 1);
            var endMonth = startMonth.AddMonths(1);

            var students = _groupService.GetStudentsByGroup(groupId).ToList();

            return DataContext.GetInstance().QueryReturn(
                "SELECT passes.student_id, passes.date, passes.hours, passes.is_excused " +
                "FROM passes " +
                "JOIN students AS s ON passes.student_id = s.id " +
                "WHERE s.group_id = @0 AND passes.date >= @1 AND passes.date < @2",
                groupId, startMonth, endMonth
            ).ToList()
            .ConvertAll(row => new Attendens(
                students.FindIndex(s => s.Id == int.Parse(row[0])),
                DateTime.Parse(row[1]).Day,
                int.Parse(row[2]),
                bool.Parse(row[3])
            ));
        }

        public IEnumerable<AttendensesOnMonth> GetAttendensesOnMonth(DateTime month, int groupId)
        {
            var students = _groupService.GetStudentsByGroup(groupId).ToList();
            var attendens = GetAttendenses(month, groupId);

            return students.Select((s, i) => new AttendensesOnMonth(
                attendens.Sum(a => a.StudentIndex == i && a.IsExcused ? a.Hours : 0),
                attendens.Sum(a => a.StudentIndex == i && !a.IsExcused ? a.Hours : 0)
            ));
        }

        public void EditAttendens(DateTime month, Attendens attendens, int groupId)
        {
            var students = _groupService.GetStudentsByGroup(groupId).ToList();
            var uid = students[attendens.StudentIndex].Id;
            var date = new DateTime(month.Year, month.Month, attendens.Day);

            DataContext.GetInstance().QueryExecute(
                "DELETE " +
                "FROM passes " +
                "WHERE passes.student_id = @0 AND passes.date = @1",
                uid, date
            );
            DataContext.GetInstance().QueryExecute(
                "INSERT " +
                "INTO passes (student_id, date, hours, is_excused) " +
                "VALUES (@0, @1, @2, @3)",
                uid, date, attendens.Hours, attendens.IsExcused
            );
        }
    }
}
