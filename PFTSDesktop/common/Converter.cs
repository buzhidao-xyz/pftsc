using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PFTSDesktop.common
{
    public class VestStatusConverter : MarkupExtension, IValueConverter
    {
        public VestStatusConverter() { }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? status = (int?)value;
            string strType = "";
            if (status == null)
            {
                strType = "未穿戴";
            }
            else
            {
                strType = "已穿戴";
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

    public class LockerStatusConverter : MarkupExtension, IValueConverter
    {

        public LockerStatusConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? status = (int?)value;
            string strType = "";
            if (status == null)
            {
                strType = "未使用";
            }
            else
            {
                strType = "已使用";
            }
            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RFIDStatusConverter : MarkupExtension, IValueConverter
    {

        public RFIDStatusConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? status = (int?)value;
            string strType = "";
            if (status == null)
            {
                strType = "新入库";
            }
            else 
            {
                strType = "已安装";
            }
            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CameraStatusConverter : MarkupExtension, IValueConverter
    {

        public CameraStatusConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? status = (int?)value;
            string strType = "";
            if (status == null)
            {
                strType = "新入库";
            }
            else 
            {
                strType = "已安装";
            }
            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SuspectStatusConverterToString : MarkupExtension, IValueConverter
    {

        public SuspectStatusConverterToString() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? status = (int?)value;
            string strType = "";
            if (status == 0)
            {
                strType = "在监";
            }
            else 
            {
                strType = "离场";
            }
            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SuspectStatusConverter : MarkupExtension, IValueConverter
    {

        public SuspectStatusConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? status = (int?)value;
            Visibility strType = Visibility.Collapsed;
            if (status == 0)
            {
                strType = Visibility.Visible;
            }
            else 
            {
                strType = Visibility.Collapsed;
            }
           
            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RecoverStatusConverter : MarkupExtension, IValueConverter
    {

        public RecoverStatusConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? status = (bool?)value;
            string strType = "";
            if (status == true)
            {
                strType = "已取";
            }
            else
            {
                strType = "未取";
            }

            return strType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
