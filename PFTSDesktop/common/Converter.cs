using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace PFTSDesktop.common
{
    public class VestStatusConverter : MarkupExtension,IValueConverter
    {
        public VestStatusConverter() { }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int status = (int)value;
            string strType = "";
            if (status == 0)
            {
                strType = "未穿戴";
            }
            else if (status == 1)
            {
                strType = "已穿戴";
            }
            else
            {
                throw new Exception("绑定类型不正确");
            }
            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
