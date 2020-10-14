﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graham
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        List<Point> dots = new List<Point>();

        public Form1()
        {
            InitializeComponent();
            int wdth = pictureBox1.Width;
            int hght = pictureBox1.Height;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = new Bitmap(wdth, hght);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Point s0 = dots[0];
            int s0_index = 0;
            for (int i = 1; i < dots.Count; i++)
            {
                if (dots[i].Y < s0.Y)
                {
                    s0 = dots[i];
                    s0_index = i;
                }
            }
            // обозначим экстремальную точку S0 и удаляем её из набора остальных точек
            set_pixel(s0.X, s0.Y, Color.Red);
            dots.RemoveAt(s0_index);

            for (int i = 0; i < dots.Count; i++)
            {
                int j = i;
                while (j > 1 && CrossP(s0, dots[j - 1], dots[j]) < 0)
                {
                    Point temp = dots[j];
                    dots[j] = dots[j - 1];
                    dots[j - 1] = temp;
                    j -= 1;
                }
            }

            // сортируем точки
            // Остальные точки сортируются в порядке увеличения полярного угла относительно точки S0.
            // Если полярные углы точек равны, то точка меньше, если ее радиус(расстояние до точки S0) меньше.
            //for (int i = 0; i < dots.Count; i++)
            //{
            //    for (int j = i + 1; j < dots.Count - 1; j++)
            //    {
            //        double r_i = Math.Sqrt(Math.Pow(s0.X - dots[i].X, 2) + Math.Pow(s0.Y - dots[i].Y, 2));
            //        double phi_i = Math.Abs(dots[i].X - s0.X) / r_i;

            //        double r_j = Math.Sqrt(Math.Pow(s0.X - dots[j].X, 2) + Math.Pow(s0.Y - dots[j].Y, 2));
            //        double phi_j = Math.Abs(dots[j].X - s0.X) / r_j;

            //        //if (phi_i > phi_j || r_i > r_j)
            //        //{
            //        //    Point temp = dots[i];
            //        //    dots[i] = dots[j];
            //        //    dots[j] = temp;
            //        //}


            //        if (dots[i].X > dots[j].X)
            //        {
            //            Point temp = dots[i];
            //            dots[i] = dots[j];
            //            dots[j] = temp;
            //        }
            //    }
            //}

            // Алгоритм Грехема
            List<Point> current = new List<Point>();
            current.Add(s0);
            current.Add(dots[0]);

            for (int i = 1; i < dots.Count; i++)
            {
                while (current.Count > 2 && CrossP(current.ElementAt(current.Count - 2), current.ElementAt(current.Count - 1), dots[i]) < 0)
                {
                    current.RemoveAt(current.Count - 1);
                }
                current.Add(dots[i]);
            }


            for (int i = 0; i < current.Count - 1; i++)
            {
                WuLine(current[i].X, current[i].Y, current[i + 1].X, current[i + 1].Y);
            }
            WuLine(current[current.Count - 1].X, current[current.Count - 1].Y, current[0].X, current[0].Y);
        }


        public void WuLine(int x0, int y0, int x1, int y1)
        {
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            DrawPoint(steep, x0, y0, 1); // Эта функция автоматом меняет координаты местами в зависимости от переменной steep
            DrawPoint(steep, x1, y1, 1); // Последний аргумент — интенсивность в долях единицы
            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = dy / dx;
            float y = y0 + gradient;
            for (var x = x0 + 1; x <= x1 - 1; x++)
            {
                DrawPoint(steep, x, (int)y, (int)((1 - (y - (int)y)) * 255));
                DrawPoint(steep, x, (int)y + 1, (int)((y - (int)y) * 255));
                y += gradient;
            }
        }

        public void DrawPoint(bool steep, int x, int y, int alpha)
        {
            if (steep)
            {
                (x, y) = (y, x);
            }
            Color color = Color.FromArgb(alpha, 0, 0, 0);
            bmp.SetPixel(x, y, color);
        }

        //Положение точки относительно прямой
        private int CrossP(PointF c, PointF l, PointF r)
        {
            var help = (c.X - l.X) * (r.Y - l.Y) - (c.Y - l.Y) * (r.X - l.X);
            if (help > 0)
            {
                return 1;
            }
            else if (help < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            dots.Add(new Point(e.X, e.Y));

            set_pixel(e.X, e.Y, Color.Black);
        }

        private void set_pixel(int x, int y, Color color)
        {
            for (int i = x - 2; i < x + 2; i++)
            {
                for (int j = y - 2; j < y + 2; j++)
                {
                    if (i >= 0 && j < pictureBox1.Height && j >= 0 && i < pictureBox1.Width)
                    {
                        bmp.SetPixel(i, j, color);
                    }
                }
            }
            pictureBox1.Image = bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int wdth = pictureBox1.Width;
            int hght = pictureBox1.Height;
            bmp = new Bitmap(wdth, hght);
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            dots.Clear();
        }
    }
}
