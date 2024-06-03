using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Entities
{
    public class Group
    {
        public int Id;
        public string Name;
        public int CuratorId;
        public Group(int id, int curatorId, string name)
        {
            Id = id;
            CuratorId = curatorId;
            Name = name;
        }
    }
}
