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
    public class RequestLoadingEventArgs : HandledEventArgs
    {
        /// <summary>
        /// 需要开始加载的位置
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 需要开始加载的数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 加载的每页数据大小
        /// </summary>
        public int PageSize { get; set; }
    }

    public delegate void RequestLoadingEventHandler(object sender, RequestLoadingEventArgs e);
    /// <summary>
    /// PageDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PageDataGrid : PagingDataGrid
    {
        #region 事件
        /// <summary>
        /// 请求数据
        /// </summary>
        public event RequestLoadingEventHandler RequestLoadingData;
        /// <summary>
        /// 开始等待数据
        /// </summary>
        public event HandledEventHandler StartWaitLoading;
        /// <summary>
        /// 结束等待数据
        /// </summary>
        public event HandledEventHandler EndWaitLoading;
        /// <summary>
        /// 等待超时事件
        /// </summary>
        public event HandledEventHandler WaitLoadingTimeOut;
        #endregion

        #region 属性

        /// <summary>
        /// 加载数据超时时间,默认20s
        /// </summary>
        private TimeSpan _timeOut = new TimeSpan(0, 0, 20);
        public TimeSpan LoadingTimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        #endregion

        /// <summary>
        /// 定时器
        /// </summary>
        DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// 最大缓冲数据
        /// </summary>
        const int MAX_BUFFER_SIZE = 2000;
        int _bufferSize = 80;

        public PageDataGrid()
        {
            InitializeComponent();
        }

        #region 数据成员
        /// <summary>
        /// 缓冲区开始位置
        /// </summary>
        private int iStart = -1;
        /// <summary>
        /// 缓冲区结束位置
        /// </summary>
        private int iEnd = -1;
        /// <summary>
        /// 当前显示数据源
        /// </summary>
        private List<Object> DataSourceList { get; set; }
        /// <summary>
        /// 当前显示数据源个数
        /// </summary>
        private int _count = 0;
        /// <summary>
        /// 总的数据源大小
        /// </summary>
        private int DataSourceCount
        {
            get
            { return _count; }
            set
            { _count = value; }
        }
        private bool bWaitLoding = false;
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        private List<Object> Buffer = new List<Object>();
        #endregion

        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }


        private void TimeOutTimerStart()
        {
            timer.Stop();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = LoadingTimeOut;
            timer.Start();
        }

        private void TimeOutTimerStop()
        {
            timer.Stop();
            timer.IsEnabled = false;
        }

        public void Query(int size, int pageIndex)
        {
            if ((pageIndex - 1) * size < iStart)        //还没有缓冲数据
            {
                bWaitLoding = true;
                StartWaitLoading(this, new HandledEventArgs());
                TimeOutTimerStart();
                this.IsEnabled = false;
                return;
            }
            DataSourceList = Buffer.Skip((pageIndex - 1) * size - iStart).Take(size).ToList();
            this.ItemsSource = DataSourceList;
            this.Total = DataSourceCount;
            this.Items.Refresh();
            if (DataSourceList.Count != size && StartWaitLoading != null)
            {
                if (DataSourceCount - (pageIndex - 1) * size > DataSourceList.Count)
                {
                    bWaitLoding = true;
                    TimeOutTimerStart();
                    StartWaitLoading(this, new HandledEventArgs());
                    this.IsEnabled = false;
                    return;
                }
            }
            if (bWaitLoding && EndWaitLoading != null)
            {
                this.IsEnabled = true;
                EndWaitLoading(this, new HandledEventArgs());
                TimeOutTimerStop();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (WaitLoadingTimeOut != null)
            {
                WaitLoadingTimeOut(this, new HandledEventArgs());
            }
            this.IsEnabled = true;
            ClearSource();
        }

        public void UpdateSource()
        {
            Query(this.PageSize, this.PageIndex);
        }

        /// <summary>
        /// 设置数据量总数
        /// </summary>
        public void SetTotalCount(int total)
        {
            DataSourceCount = total;
        }

        public void ClearSource()
        {
            DataSourceCount = 0;
            if (DataSourceList != null)
                DataSourceList.Clear();
            Buffer.Clear();
            iStart = -1;
            iEnd = -1;
            this.PageIndex = 1;
            this.PageCount = 1;
            this.Start = 0;
            this.End = 0;
            //this.Items.Clear();
            this.ItemsSource = DataSourceList;
            this.Total = DataSourceCount;
            this.Items.Refresh();
        }

        #region 向缓冲区添加数据
        public void FillPageData<T>(int pageIndex, List<T> data) where T : class, new()
        {
            if (pageIndex < 1)
            {
                throw new ArgumentException();
            }
            FillData(this.PageSize * (pageIndex - 1), data);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="index">数据的其实位置</param>
        /// <param name="data"></param>
        public void FillData<T>(int index, IList<T> data) where T : class, new()
        {
            if (data.Count > MAX_BUFFER_SIZE)
            {
                throw new ArgumentException();
            }
            if (data.Count <= 0) return;
            
            if (iEnd < 0)
            {
                foreach (T t in data)
                {
                    Buffer.Add(t);
                }
                iStart = 0;
                iEnd = data.Count - 1;
                if (DataSourceCount == 0)
                {
                    DataSourceCount = data.Count;
                }
            }
            else if (index > iEnd + 1 || index + data.Count < iStart)
            {
                Buffer.Clear();
                foreach (T t in data)
                {
                    Buffer.Add(t);
                }
                iStart = index;
                iEnd = index + data.Count - 1;
            }
            else if (index < iStart)
            {
                List<Object> BufferTemp = new List<Object>();
                for (int i = index; i < index + data.Count; i++)
                {
                    BufferTemp.Add(data[i - index]);
                }
                for (int i = index + data.Count; i <= iEnd; i++)
                {
                    BufferTemp.Add(Buffer[i - iStart] as T);
                }
                iStart = index;
                Buffer = BufferTemp;
            }
            else if (index + data.Count > iEnd)
            {
                for (int i = index; i < iEnd; i++)
                {
                    Buffer[i - iStart] = data[i - index];
                }
                for (int i = iEnd + 1; i < index + data.Count; i++)
                {
                    Buffer.Add(data[i - index]);
                }
                iEnd = index + data.Count - 1;
            }
            else
            {
                for (int i = index; i < data.Count; i++)
                {
                    Buffer[i - iStart] = data[i - index];
                }
            }
            int start = PageSize * (PageIndex - 1);
            int end = PageSize * PageIndex - 1;
            UpdateSource();
            UpdatePager();
        }

        /// <summary>
        /// 结尾添加一行数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rec"></param>
        public void AppendData<T>(T rec) where T : class, new()
        {
            List<T> recs = new List<T>(){rec};
            DataSourceCount += 1;
            FillData<T>(iEnd + 1, recs);
        }

        public void AppendData<T>(IList<T> recs) where T : class, new()
        {
            DataSourceCount += recs.Count;
            FillData<T>(iEnd + 1, recs);
        }

        #endregion

        private void PagingDataGrid_PagingChanged(object sender, PagingChangedEventArgs args)
        {
            if (DataSourceCount == 0) return;
            backgroundWorker_DoWork(null, args);
            Query(args.PageSize, args.PageIndex);
        }

        private void backgroundWorker_DoWork(object sender, PagingChangedEventArgs inArg)
        {
            if (RequestLoadingData != null)
            {
                int nStart = (inArg.PageIndex - 1) * inArg.PageSize;
                int nEnd = nStart + inArg.PageSize - 1;
                RequestLoadingEventArgs arg = new RequestLoadingEventArgs();
                arg.PageSize = inArg.PageSize;
                if (nStart >= iStart && nStart + BufferSize - 1 <= iEnd)
                {
                    return;
                }
                if (nStart > iEnd || nEnd < iStart)
                {
                    arg.Start = nStart;
                    arg.Count = BufferSize;
                }
                else if (nStart >= iStart && nStart + BufferSize - 1 > iEnd)
                {
                    arg.Start = iEnd + 1;
                    arg.Count = nStart + BufferSize - iEnd - 1;
                }
                else if (nStart < iStart && nStart + BufferSize - 1 < iEnd)
                {
                    arg.Start = nStart;
                    arg.Count = iStart - nStart;
                }
                else
                {
                    arg.Start = nStart;
                    arg.Count = BufferSize;
                }
                if (arg.Start >= DataSourceCount) return;
                if (arg.Start + arg.Count > DataSourceCount) arg.Count = DataSourceCount - arg.Start;
                if (arg.Count <= 0) return;
                RequestLoadingData(this, arg);
            }
        }

        public T GetSourceItem<T>(int index) where T : class,new()
        {
            if (index < iStart || index > iEnd) return null;
            return Buffer[index - iStart] as T;
        }

        /// <summary>
        /// 获取当前选中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSelectItem<T>() where T : class,new()
        {
            int pos = this.SelectedIndex;
            if (pos < 0) return null;
            int index = (this.PageIndex - 1) * this.PageSize + pos;
            return GetSourceItem<T>(index);
        }

        public void SetSelectItem<T>(T rec) where T : class, new()
        {
            int pos = this.SelectedIndex;
            if (pos < 0) return;
            int index = (this.PageIndex - 1) * this.PageSize + pos;
            List<T> list = new List<T>();
            list.Add(rec);
            FillData<T>(index, list);
            UpdateSource();
        }

        public void RemoveSelectItem()
        {
            int pos = this.SelectedIndex;
            if (pos < 0) return;
            int index = (this.PageIndex - 1) * this.PageSize + pos;
            if (index < iStart || index > iEnd) return;
            Buffer.RemoveAt(index - iStart);
            if (PageCount <= PageSize)
                End = End - 1;
            SetTotalCount(DataSourceCount - 1);
            UpdateSource();
        }

        public void RemoveSelectItems()
        {
            int selectCount = this.SelectedItems.Count;
            foreach (object item in this.SelectedItems)
            {
                Buffer.Remove(item);
            }
            SetTotalCount(DataSourceCount - selectCount);
            UpdateSource();
        }

        public List<T> GetSelectItemCopy<T>() where T : class, new()
        {
            List<T> retList = new List<T>();
            foreach (object item in this.SelectedItems)
            {
                retList.Add(item as T);
            }
            return retList;
        }
    }
}
