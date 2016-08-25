using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class BtrackerInfoEntity
    {
        public int id{get;set;}

        public string no { get; set; }

        public string name { get; set; }

        public string number { get; set; }

        public string sex { get; set; }

        public int vest_id { get; set; }

        public System.Nullable<int> _lockerid { get; set; }

        public System.Nullable<int> officer_id { get; set; }

        public System.DateTime in_time { get; set; }

        public System.Nullable<System.DateTime> out_time { get; set; }

        public int status { get; set; }

        public System.Nullable<bool> recover { get; set; }

        public string private_goods { get; set; }

        public string officer_name { get; set; }
    }
}
