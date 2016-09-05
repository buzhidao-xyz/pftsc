using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PFTSScene.Tools
{
    class GridRoomEx : Grid
    {
        private int m_rows;
        private int m_cols;
        private double m_metaWidth;
        private double m_metaHeight;

        // 预定义的位置
        private List<InPosition> m_positionStack = new List<InPosition>();

        public Size MetaSize
        {
            set
            {
                int c = (int)(this.ActualWidth / value.Width);
                int r = (int)(this.ActualHeight / value.Height);
                this.m_metaWidth = value.Width;
                this.m_metaHeight = value.Height;
                int m = c <= r ? c : r;
                m_rows = m;
                m_cols = m;
                // col
                ColumnDefinition cols = new ColumnDefinition();
                cols.Width = new GridLength((this.ActualWidth - ((double)m * value.Width)) / 2);
                this.ColumnDefinitions.Add(cols);
                for (var i= 0;i < m; i++)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(value.Width);
                    this.ColumnDefinitions.Add(col);
                }
                ColumnDefinition cole = new ColumnDefinition();
                cole.Width = new GridLength((this.ActualWidth - ((double)m * value.Width)) / 2);
                this.ColumnDefinitions.Add(cole);
                // row
                RowDefinition rows = new RowDefinition();
                rows.Height = new GridLength((this.ActualHeight - ((double)m * value.Height)) / 2);
                this.RowDefinitions.Add(rows);
                for (var i = 0; i < m; i++)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(value.Height);
                    this.RowDefinitions.Add(row);
                }
                RowDefinition rowe = new RowDefinition();
                rowe.Height = new GridLength((this.ActualHeight - ((double)m * value.Height)) / 2);
                this.RowDefinitions.Add(rowe);

                int startR = this.RowDefinitions.Count / 2;
                int startC = this.ColumnDefinitions.Count / 2;
                List<List<int>> tag = new List<List<int>>();
                for (int i = 0;i < m+2; i++)
                {
                    var itag = new List<int>();
                    for (int j = 0;j < m+2; j++)
                    {
                        itag.Add(0);
                    }
                    tag.Add(itag);
                }
                int xpos = -1;
                int ypos = 0;
                while (true)
                {
                    if (startR <= 0 || startR > m || startC <= 0 || startC > m)
                    {
                        break;
                    }
                    if (tag[startR][startC] == 1) break;
                    InPosition s = new InPosition();
                    s.x = startR;
                    s.y = startC;
                    m_positionStack.Add(s);
                    tag[startR][startC] = 1;
                    startR += xpos;
                    startC += ypos;
                    int nx = -1;
                    int ny = 0;
                    if (xpos == -1 && ypos == 0)
                    {
                        nx = 0;
                        ny = -1;
                    }
                    if (xpos == 0 && ypos == -1)
                    {
                        nx = 1;
                        ny = 0;
                    }
                    if (xpos == 1 && ypos == 0)
                    {
                        nx = 0;
                        ny = 1;
                    }
                    if (xpos == 0 && ypos == 1)
                    {
                        nx = -1;
                        ny = 0;
                    }
                    if (tag[startR + nx][startC+ny] == 0)
                    {
                        xpos = nx;
                        ypos = ny;
                    }
                }
            }
        }

        public void AddImage(Image img)
        {
            for (int i = 0;i < m_positionStack.Count;i++)
            {
                var ip = m_positionStack[i];
                if (!ip.used)
                {
                    this.Children.Add(img);
                    Grid.SetRow(img,ip.x);
                    Grid.SetColumn(img,ip.y);
                    m_positionStack[i].used = true;
                    break;
                }
            }
        }

        public void Resize()
        {
            if (this.ActualHeight < 1 || this.ActualHeight < 1 || m_metaHeight < 1 || m_metaWidth < 1) return;
            // col
            this.ColumnDefinitions[0].Width = new GridLength((this.ActualWidth - ((double)m_cols * m_metaWidth)) / 2);
            for (var i = 0; i < m_cols; i++)
            {
                this.ColumnDefinitions[1+i].Width = new GridLength(m_metaWidth);
            }
            this.ColumnDefinitions[m_cols + 1].Width = new GridLength((this.ActualWidth - ((double)m_cols * m_metaWidth)) / 2);
            // row
            this.RowDefinitions[0].Height = new GridLength((this.ActualWidth - ((double)m_rows * m_metaHeight)) / 2);
            for (var i = 0; i < m_rows; i++)
            {
                this.RowDefinitions[1 + i].Height = new GridLength(m_metaHeight);
            }
            this.RowDefinitions[m_rows + 1].Height = new GridLength((this.ActualWidth - ((double)m_rows * m_metaHeight)) / 2);
        }

        public class InPosition
        {
            public int x;
            public int y;
            public bool used;
        }
    }
}
