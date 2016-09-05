using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PFTSScene.Tools
{
    public class GridRoom : Grid
    {
        private List<Image> m_images = new List<Image>();
        private List<GridUtil.Point> m_pointsStack;
        private int m_preCount = 10;
        private GridUtil m_util;
        private double m_metaX;
        private double m_metaY;

        private List<Image> m_cacheImages = new List<Image>();

        public GridRoom(double mx,double my)
        {
            m_metaX = mx;
            m_metaY = my;
            this.SizeChanged += GridRoom_SizeChanged;
            this.Loaded += GridRoom_Loaded;
        }

        private void GridRoom_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            m_util = new Tools.GridUtil(this.ActualHeight, this.ActualWidth, m_metaX, m_metaY);
            m_pointsStack = m_util.Product(m_preCount);
            if (m_cacheImages.Count > 0)
            {
                foreach(var img in m_cacheImages)
                {
                    addAImage(img);
                }
                m_cacheImages = new List<Image>();
            }
        }

        private void GridRoom_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }
            m_util = new Tools.GridUtil(this.ActualHeight, this.ActualWidth, m_metaX, m_metaY);
            m_pointsStack = m_util.Product(m_preCount);
            for (int i = 0;i < m_images.Count;i++)
            {
                GridUtil.Point p = m_pointsStack[i];
                Image img = m_images[i];
                img.Margin = new System.Windows.Thickness(p.CenterY - p.SizeY / 2, p.CenterX - p.SizeX / 2,
                    this.ActualWidth - (p.CenterY + p.SizeY / 2), this.ActualHeight - (p.CenterX + p.SizeX / 2));
            }
        }

        public void AddAImage(Image img)
        {
            if (!this.IsLoaded)
            {
                m_cacheImages.Add(img);
                return;
            }
            if (m_cacheImages.Count > 0)
            {
                foreach(var aImg in m_cacheImages)
                {
                    addAImage(aImg);
                }
                m_cacheImages = new List<Image>();
            }
            addAImage(img);
        }

        private void addAImage(Image img)
        {
            int idx = m_images.Count;
            if (idx >= m_pointsStack.Count)
            {
                //数量不够，继续申请
                m_preCount = (idx / 10 + 1) * 10;
                m_pointsStack = m_util.Product(m_preCount);
            }
            GridUtil.Point p = m_pointsStack[idx];
            img.Width = m_metaY;
            img.Height = m_metaX;
            img.Margin = new System.Windows.Thickness(p.CenterY - p.SizeY / 2,p.CenterX - p.SizeX / 2,
                this.ActualWidth - (p.CenterY + p.SizeY / 2),this.ActualHeight - (p.CenterX + p.SizeX / 2));
            this.Children.Add(img);
            m_images.Add(img);
        }

        public void RemoveAImage(Image img)
        {
            if (!this.IsLoaded)
            {
                foreach( var i in m_cacheImages)
                {
                    if (img == i)
                    {
                        m_cacheImages.Remove(img);
                        return;
                    }
                }
            }else
            {
                int idx = -1;
                for(var i = 0;i < m_images.Count;i++)
                {
                    if (m_images[i] == img)
                    {
                        this.Children.Remove(img);
                        idx = i;
                        break;
                    }
                }
                if (idx >= 0)
                {
                    m_images.Remove(img);
                    for (var i = idx;i < m_images.Count; i++)
                    {
                        GridUtil.Point p = m_pointsStack[i];
                        Image ig = m_images[i];
                        ig.Margin = new System.Windows.Thickness(p.CenterY - p.SizeY / 2, p.CenterX - p.SizeX / 2,
                            this.ActualWidth - (p.CenterY + p.SizeY / 2), this.ActualHeight - (p.CenterX + p.SizeX / 2));
                    }
                }
            }
        }
    }
}
