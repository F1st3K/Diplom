using AttendanceTracking.View.Data;
using AttendanceTracking.View.Entities;
using System.Collections.Generic;
using System.Linq;

namespace AttendanceTracking.View.Services
{
    class AccountService
    {
        public IEnumerable<Account> GetAccounts()
        {
            return DataContext.GetInstance().QueryReturn(
                "SELECT " +
                    "p.id, " +
                    "p.first_name, " +
                    "p.last_name, " +
                    "COALESCE(p.patronomic, '') AS patronomic, " +
                    "users.login, " +
                    "users.hash, " +
                    "users.is_active, " +
                    "CASE WHEN a.id IS NOT NULL THEN TRUE ELSE FALSE END AS admin_id, " +
                    "CASE WHEN r.id IS NOT NULL THEN TRUE ELSE FALSE END AS secretary_id, " +
                    "CASE WHEN t.id IS NOT NULL THEN TRUE ELSE FALSE END AS teacher_id, " +
                    "CASE WHEN c.id IS NOT NULL THEN TRUE ELSE FALSE END AS group_curator_id, " +
                    "CASE WHEN s.id IS NOT NULL THEN TRUE ELSE FALSE END AS student_id, " +
                    "CASE WHEN l.id IS NOT NULL THEN TRUE ELSE FALSE END AS group_leader_id " +
                "FROM users " +
                "JOIN peoples AS p ON users.id = p.id " +
                "LEFT JOIN administrators AS a ON a.id = p.id " +
                "LEFT JOIN secretaries AS r ON r.id = p.id " +
                "LEFT JOIN teachers AS t ON t.id = p.id " +
                "LEFT JOIN group_curators AS c ON c.id = p.id " +
                "LEFT JOIN students AS s ON s.id = p.id " +
                "LEFT JOIN group_leaders AS l ON l.id = p.id"
            ).ToList()
            .ConvertAll(row => new Account(
                new People(
                    int.Parse(row[0]),
                    row[1],
                    row[2],
                    row[3],
                    GetRoles(row.Skip(7).Select(v => v == "1").ToArray())
                ),
                row[4],
                row[5],
                bool.Parse(row[6])
            ));
        }

        public void EditAccount(Account account)
        {
            DataContext.GetInstance().QueryExecute(
                "DELETE " +
                "FROM users " +
                "WHERE id = @0",
                account.People.Id
            );
            DataContext.GetInstance().QueryExecute(
                "INSERT " +
                "INTO users (id, login, hash, is_active)" +
                "VALUES (@0, @1, @2, @3)",
                account.People.Id, account.Login, account.Hash, account.IsActive
            );
        }

        public void DeleteAccount(Account account)
        {
            DataContext.GetInstance().QueryExecute(
                "DELETE " +
                "FROM users " +
                "WHERE id = @0",
                account.People.Id
            );
        }

        public void CreateAccount(Account account)
        {
            DataContext.GetInstance().QueryExecute(
                "INSERT " +
                "INTO users (id, login, hash, is_active)" +
                "VALUES (@0, @1, @2, @3)",
                account.People.Id, account.Login, account.Hash, account.IsActive
            );
        }

        public IEnumerable<People> GetPeoples()
        {
            return DataContext.GetInstance().QueryReturn(
                "SELECT " +
                    "peoples.id, " +
                    "peoples.first_name, " +
                    "peoples.last_name, " +
                    "COALESCE(peoples.patronomic, '') AS patronomic, " +
                "CASE WHEN a.id IS NOT NULL THEN TRUE ELSE FALSE END AS admin_id, " +
                "CASE WHEN r.id IS NOT NULL THEN TRUE ELSE FALSE END AS secretary_id, " +
                "CASE WHEN t.id IS NOT NULL THEN TRUE ELSE FALSE END AS teacher_id, " +
                "CASE WHEN c.id IS NOT NULL THEN TRUE ELSE FALSE END AS group_curator_id, " +
                "CASE WHEN s.id IS NOT NULL THEN TRUE ELSE FALSE END AS student_id, " +
                "CASE WHEN l.id IS NOT NULL THEN TRUE ELSE FALSE END AS group_leader_id " +
                "FROM " +
                    "peoples " +
                "LEFT JOIN " +
                    "administrators AS a ON a.id = peoples.id " +
                "LEFT JOIN " +
                    "secretaries AS r ON r.id = peoples.id " +
                "LEFT JOIN " +
                    "teachers AS t ON t.id = peoples.id " +
                "LEFT JOIN " +
                    "group_curators AS c ON c.id = peoples.id " +
                "LEFT JOIN " +
                    "students AS s ON s.id = peoples.id " +
                "LEFT JOIN " +
                    "group_leaders AS l ON l.id = peoples.id"
            ).ToList()
            .ConvertAll(row => new People(
                int.Parse(row[0]),
                row[1],
                row[2],
                row[3],
                GetRoles(row.Skip(4).Select(v => v == "1").ToArray())
            ));
        }
        private static IEnumerable<Roles.Role> GetRoles(bool[] row)
        {
            var roles = new Roles.Role[] { }.ToList();
            if (row[0]) roles.Add(Roles.Role.Administrator);
            if (row[1]) roles.Add(Roles.Role.Secretary);
            if (row[2]) roles.Add(Roles.Role.Teacher);
            if (row[3]) roles.Add(Roles.Role.Curator);
            if (row[4]) roles.Add(Roles.Role.Student);
            if (row[5]) roles.Add(Roles.Role.Leader);
            return roles;
        }
    }
}
