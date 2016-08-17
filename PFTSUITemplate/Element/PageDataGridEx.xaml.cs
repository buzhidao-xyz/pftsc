using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PFTSUITemplate.Controls;
using System.ComponentModel;
using System.Windows.Threading;

namespace PFTSUITemplate.Element
{
    /// <summary>
    /// 加载请求参数
    /// </summary>
    public class RequestPagingEventArgs : HandledEventArgs
    {
        public RequestPagingEventArgs(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
        /// <summary>
        /// 需要加载的页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 需要加载的数据大小
        /// </summary>
        public int PageSize { get; set; }
    }

    public delegate void RequestPagingEventHandler(object sender, RequestPagingEventArgs e);
    /// <summary>
    /// PageDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PageDataGridEx : PagingDataGrid
    {
        #region 事件
        /// <summary>
        /// 请求数据
        /// </summary>
        public event RequestPagingEventHandler RequestPagingData;

        #endregion

        #region 属性
        /// <summary>
        /// 所有页面行数容量
        /// </summary>
        private int _count = 0;
        private int DataSourceCount
        {
            get
            { return _count; }
            set
            { _count = value; }
        }

        /// <summary>
        /// 分页模式
        /// </summary>
        private bool _pagingMode = false;
        public bool PagingMode
        {
            get { return _pagingMode; }
            set { _pagingMode = value; }
        }

        /// <summary>
        /// 数据源,分页模式下为当前页面数据
        /// </summary>
        private List<Object> DataSourceList { get; set; }
        #endregion

        public PageDataGridEx()
        {
            InitializeComponent();
        }

        public void UpdateSource()
        {
            this.Total = DataSourceCount;
            this.PageCount = this.Total / this.PageSize;
            if (this.Total % this.PageSize > 0) this.PageCount += 1;
            if (this.PageCount > 0 && PageIndex < 1) PageIndex = 1;
            if (PagingMode)
            {
                this.ItemsSource = DataSourceList;
            }
            else
            {
                this.ItemsSource = DataSourceList == null ? null : DataSourceList.Skip(this.PageSize * (this.PageIndex - 1)).Take(this.PageSize).ToList();
            }
            this.Items.Refresh();
            UpdatePager();
        }

        /// <summary>
        /// 设置数据源总数,在分页模式下有效
        /// </summary>
        public void SetTotalCount(int total)
        {
            if (PagingMode == false) return;
            DataSourceCount = total;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearSource()
        {
            DataSourceCount = 0;
            if (DataSourceList != null)
            {
                DataSourceList.Clear();
                this.ItemsSource = null;
            }
            this.PageIndex = 1;
            this.PageCount = 1;
            this.Start = 0;
            this.End = 0;
            UpdateSource();
        }

        /// <summary>
        /// 判断是否为最后一页
        /// </summary>
        /// <returns></returns>
        private bool LastPage()
        {
            return this.PageIndex > this.PageCount - 1;
        }

        /// <summary>
        /// 设置pageIndex页数据并显示,只在分页模式下有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="data"></param>
        public bool ChangePageData<T>(int pageIndex, List<T> data) where T : class, new()
        {
            if (pageIndex > this.PageCount && this.PageCount > 0) return false;
            if (PagingMode == false) return false;              //非分页模式下无效
            if (data == null || (LastPage() == false && data.Count < this.PageSize))
                return false;                                   //数据异常
            this.PageIndex = pageIndex;
            DataSourceList = new List<object>(data);
            UpdateSource();
            return true;
        }

        /// <summary>
        /// 添加数据源，在非分页模式下有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool FillDataSource(List<Object> data) //where T : class
        {
            if (data == null || data.Count == 0)
            {
                ClearSource();
            }
            DataSourceList = data;
            DataSourceCount = data.Count;
            UpdateSource();
            return true;
        }

        /// <summary>
        /// 结尾添加一行数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rec"></param>
        public void AppendData(Object rec) //where T : class, new()
        {
            List<Object> recs = new List<Object>();
            recs.Add(rec);
            AppendData(recs);
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recs"></param>
        public void AppendData(List<Object> recs) //where T : class, new()
        {
            if (recs == null || recs.Count == 0) return;
            if (PagingMode == false)
            {
                if (DataSourceList == null)
                {
                    DataSourceList = new List<Object>(recs);
                }
                else
                {
                    DataSourceList.AddRange(recs);
                }
                DataSourceCount = DataSourceList.Count;
            }
            else
            {
                if (DataSourceList.Count < this.PageSize)
                {
                    int count = this.PageSize - DataSourceList.Count;
                    count = recs.Count < count ? recs.Count : count;
                    DataSourceList.AddRange(recs.Take(count).ToList());
                }
                DataSourceCount += recs.Count;
            }
            UpdateSource();
        }

        private void PagingDataGrid_PagingChanged(object sender, PagingChangedEventArgs args)
        {
            if (DataSourceCount == 0) return;
            this.PageSize = args.PageSize;
            this.PageIndex = args.PageIndex;
            if (PagingMode)
            {
                if (RequestPagingData == null)
                    return;
                var outArgs = new RequestPagingEventArgs(args.PageIndex, args.PageSize);
                RequestPagingData(this, outArgs);

            }
            else
            {

            }
            UpdateSource();
        }

        /// <summary>
        /// 获取当前选中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public object GetSelectItem()
        {
            int pos = this.SelectedIndex;
            if (pos < 0) return null;
            if (DataSourceList.Count <= pos) return null;
            return DataSourceList[pos];
        }

        public void SetSelectItem(Object rec) //where T : class, new()
        {
            int pos = this.SelectedIndex;
            if (pos < 0) return;
            if (PagingMode)
            {
                if (DataSourceList.Count < pos) return;
                //DataSourceList.Insert(pos, rec);
                DataSourceList[pos] = rec;
                DataSourceCount += 1;
                if (DataSourceList.Count > this.PageSize)
                {
                    DataSourceList = DataSourceList.Take(this.PageSize).ToList();
                }
            }
            else
            {
                if (DataSourceList.Count < (this.PageIndex-1) * this.PageSize + pos) return;
                // DataSourceList.Insert(this.PageIndex * this.PageSize + pos,rec);
                DataSourceList[(this.PageIndex-1) * this.PageSize + pos] = rec;
                DataSourceCount = DataSourceList.Count;
            }
            UpdateSource();
        }

        public void RemoveSelectItem()
        {
            int pos = this.SelectedIndex;
            if (pos < 0) return;
            if (PagingMode)
            {
                if (DataSourceList.Count <= pos) return;
                if (LastPage())
                {
                    if (End - Start == 0)
                    {
                        if (PageIndex != 1)
                        {
                            this.PageIndex = this.PageIndex - 1;
                        }
                        var outArgs = new RequestPagingEventArgs(this.PageIndex, this.PageSize);
                        RequestPagingData(this, outArgs);
                    }
                    else
                    {
                        DataSourceList.RemoveAt(pos);
                    }
                }
                else
                {
                    if (DataSourceCount == 0) return;

                    if (RequestPagingData == null)
                        throw new Exception("未注册分页加载函数");
                    var outArgs = new RequestPagingEventArgs(this.PageIndex, this.PageSize);
                    RequestPagingData(this, outArgs);
                }
                DataSourceCount -= 1;
            }
            else
            {
                if (DataSourceList.Count <= (this.PageIndex - 1) * this.PageSize + pos) return;

                DataSourceList.RemoveAt((this.PageIndex - 1) * this.PageSize + pos);
                if (End - Start == 0)
                {
                    this.PageIndex = this.PageIndex - 1;
                    FillDataSource(DataSourceList);
                }
                DataSourceCount = DataSourceList.Count;
            }
            UpdateSource();
        }
    }
}
