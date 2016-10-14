using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PFTSScene.Tools
{
    public enum DebugLevel
    {
        DEBUG,
        INFO,
        ALERT,
        ERROR,
        FATAL
    }

    public class TextBlockDebug : TextBlock
    {
        private List<DebugItem> m_items = new List<DebugItem>();

        public TextBlockDebug()
        {

        }

        public ScrollViewer ScrollView { get; set; }

        public void AddText(DebugItem item,bool debug = false)
        {
            Run r = new Run();
            r.FontSize = 18;
            string str = "";
            var now = DateTime.Now;
            if (item.Level == DebugLevel.DEBUG)
            {
                r.Foreground = Brushes.White;
                str += "[DEBUG]"; 
            }
            else if (item.Level == DebugLevel.INFO)
            {
                r.Foreground = Brushes.Black;
                str += "[INFO]";
            }
            else if (item.Level == DebugLevel.ALERT)
            {
                r.Foreground = Brushes.Yellow;
                str += "[ALERT]";
            }
            else if (item.Level == DebugLevel.ERROR)
            {
                r.Foreground = Brushes.Red;
                str += "[ERROR]";
            }
            else
            {
                r.Foreground = Brushes.DarkRed;
                str += "[FATAL]";
            }
            str += now.ToString("[yyyy-MM-dd HH:mm:ss]");
            if (debug)
            {
                r.Text = str + item.Text;
            }else
            {
                r.Text = item.Text;
            }
            this.Inlines.Add(r);
            this.Inlines.Add(new LineBreak());
            if (ScrollView != null) ScrollView.ScrollToEnd();
            m_items.Add(item);
        }

        public void Debug(string text, bool b = true)
        {
            var item = new DebugItem();
            item.Level = DebugLevel.DEBUG;
            item.Text = text;
            AddText(item,b);
        }

        public void Info(string text,bool b = true)
        {
            var item = new DebugItem();
            item.Level = DebugLevel.INFO;
            item.Text = text;
            AddText(item, b);
        }

        public void Alert(string text,bool b = true)
        {
            var item = new DebugItem();
            item.Level = DebugLevel.ALERT;
            item.Text = text;
            AddText(item, b);
        }

        public void Error(string text, bool b = true)
        {
            var item = new DebugItem();
            item.Level = DebugLevel.ERROR;
            item.Text = text;
            AddText(item, b);
        }

        public void Fatal(string text, bool b = true)
        {
            var item = new DebugItem();
            item.Level = DebugLevel.FATAL;
            item.Text = text;
            AddText(item, b);
        }

        public void Clear()
        {
            this.Inlines.Clear();
            m_items.Clear();
        }

        public struct DebugItem
        {
            public DebugLevel Level;
            public string Text;
        }

    }
}
