using PFTSDesktop.ViewModel;
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
    }
}
