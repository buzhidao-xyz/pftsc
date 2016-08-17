using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PFTSUITemplate.Controls
{
    public class ButtonTemplet : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty MyMoverBrushProperty;
        public static readonly DependencyProperty MyEnterBrushProperty;
        public static readonly DependencyProperty SelectEdProperty;
        public Brush MyMoverBrush
        {
            get
            {
                return base.GetValue(ButtonTemplet.MyMoverBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(ButtonTemplet.MyMoverBrushProperty, value);
            }
        }
        public Brush MyEnterBrush
        {
            get
            {
                return base.GetValue(ButtonTemplet.MyEnterBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(ButtonTemplet.MyEnterBrushProperty, value);
            }
        }
        public bool? SelectEd
        {
            get
            {
                return base.GetValue(ButtonTemplet.SelectEdProperty) as bool?;
            }
            set
            {
                base.SetValue(ButtonTemplet.SelectEdProperty, value);
            }
        }



        static ButtonTemplet()
        {
            ButtonTemplet.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(ButtonTemplet), new PropertyMetadata(null));
            ButtonTemplet.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(ButtonTemplet), new PropertyMetadata(null));
            ButtonTemplet.SelectEdProperty = DependencyProperty.Register("SelectEd", typeof(bool?), typeof(ButtonTemplet), new PropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonTemplet), new FrameworkPropertyMetadata(typeof(ButtonTemplet)));
        }
        //public ButtonTemplet()
        //{
        //    base.Content = "";
        //    base.Background = Brushes.Transparent;
        //}
    }
}
