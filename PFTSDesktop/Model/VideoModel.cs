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
        public long video_time { get; set; }
        public string video_Date
        {
            get
            {
                TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(video_time/1000));
                string str = "";
                if (ts.Hours > 0)
                {
                    str = (ts.Hours<10?"0"+ts.Hours.ToString():ts.Hours.ToString()) + ": " +
                        (ts.Minutes < 10 ? "0" + ts.Minutes.ToString() : ts.Minutes.ToString()) + ": " +
                       (ts.Seconds < 10 ?"0"+ ts.Seconds.ToString() : ts.Seconds.ToString());
                }
                if (ts.Hours == 0 && ts.Minutes > 0)
                {
                    str = "00:" + (ts.Minutes < 10 ? "0" + ts.Minutes.ToString() : ts.Minutes.ToString()) + ": " +
                        (ts.Seconds < 10 ? "0" + ts.Seconds.ToString() : ts.Seconds.ToString());
                }
                if (ts.Hours == 0 && ts.Minutes == 0)
                {
                    str = "00:00:" + (ts.Seconds < 10 ? "0" + ts.Seconds.ToString() : ts.Seconds.ToString());
                }
                return str;
            }
        }
        public List<string> videos { get; set; }

        public List<TimeSpan> videoTimes { get; set; }
    }
}
