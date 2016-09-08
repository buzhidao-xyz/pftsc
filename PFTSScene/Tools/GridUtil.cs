using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSScene.Tools
{
    /// <summary>
    /// 根据表格长款，计算表格分布
    /// </summary>
    public class GridUtil
    {
        private double m_ox;
        private double m_oy;
        private double m_dx;
        private double m_dy;

        /// <summary>
        /// 区域值
        /// </summary>
        /// <param name="ox">高度</param>
        /// <param name="oy">宽带</param>
        /// <param name="dx">基本方格的高度</param>
        /// <param name="dy">基本方格的宽度</param>
        public GridUtil(double ox, double oy, double dx, double dy)
        {
            m_ox = ox;
            m_oy = oy;
            m_dx = dx;
            m_dy = dy;
        }

        /// <summary>
        /// 计算位置点
        /// </summary>
        /// <param name="preCount">预计个数</param>
        /// <returns></returns>
        public List<Point> Product(int preCount)
        {
            List<Point> points = new List<Point>();
            double centerX = m_ox / 2;
            double centerY = m_oy / 2;
            double pow = 0;
            bool first = true;

            points.Add(Point.Product(centerX, centerY, centerX, centerY, m_dx, m_dy));

            while (true)
            {
                double m = Math.Pow(0.5, pow);
                double ix;
                double iy;
                double dx;
                double dy;
                int startIndex;

                if (first)
                { // 第一次从中心点
                    startIndex = 0;
                    first = false;
                }
                else
                {
                    startIndex = points.Count;
                }
                ix = m * m_dx;
                iy = m * m_dy;
                dx = m * 2 * m_dx;
                dy = m * 2 * m_dy;

                //left up
                for (double i = centerX - ix; i >= 0; i = i - dx)
                {
                    for (double j = centerY - iy; j >= 0; j = j - dy)
                    {
                        points.Add(Point.Product(i, j, centerX, centerY, m_dx, m_dy));
                    }
                }
                //right up
                for (double i = centerX - ix; i >= 0; i = i - dx)
                {
                    for (double j = centerY + iy; j <= m_oy; j = j + dy)
                    {
                        points.Add(Point.Product(i, j, centerX, centerY, m_dx, m_dy));
                    }
                }
                //left down
                for (double i = centerX + ix; i < m_ox; i = i + dx)
                {
                    for (double j = centerY - iy; j >= 0; j = j - dy)
                    {
                        points.Add(Point.Product(i, j, centerX, centerY, m_dx, m_dy));
                    }
                }
                //right down
                for (double i = centerX + ix; i < m_ox; i = i + dx)
                {
                    for (double j = centerY + iy; j <= m_oy; j = j + dy)
                    {
                        points.Add(Point.Product(i, j, centerX, centerY, m_dx, m_dy));
                    }
                }
                if (points.Count >= preCount)
                {
                    break;
                }
                pow++;
                int endIndex = points.Count - 1;
                if (startIndex < endIndex)
                {
                    QuickSort(ref points, startIndex, endIndex);
                }
            }

            return points;
        }

        private void QuickSort(ref List<Point> datas, int startIndex, int endIndex)
        {
            int q;
            if (startIndex < endIndex)
            {
                q = Partition(datas, startIndex, endIndex);
                QuickSort(ref datas, startIndex, q - 1);
                QuickSort(ref datas, q + 1, endIndex);
            }
        }

        int Partition(List<Point> datas, int p, int r) //分治法，作用就是将数组分为A[p..q-1] 和A[q+1..r]  
        {                                                   //然后调整元素使得A[p..q-1]小于等于q，也小于等于A[q+1..r]  
            double x;
            int i, j;
            Point temp;

            x = datas[r].CenterLength;  //将最后一个值保存在x中  
            i = p - 1;   //开始的时候将i 移动到数组的外面  
            for (j = p; j <= r - 1; j++)
            {
                if (datas[j].CenterLength <= x)
                {
                    i += 1;
                    temp = datas[i];
                    datas[i] = datas[j];
                    datas[j] = temp;
                }
            }

            temp = datas[i + 1];  //exchange  
            datas[i + 1] = datas[r];
            datas[r] = temp;

            return i + 1;  //返回q值  
        }

        /// <summary>
        /// 点对象
        /// </summary>
        public struct Point
        {
            public double CenterX;
            public double CenterY;
            public double SizeX;
            public double SizeY;
            public double CenterLength;    //距离中间区域的长度

            public static Point Product(double i, double j, double centerX, double centerY, double dx, double dy)
            {
                Point p = new Point();
                p.CenterX = i;
                p.CenterY = j;
                p.SizeX = dx;
                p.SizeY = dy;
                p.CenterLength = (i - centerX) * (i - centerX) + (j - centerY) * (j - centerY);
                return p;
            }
        }

    }
}
