using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        List<circle> circles = new List<circle>(); // лист содержит круги для рисовки
        List<Point> pointsforlines = new List<Point>(); // координаты точек по которым рисуются линии
        int times = 0;
        public int Score;
        public MainWindow()
        {
            InitializeComponent();
            DrawCircles();
            countCircles.Text = Info.kol.ToString();
            Screen.canvas = Canvas;
            foreach (var circle in circles)
            {
               Canvas.Children.Add((Ellipse)circle);
            }
            System.Windows.Threading.DispatcherTimer dispathcertimer = new System.Windows.Threading.DispatcherTimer();
            dispathcertimer.Tick += new EventHandler(Tick);
            dispathcertimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            dispathcertimer.Start();
            
        }
        ///<summary>
        /// Рисует круги рандомно
        /// </summary>
        /// <param name="DrawCircles"></param>
        /// <returns></returns>
        public void DrawCircles()
        {
            
            Random rnd = new Random();
            int k = Info.kol;
            circles.Clear();
            for (int i = 0; i < k; i++)
            {
                int r = (int)Info.Radius;
                try
                {
                    circles.Add(new circle((double)rnd.Next(r, (int)Canvas.ActualWidth - r), (double)rnd.Next(r, (int)Canvas.ActualHeight - r), r));
                }
                catch
                {
                    circles.Add(new circle((double)rnd.Next(150, (int)Width-150), (double)rnd.Next(150, (int)Height-150),25.0));
                }
            }
            foreach (var circle in circles)
            {
                Canvas.Children.Add((Ellipse)circle);
            }
        }
        public void Tick(object sender, EventArgs e) // функция которая выполняется по тикрейту
        {
            if (times++ == Info.time)
            {
                Canvas.Children.Clear(); 
                pointsforlines.Clear();
                DrawCircles();
                times = 0;
            }
            Timer.Text = "Time: " + (5 - times);
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<line> lines= new List<line>();
            var point = Mouse.GetPosition(Canvas);
            Screen.Clear();
            foreach (var circle in circles)
            {
                circle.ClickOnCircle(point);
                Canvas.Children.Add((Ellipse)circle);
            }
            Canvas.Children.Add((Ellipse)new circle(point.X, point.Y, 10, Background));
            
            pointsforlines.Add(Mouse.GetPosition(Canvas));
            if (pointsforlines.Count >= 2)
            {
                int maxcounter = pointsforlines.Count - 1; // рисуется изломанная линия сначала 1-2 точка потом 2-3 точка тоесть 2 линии на 3 точки
                    for (int i = 0; i < maxcounter; i++)
                    {
                        lines.Add(new line(pointsforlines[i].X, pointsforlines[i].Y, pointsforlines[i + 1].X, pointsforlines[i + 1].Y));
                    }
                
                foreach (var line in lines)
                {
                    Canvas.Children.Add((Line)line);     
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) // убавить 1
        {
            Info.kol -= 1;
            countCircles.Text = Info.kol.ToString();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) // прибавить 1
        {
            Info.kol += 1;
            countCircles.Text = Info.kol.ToString();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

     

        private void UpOfWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
    public class line
    {
        public double X1;
        public double Y1;
        public double X2;
        public double Y2;
        public line(double x1, double y1, double x2, double y2)
        {
            
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }
        public static explicit operator Line(line line)
        {
            Line newline = new Line();
            newline.X1 = line.X1;
            newline.Y1 = line.Y1;
            newline.X2 = line.X2;
            newline.Y2 = line.Y2;
            newline.Stroke = Brushes.Black;
            return newline;
        }
    }
    /// <summary>
    /// создает 2х мерные круги,
    /// выводит Ellipse
    /// </summary>
    public class circle
    {
        public double X;
        public double Y;
        // координаты центра
        public double Radius;
        //радиус
        private double Top;
        private double Left;
        //значения самого верха и лева
        public Brush Background = Brushes.Red;
        public Brush BordersColor = Brushes.Green;
        //цвета
        ///<summary>
        /// рисует круг где центр находится по коор-ам x,y с радиусом Double
        /// </summary>
        public circle(double x,double y, double radius)
        {
            
            X = x;
            Y = y;
            Radius = radius;
            SetCoordinatesForCircle(x, y, radius);
        }
        ///<summary>
        /// рисует круг где центр находится по коор-ам x,y с радиусом Double
        /// </summary>
        /// <param name="background">цвет круга</param>
        /// <returns></returns>
        public circle(double x, double y, double radius, Brush background)
        {
            
            X = x;
            Y = y;
            Radius = radius;
            Background = background;
            SetCoordinatesForCircle(x, y, radius);
        }
        /// <summary>
        /// функция требующая координат типа point чтобы узнать попал ли курсор по кругу или нет
        /// </summary>
        /// <param name="point"></param>
        public void ClickOnCircle(Point point)
        {
            double Line =Math.Sqrt(Math.Abs(Math.Pow(point.X - X,2) + Math.Pow(point.Y - Y,2)));
            Background = Line <= Radius ? Brushes.Gray : Background;
        }
        private void SetCoordinatesForCircle(double x, double y, double radius) // ставит координаты справа и сверху от центра круга на радиус чтобы найти верхний левый угол 
        {
            Top = y - radius;
            Left = x - radius;
        }
        public static explicit operator Ellipse(circle circle) //вывод готового еллипса
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = circle.Background;
            ellipse.Stroke = circle.BordersColor;
            ellipse.SetValue(Canvas.LeftProperty, circle.Left);
            ellipse.SetValue(Canvas.TopProperty, circle.Top);
            ellipse.Height = circle.Radius*2;
            ellipse.Width = circle.Radius * 2;
            return ellipse;
        }
    }
    public static class Screen
    {
        public static Canvas canvas;
        public static void Clear()
        {
            canvas.Children.Clear();
        }
    }
    public static class Info // данные с которым работает приложение
    {
        public static int kol = 10; // количество кругов
        public static double Radius = 25; //радиус кругов
        public static int time = 5; // время в секундах
    }
}