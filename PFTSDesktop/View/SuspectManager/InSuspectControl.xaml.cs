using PFTSDesktop.ViewModel;
using PFTSModel;
using PFTSModel.Services;
using PFTSTools;
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

namespace PFTSDesktop.View.SuspectManager
{
    /// <summary>
    /// InSuspectControl.xaml 的交互逻辑
    /// </summary>
    public partial class InSuspectControl : UserControl
    {
        public InSuspectControl()
        {
            InitializeComponent();
            this.DataContext = SuspectViewModel.GetInstance();
        }

        private void dataGridSuspect_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            if (item != null)
            {
                dataGridSuspect.ContextMenu.Opacity = 1;
            }
            else
            {
                dataGridSuspect.ContextMenu.Opacity = 0;
            }
        }

        private object GetElementFromPoint(ItemsControl itemsControl, Point point)
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;
                object item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                    return item;
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        private BTrackerService service;
        private void export_click(object sender, RoutedEventArgs e)
        {
            service = new BTrackerService();
            view_btracker_info model = dataGridSuspect.SelectedItem as view_btracker_info;
            List<view_btracker_video> videos = service.GetVideoByBtracker(model.id);
            List<string> videoList = new List<string>();
            foreach (view_btracker_video entity in videos)
            {
                 long timeRange = (entity.end_time.Ticks - entity.start_time.Ticks) / 10000 / 1000;
                 if (Convert.ToInt32(timeRange) > 20)
                 {
                     videoList.Add(entity.filename);
                 }
            }
            if (videoList.Count == 0)
            {
                MessageBox.Show("嫌疑犯不存在视频！", "系统提示");
                return;
            }
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".mp4"; // Default file extension
            dlg.Filter = "mp4文件(.mp4)|*.mp4"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            string filename = string.Empty;

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                filename = dlg.FileName;

                // Result could be true, false, or null

                VideoConcatTool tool = new VideoConcatTool();

                tool.ConcatVideo(videoList, filename);

            }
            else
            {
                return;
            }
        }
    }
}
