using System;
using System.ComponentModel;
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
using System.Windows.Controls.Primitives;

namespace PFTSScene
{

    public enum CameraMode
    {
        // 显示所有位置，高亮显示
        ShowAllPosition,
        // 监控模式，只显示摄像头，并包含操作菜单
        Monitoring,
        // 配置模式，摄像头高亮，空位置灰掉
        Configing,
        // 隐藏所有
        Hidden
    }

    public enum RFIDMode
    {
        // 显示所有位置，高亮显示
        ShowAllPosition,
        // 监控模式，全部隐藏
        Monitoring,
        // 配置模式，摄像头高亮，空位置灰掉
        Configing,
        // 隐藏所有
        Hidden
    }

    /// <summary>
    /// PFTSSceneControl.xaml 的交互逻辑
    /// </summary>
    public partial class PFTSSceneControl : UserControl
    {
        static PFTSResourceLoader sceneResourceCache = new PFTSResourceLoader();
        private Dictionary<int, Grid> m_mapRooms = new Dictionary<int, Grid>();
        private Dictionary<int, Image> m_mapRfids = new Dictionary<int, Image>();
        private Dictionary<int, Image> m_mapCameras = new Dictionary<int, Image>();
        private Dictionary<int, ContextMenu> m_mapCameraMenu = new Dictionary<int, ContextMenu>();

        private CameraMode m_cameraMode;
        private RFIDMode m_rfidMode;

        private List<InArrow> paths;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PFTSSceneControl()
        {
            InitializeComponent();
            // load controls
            loadLocalRooms();
            loadLocalRFIDImages();
            loadLocalCameraImages();

            //LoadRFIDInfos();
            //LoadCameraInfos();
        }

        #region Dependency Properties

        public static readonly DependencyProperty RfidImageVisibleProperty = DependencyProperty.Register("RfidImageVisiable", typeof(Visibility), typeof(PFTSSceneControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty CameraImageVisibleProperty = DependencyProperty.Register("CameraImageVisible", typeof(Visibility), typeof(PFTSSceneControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        #endregion

        #region CLR Properties

        public Visibility RfidImageVisiable
        {
            get { return (Visibility)base.GetValue(RfidImageVisibleProperty); }
            set { base.SetValue(RfidImageVisibleProperty, value); }
        }

        public Visibility CameraImageVisible
        {
            get { return (Visibility)base.GetValue(CameraImageVisibleProperty); }
            set { base.SetValue(CameraImageVisibleProperty, value); }
        }

        #endregion

        #region 保存场景界面元素
        public void loadLocalRooms()
        {
            for(var i = 0;i < 42; i++)
            {
                object element = this.FindName("room" + (i + 1));
                if (element == null) continue;
                try
                {
                    Grid room = (Grid)element;
                    string tags = room.Tag.ToString();
                    int tagi = int.Parse(tags);
                    m_mapRooms.Add(tagi, room);
                }
                catch { continue; }
            }
        }

        public void loadLocalRFIDImages()
        {
            for(var i = 0; i < 42; i++)
            {
                object element = this.FindName("imgRfid" + (i + 1));
                if (element == null) continue;
                try
                {
                    Image img = (Image)element;
                    Grid room = (Grid)img.Parent;
                    string tags = room.Tag.ToString();
                    int tagi = int.Parse(tags);
                    m_mapRfids.Add(tagi, img);
                }
                catch { continue; }
            }
        }

        public void loadLocalCameraImages()
        {
            for (var i = 0; i < 40; i++)
            {
                object element = this.FindName("imgCamera" + (i + 1));
                if (element == null) continue;
                try
                {
                    Image img = (Image)element;
                    string tags = img.Tag.ToString();
                    int tagi = int.Parse(tags);
                    m_mapCameras.Add(tagi, img);
                }
                catch { continue; }
            }
        }
        #endregion

        /// <summary>
        /// 摄像头模式
        /// </summary>
        public CameraMode CameraMode
        {
            get
            {
                return m_cameraMode;
            }
            set
            {
                m_cameraMode = value;
                switch (m_cameraMode)
                {
                    case CameraMode.ShowAllPosition:
                        CameraImageVisible = Visibility.Visible;
                        break;
                    case CameraMode.Hidden:
                        CameraImageVisible = Visibility.Hidden;
                        break;
                    case CameraMode.Monitoring:
                        LoadMonitoringCameraInfos();
                        break;
                    case CameraMode.Configing:
                        LoadConfigCameraInfos();
                        break;
                }
            }
        }

        public RFIDMode RFIDMode
        {
            get
            {
                return m_rfidMode;
            }
            set
            {
                m_rfidMode = value;
                switch (m_rfidMode)
                {
                    case RFIDMode.ShowAllPosition:
                        RfidImageVisiable = Visibility.Visible;
                        break;
                    case RFIDMode.Monitoring:
                    case RFIDMode.Hidden:
                        RfidImageVisiable = Visibility.Hidden;
                        break;
                    case RFIDMode.Configing:
                        LoadConfigRFIDInfos();
                        break;
                }
            }
        }


        /// <summary>
        /// 更新rfid信息,Configing模式
        /// </summary>
        public void LoadConfigRFIDInfos()
        {
            var rfidPositions = PFTSSceneControl.sceneResourceCache.PositionRFIDs;
            if (rfidPositions == null) return;
            foreach(PFTSModel.position_rfid pr in rfidPositions)
            {
                if (!m_mapRfids.ContainsKey(pr.id)) continue;
                Image img = m_mapRfids[pr.id];
                if (pr.rfid_id == null)
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/rfid_gray.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/rfid.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                }
            }
        }

        /// <summary>
        /// 更新camera信息,Configing模式
        /// </summary>
        public void LoadConfigCameraInfos()
        {
            var cameraPositions = PFTSSceneControl.sceneResourceCache.PositionCameras;
            if (cameraPositions == null) return;
            foreach (PFTSModel.position_camera pr in cameraPositions)
            {
                if (!m_mapCameras.ContainsKey(pr.id)) continue;
                Image img = m_mapCameras[pr.id];
                if (pr.camera_id == null)
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/camera_gray.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/camera.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                    img.Cursor = Cursors.Hand;   
                }
            }
        }

        /// <summary>
        /// 加载相机信息，监控模式
        /// </summary>
        public void LoadMonitoringCameraInfos()
        {
            var cameraPositions = PFTSSceneControl.sceneResourceCache.PositionCameras;
            if (cameraPositions == null) return;
            foreach (PFTSModel.position_camera pr in cameraPositions)
            {
                if (!m_mapCameras.ContainsKey(pr.id)) continue;
                Image img = m_mapCameras[pr.id];
                if (pr.camera_id == null)
                {
                    img.Visibility = Visibility.Hidden;
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/camera.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                    img.Cursor = Cursors.Hand;
                    var ctxMenu = new ContextMenu();
                    var menuItem1 = new MenuItem();
                    menuItem1.Header = "实时监控";
                    var menuItem2 = new MenuItem();
                    menuItem2.Header = "历史画面";
                    var menuItem3 = new MenuItem();
                    menuItem3.Header = "取消";
                    ctxMenu.Items.Add(menuItem1);
                    ctxMenu.Items.Add(menuItem2);
                    ctxMenu.Items.Add(menuItem3);
                    m_mapCameraMenu.Add(pr.id, ctxMenu);
                    img.MouseUp += ImgCamera_MouseUp;
                }
            }
        }

        /// <summary>
        /// 摄像头鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgCamera_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image img = (Image)sender;
                Grid room = (Grid)img.Parent;
                string tags = room.Tag.ToString();
                int tagi = int.Parse(tags);
                if (m_mapCameraMenu.ContainsKey(tagi))
                {
                    var ctxMenu = m_mapCameraMenu[tagi];
                    ctxMenu.PlacementTarget = img;
                    ctxMenu.Placement = PlacementMode.Top;
                    ctxMenu.IsOpen = true;
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 画不同房间之间的路径（包含方向）
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dest"></param>
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
            //PathTo(room1, room10);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //foreach (var ia in this.paths)
            //{
            //    RefreshPosition(ia);
            //}
        }

        public struct InArrow
        {
            public Tools.Arrow ArrowD;
            public Grid RoomOrigin;
            public Grid RoomDest;
        }
    }
}
