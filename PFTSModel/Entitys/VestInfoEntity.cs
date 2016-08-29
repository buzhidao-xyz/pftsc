using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Entitys
{
    public class VestInfoEntity
    {
        public int id { get; set; }

        public string no { get; set; }

        public string name { get; set; }

        public int status { get; set; }

        public System.Nullable<int> btracker_id { get; set; }

        public System.Nullable<System.DateTime> create_time { get; set; }

        public string btracker_name { get; set; }

       
    }
}
