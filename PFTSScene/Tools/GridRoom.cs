using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace PFTSScene.Tools
{
    class GridRoom : Grid
    {
        #region Dependency Properties

        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(double), typeof(GridRoom), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(double), typeof(GridRoom), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        //public static readonly DependencyProperty BaseElementProperty = DependencyProperty.Register("BaseElement", typeof(UIElement), typeof(GridRoom), new FrameworkPropertyMetadata(null));

        #endregion

        #region CLR Properties

        //public UIElement BaseElement
        //{
        //    get { return (UIElement)base.GetValue(BaseElementProperty); }
        //    set { base.SetValue(BaseElementProperty, value); }
        //}

        [TypeConverter(typeof(LengthConverter))]
        public double CenterX
        {
            get {
                Window window = Window.GetWindow(this);
                Point point = this.TransformToAncestor(window).Transform(new Point(0, 0));
                return point.X;
            }
            //set { base.SetValue(CenterXProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double CenterY
        {
            get {
                Window window = Window.GetWindow(this);
                Point point = this.TransformToAncestor(window).Transform(new Point(0, 0));
                return point.Y;
            }
            //set { base.SetValue(CenterYProperty, value); }
        }

        #endregion
    }
}
