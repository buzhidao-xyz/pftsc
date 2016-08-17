using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PFTSUITemplate.Controls
{
    /// <summary>
    /// 分页更改参数
    /// </summary>
    public class PageChangedEventArgs : RoutedEventArgs
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public PageChangedEventArgs(RoutedEvent routeEvent, int pageSize, int pageIndex)
            : base(routeEvent)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
        }
    }
}
