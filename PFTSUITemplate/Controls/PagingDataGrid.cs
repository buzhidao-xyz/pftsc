using System;
using System.Collections;
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
using System.ComponentModel;
using System.Windows.Markup;

namespace PFTSUITemplate.Controls
{
    /// <summary>
    /// 分页DataGrid
    /// </summary>
    [TemplatePart(Name = PagingDataGrid.ElementFirstPageImageButton, Type = typeof(ImageButton))]
    [TemplatePart(Name = PagingDataGrid.ElementPerviousPageImageButton, Type = typeof(ImageButton))]
    [TemplatePart(Name = PagingDataGrid.ElementNextPageImageButton, Type = typeof(ImageButton))]
    [TemplatePart(Name = PagingDataGrid.ElementLastPageImageButton, Type = typeof(ImageButton))]
    [TemplatePart(Name = PagingDataGrid.ElementPageSizeListComBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = PagingDataGrid.ElementPageIndexTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PagingDataGrid.ElementPageCountTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = PagingDataGrid.ElementStartTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = PagingDataGrid.ElementEndTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = PagingDataGrid.ElementCountTextBlock, Type = typeof(TextBlock))]
    public class PagingDataGrid : DataGrid
    {

        static PagingDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PagingDataGrid), new FrameworkPropertyMetadata(typeof(PagingDataGrid)));
        }

        public PagingDataGrid()
        {
            CanUserAddRows = false;
        }

        #region 字段和属性
        private const string ElementFirstPageImageButton = "PART_FirstPage";
        private const string ElementPerviousPageImageButton = "PART_PerviousPage";
        private const string ElementNextPageImageButton = "PART_NextPage";
        private const string ElementLastPageImageButton = "PART_LastPage";

        private const string ElementPageSizeListComBox = "PART_PageSizeList";
        private const string ElementPageIndexTextBox = "PART_PageIndex";

        private const string ElementPageCountTextBlock = "PART_PageCount";
        private const string ElementStartTextBlock = "PART_Start";
        private const string ElementEndTextBlock = "PART_End";
        private const string ElementCountTextBlock = "PART_Count";


        private ImageButton btnFirst, btnPrevious, btnNext, btnLast;
        private ComboBox cboPageSize;
        private TextBox txtPageIndex;
        private TextBlock tbPageCount, tbStart, tbEnd, tbCount;

        public delegate void PagingChangedEventHandler(object sender, PagingChangedEventArgs args);
        private PagingChangedEventArgs pagingChangedEventArgs;

        #endregion


        #region 依赖属性

        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(int), typeof(PagingDataGrid), new UIPropertyMetadata(30));

        [DefaultValue(30)]
        public int HeaderHeight
        {
            get
            {
                return (int)base.GetValue(PagingDataGrid.HeaderHeightProperty);
            }
            set
            {
                base.SetValue(PagingDataGrid.HeaderHeightProperty, value);
            }
        }

        public bool IsShowRowHeaderNumber
        {
            get { return (bool)GetValue(IsShowRowHeaderNumberProperty); }
            set { SetValue(IsShowRowHeaderNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowRowHeaderNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowRowHeaderNumberProperty =
            DependencyProperty.Register("IsShowRowHeaderNumber", typeof(bool), typeof(PagingDataGrid), new UIPropertyMetadata(true));




        public bool IsShowPaging
        {
            get { return (bool)GetValue(IsShowPagingProperty); }
            set { SetValue(IsShowPagingProperty, value); }
        }

        /// <summary>
        /// 是否显示分页
        /// </summary>
        public static readonly DependencyProperty IsShowPagingProperty =
            DependencyProperty.Register("IsShowPaging", typeof(bool), typeof(PagingDataGrid), new UIPropertyMetadata(true));



        public bool AllowPaging
        {
            get { return (bool)GetValue(AllowPagingProperty); }
            set { SetValue(AllowPagingProperty, value); }
        }

        /// <summary>
        /// 是否允许分页
        /// </summary>
        public static readonly DependencyProperty AllowPagingProperty =
            DependencyProperty.Register("AllowPaging", typeof(bool), typeof(PagingDataGrid), new UIPropertyMetadata(true));



        public string PageSizeList
        {
            get { return (string)GetValue(PageSizeListProperty); }
            set { SetValue(PageSizeListProperty, value); }
        }

        /// <summary>
        /// 显示每页记录数字符串列表
        /// 例:10,20,30
        /// </summary>
        public static readonly DependencyProperty PageSizeListProperty =
            DependencyProperty.Register("PageSizeList", typeof(string), typeof(PagingDataGrid), new UIPropertyMetadata(null, (s, e) =>
            {
                PagingDataGrid dp = s as PagingDataGrid;
                if (dp.PageSizeItemsSource == null)
                {
                    dp.PageSizeItemsSource = new List<int>();
                }
                if (dp.PageSizeItemsSource != null)
                {
                    List<string> strs = e.NewValue.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    dp.PageSizeItemsSource.Clear();
                    strs.ForEach(c =>
                    {
                        dp.PageSizeItemsSource.Add(Convert.ToInt32(c));
                    });
                }
            }));

        protected IList<int> PageSizeItemsSource
        {
            get { return (IList<int>)GetValue(PageSizeItemsSourceProperty); }
            set { SetValue(PageSizeItemsSourceProperty, value); }
        }

        /// <summary>
        /// 显示每页记录数集合
        /// </summary>
        protected static readonly DependencyProperty PageSizeItemsSourceProperty =
            DependencyProperty.Register("PageSizeItemsSource", typeof(IList<int>), typeof(PagingDataGrid), new UIPropertyMetadata(new List<int> { 20, 40, 80 }));

        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set
            {
                SetValue(TotalProperty, value);
            }
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(int), typeof(PagingDataGrid), new UIPropertyMetadata(0, (s, e) =>
            {
                if (e.NewValue != e.OldValue)
                {
                    PagingDataGrid dp = s as PagingDataGrid;
                    //dp.UpdatePager();
                    //dp.Last_Click();
                }
            }));


        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        /// <summary>
        /// 每页记录数，默认：20
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(PagingDataGrid), new UIPropertyMetadata(20));



        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        /// <summary>
        /// 当前页码，默认：1
        /// </summary>
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(PagingDataGrid), new UIPropertyMetadata(1));



        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        protected static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(PagingDataGrid), new UIPropertyMetadata(10));



        protected int Start
        {
            get { return (int)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        /// <summary>
        /// 起始记录数
        /// </summary>
        protected static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(int), typeof(PagingDataGrid), new UIPropertyMetadata(0));




        protected int End
        {
            get { return (int)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        /// <summary>
        /// 终止记录数
        /// </summary>
        protected static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(int), typeof(PagingDataGrid), new UIPropertyMetadata(0));



        public string SortField
        {
            get { return (string)GetValue(SortFieldProperty); }
            set { SetValue(SortFieldProperty, value); }
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        public static readonly DependencyProperty SortFieldProperty =
            DependencyProperty.Register("SortField", typeof(string), typeof(PagingDataGrid), new UIPropertyMetadata(null));

        public static readonly RoutedEvent PagingChangedEvent = EventManager.RegisterRoutedEvent("PagingChangedEvent", RoutingStrategy.Bubble, typeof(PagingChangedEventHandler), typeof(PagingDataGrid));
        /// <summary>
        /// 分页事件
        /// </summary>
        public event PagingChangedEventHandler PagingChanged
        {
            add
            {
                AddHandler(PagingChangedEvent, value);
            }
            remove
            {
                RemoveHandler(PagingChangedEvent, value);
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 应用样式
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            btnFirst = GetTemplateChild(ElementFirstPageImageButton) as ImageButton;
            btnPrevious = GetTemplateChild(ElementPerviousPageImageButton) as ImageButton;
            btnNext = GetTemplateChild(ElementNextPageImageButton) as ImageButton;
            btnLast = GetTemplateChild(ElementLastPageImageButton) as ImageButton;

            cboPageSize = GetTemplateChild(ElementPageSizeListComBox) as ComboBox;
            txtPageIndex = GetTemplateChild(ElementPageIndexTextBox) as TextBox;

            tbPageCount = GetTemplateChild(ElementPageCountTextBlock) as TextBlock;
            tbStart = GetTemplateChild(ElementStartTextBlock) as TextBlock;
            tbEnd = GetTemplateChild(ElementEndTextBlock) as TextBlock;
            tbCount = GetTemplateChild(ElementCountTextBlock) as TextBlock;

            btnFirst.Click += new RoutedEventHandler(btnFirst_Click);
            btnLast.Click += new RoutedEventHandler(btnLast_Click);
            btnNext.Click += new RoutedEventHandler(btnNext_Click);
            btnPrevious.Click += new RoutedEventHandler(btnPrevious_Click);

            cboPageSize.SelectionChanged += new SelectionChangedEventHandler(cboPageSize_SelectionChanged);
            txtPageIndex.PreviewKeyDown += new KeyEventHandler(txtPageIndex_PreviewKeyDown);
            txtPageIndex.LostFocus += new RoutedEventHandler(txtPageIndex_LostFocus);

            this.Loaded += new RoutedEventHandler(PagingDataGrid_Loaded);
        }

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);

            if (IsShowRowHeaderNumber)
            {
                e.Row.Header = e.Row.GetIndex() + 1;
                e.Row.Height = HeaderHeight;
            }
        }
        #endregion

        #region 分页事件
        public void RaisePageChanged(bool triggerChange = true)
        {

            if (pagingChangedEventArgs == null)
            {
                pagingChangedEventArgs = new PagingChangedEventArgs(PagingChangedEvent, PageSize, PageIndex);
            }

            if (AllowPaging)
            {
                pagingChangedEventArgs.PageSize = this.PageSize;
                pagingChangedEventArgs.PageIndex = this.PageIndex;
            }
            else
            {

                pagingChangedEventArgs.PageSize = this.PageSize = int.MaxValue;
                pagingChangedEventArgs.PageIndex = 1;
            }
            if (triggerChange)
                RaiseEvent(pagingChangedEventArgs);

            //calc start、end
            if (ItemsSource != null)
            {
                int curCount = 0;
                IEnumerator enumrator = ItemsSource.GetEnumerator();
                while (enumrator.MoveNext())
                {
                    curCount++;
                }

                //不允许分页处理
                if (!AllowPaging)
                {
                    PageSizeItemsSource.Clear();
                    PageSizeItemsSource.Add(curCount);
                    PageSize = curCount;
                }


                if (Total == 0)
                {
                    Start = End = Total = 0;
                    PageCount = 1;
                }
                else
                {
                    Start = (PageIndex - 1) * PageSize + 1;
                    End = Start + curCount - 1;

                    if (Total % PageSize != 0)
                    {
                        PageCount = Total / PageSize + 1;
                    }
                    else
                    {
                        PageCount = Total / PageSize;
                    }
                }
            }
            else
            {
                Start = End = Total = 0;
                PageCount = 1;
            }

            //调整图片的显示
            btnFirst.IsEnabled = btnPrevious.IsEnabled = (PageIndex != 1);
            btnNext.IsEnabled = btnLast.IsEnabled = (PageIndex != PageCount);
            txtPageIndex.IsEnabled = (PageCount != 1);
        }

        void PagingDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RaisePageChanged(false);
        }

        void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 1;
            RaisePageChanged();
        }



        void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex > 1)
            {
                --PageIndex;
            }
            RaisePageChanged();
        }

        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Total % PageSize != 0)
            {
                PageCount = Total / PageSize + 1;
            }
            else
            {
                PageCount = Total / PageSize;
            }

            if (PageIndex < PageCount)
            {
                ++PageIndex;
            }
            RaisePageChanged();
        }

        public void Last_Click()
        {
            if (Total % PageSize != 0)
            {
                PageCount = Total / PageSize + 1;
            }
            else
            {
                PageCount = Total / PageSize;
            }
            PageIndex = PageCount;
            RaisePageChanged();
        }

        void btnLast_Click(object sender, RoutedEventArgs e)
        {
            if (Total % PageSize != 0)
            {
                PageCount = Total / PageSize + 1;
            }
            else
            {
                PageCount = Total / PageSize;
            }
            PageIndex = PageCount;
            RaisePageChanged();
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RaisePageChanged();
        }

        void txtPageIndex_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPageIndex_LostFocus(sender, null);
                TextBox text = sender as TextBox;
                if (text.Name == "PART_PageIndex")
                {
                    //text.MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
                    // text.focus
                }
            }
        }

        void txtPageIndex_LostFocus(object sender, RoutedEventArgs e)
        {
            int pIndex = 0;
            try
            {
                pIndex = Convert.ToInt32(txtPageIndex.Text);
            }
            catch { pIndex = 1; }

            if (pIndex < 1) PageIndex = 1;
            else if (pIndex > PageCount) PageIndex = PageCount;
            else PageIndex = pIndex;

            RaisePageChanged();

        }

        void cboPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                PageSize = (int)cboPageSize.SelectedItem;
                if ((Total / PageSize + 1) < PageIndex)
                {
                    PageIndex = (Total / PageSize + 1);
                }
                RaisePageChanged();
            }
        }
        #endregion

        #region 更新页面显示
        public void UpdatePager()
        {
            //calc start、end
            if (ItemsSource != null)
            {
                int curCount = 0;
                IEnumerator enumrator = ItemsSource.GetEnumerator();
                while (enumrator.MoveNext())
                {
                    curCount++;
                }

                //不允许分页处理
                if (!AllowPaging)
                {
                    PageSizeItemsSource.Clear();
                    PageSizeItemsSource.Add(curCount);
                    PageSize = curCount;
                }


                if (Total == 0)
                {
                    Start = End = Total = 0;
                    PageCount = 1;
                }
                else
                {
                    Start = (PageIndex - 1) * PageSize + 1;
                    End = Start + curCount - 1;
                    if (Total % PageSize != 0)
                    {
                        PageCount = Total / PageSize + 1;
                    }
                    else
                    {
                        PageCount = Total / PageSize;
                    }
                }

            }
            else
            {
                Start = End = Total = 0;
                PageCount = 1;
            }
            //调整图片的显示
            if (btnFirst != null && btnPrevious != null) btnFirst.IsEnabled = btnPrevious.IsEnabled = (PageIndex != 1);
            if (btnNext != null && btnNext != null) btnNext.IsEnabled = btnLast.IsEnabled = (PageIndex != PageCount);
            if (txtPageIndex != null) txtPageIndex.IsEnabled = (PageCount != 1);
            //调整图片的显示
            //if(btnFirst != null && btnPrevious != null) btnFirst.IsEnabled = btnPrevious.IsEnabled = (PageIndex != 1);
            //if (btnNext != null && btnNext != null) btnNext.IsEnabled = btnNext.IsEnabled = (PageIndex != PageCount);
        }
        #endregion
    }

    public class PagingChangedEventArgs : RoutedEventArgs
    {

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string Sort { get; set; }

        public PagingChangedEventArgs(RoutedEvent eventToRaise, int pageSize, int pageIndex, string sort = null)
            : base(eventToRaise)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Sort = sort;
        }
    }

}
