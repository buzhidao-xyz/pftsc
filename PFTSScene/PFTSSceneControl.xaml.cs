using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PFTSScene
{
    /// <summary>
    /// PFTSSceneControl.xaml 的交互逻辑
    /// </summary>
    public partial class PFTSSceneControl : UserControl
    {
        private List<InArrow> paths;

        public PFTSSceneControl()
        {
            InitializeComponent();

            paths = new List<InArrow>();
        }

        private void PathTo(Grid origin, Grid dest)
        {
            var transformStart = origin.TransformToAncestor(this.baseGrid);
            Point pointStart = transformStart.Transform(new Point(origin.ActualWidth / 2, origin.ActualHeight/2));

            var transformEnd = dest.TransformToAncestor(this.baseGrid);
            Point pointEnd = transformEnd.Transform(new Point(dest.ActualWidth / 2, dest.ActualHeight / 2));

            var arrow = new Tools.Arrow();
            arrow.X1 = pointStart.X;
            arrow.Y1 = pointStart.Y;
            arrow.X2 = pointEnd.X;
            arrow.Y2 = pointEnd.Y;
            arrow.HeadWidth = 10;
            arrow.HeadHeight = 5;
            arrow.Stroke = new SolidColorBrush(Color.FromRgb(255,0,0));
            arrow.StrokeThickness = 2;
            this.gridPaths.Children.Add(arrow);
            var ia = new InArrow();
            ia.ArrowD = arrow;
            ia.RoomOrigin = origin;
            ia.RoomDest = dest;
            paths.Add(ia);
        }

        private void RefreshPosition(InArrow ia)
        {
            var transformStart = ia.RoomOrigin.TransformToAncestor(this.baseGrid);
            Point pointStart = transformStart.Transform(new Point(ia.RoomOrigin.ActualWidth / 2, ia.RoomOrigin.ActualHeight / 2));

            var transformEnd = ia.RoomDest.TransformToAncestor(this.baseGrid);
            Point pointEnd = transformEnd.Transform(new Point(ia.RoomDest.ActualWidth / 2, ia.RoomDest.ActualHeight / 2));

            ia.ArrowD.X1 = pointStart.X;
            ia.ArrowD.Y1 = pointStart.Y;
            ia.ArrowD.X2 = pointEnd.X;
            ia.ArrowD.Y2 = pointEnd.Y;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PathTo(room1, room10);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var ia in this.paths)
            {
                RefreshPosition(ia);
            }
        }

        public struct InArrow
        {
            public Tools.Arrow ArrowD;
            public Grid RoomOrigin;
            public Grid RoomDest;
        }
    }
}
