using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSDesktop.Model
{
    public class VideoModel
    {
        public int id { get; set; }
        public string video_name { get; set; }
        public List<string> videos { get; set; }
    }
}
