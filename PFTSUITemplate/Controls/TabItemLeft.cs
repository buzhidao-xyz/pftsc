using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace PFTSUITemplate.Controls
{
    public class TabItemLeft : TabItem
    {
        public static readonly DependencyProperty MyMoverBrushProperty;
		public static readonly DependencyProperty MyEnterBrushProperty;
		public Brush MyMoverBrush
		{
			get
			{
				return base.GetValue(TabItemLeft.MyMoverBrushProperty) as Brush;
			}
			set
			{
				base.SetValue(TabItemLeft.MyMoverBrushProperty, value);
			}
		}
		public Brush MyEnterBrush
		{
			get
			{
				return base.GetValue(TabItemLeft.MyEnterBrushProperty) as Brush;
			}
			set
			{
				base.SetValue(TabItemLeft.MyEnterBrushProperty, value);
			}
		}
		static TabItemLeft()
		{
			TabItemLeft.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(TabItemLeft), new PropertyMetadata(null));
			TabItemLeft.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(TabItemLeft), new PropertyMetadata(null));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemLeft), new FrameworkPropertyMetadata(typeof(TabItemLeft)));
		}
    }
}
