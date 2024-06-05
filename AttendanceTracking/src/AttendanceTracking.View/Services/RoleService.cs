using AttendanceTracking.View.Data;

namespace AttendanceTracking.View.Services
{
    class RoleService
    {
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
