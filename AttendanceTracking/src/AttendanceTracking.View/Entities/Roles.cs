namespace AttendanceTracking.View.Entities
{
    public static class Roles
    {
        public enum Role
        {
            Administrator,
            Secretary,
            Teacher,
            Curator,
            Student,
            Leader
        }
        public static string ToTextRole(this Role role)
        {
            switch (role)
            {
                case Role.Administrator:
                    return "Администратор";
                case Role.Secretary:
                    return "Работник УЧ";
                case Role.Teacher:
                    return "Преподаватель";
                case Role.Curator:
                    return "Куратор";
                case Role.Student:
                    return "Студент";
                case Role.Leader:
                    return "Староста";
                default:
                    return string.Empty;
            }
        }
    }
}
