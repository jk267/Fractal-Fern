/*********************************************************************
* Title: Fractal Fern
* Author: David Kim
* Date: 10/28/2023
*********************************************************************/

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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Converters;

namespace FernNamespace
{
    /*
     * this class draws a fractal fern when the constructor is called.
     * 
     * Bugs: WPF and shape objects are the wrong tool for the task 
     */
    class Fern
    {
        private static double DELTATHETA = 0.1;
        private static double THETA = 3 * Math.PI / 2;

        /* 
         * Fern constructor erases screen and draws a fern
         * 
         * Size: number of 3-pixel segments of tendrils
         * Redux: how much smaller children clusters are compared to parents
         * Turnbias: how likely to turn right vs. left (0=always left, 0.5 = 50/50, 1.0 = always right)
         * canvas: the canvas that the fern will be drawn on
         */
        public Fern(double size, double redux, double turnbias, Canvas canvas)
        {
            canvas.Children.Clear();
            // delete old canvas contents
            // draw a new fern at the center of the canvas with given parameters
            canvas.Background = new SolidColorBrush(Color.FromArgb(255, 215, 246, 255));
            fractal_fern((int)(canvas.Width / 2), (int)(canvas.Height * 5 / 6), size, redux, turnbias, THETA, canvas);
            pot((int)(canvas.Width / 2), (int)(canvas.Height * 5 / 6), 80, 70, canvas);
            ovals(canvas);
            drawSun(canvas);
            drawGrass(canvas);
        }

        /*fractal fern draws a fern random numbers of berries and pentagons*/

        private void fractal_fern(int x1, int y1, double size, double redux, double turnbias, double direction, Canvas canvas)
        {
            int x2 = x1, y2 = y1;
            Random random = new Random();
            if (size > 1)
            {
                for (int i = 0; i < 30; i++)
                {
                    direction += (random.NextDouble() > turnbias) ? -1 * DELTATHETA : DELTATHETA;
                    x1 = x2; y1 = y2;
                    x2 = x1 + (int)(size * Math.Cos(direction)) * -1;
                    y2 = y1 + (int)(size * Math.Sin(direction));

                    byte red = 73;
                    byte green = 187;
                    byte blue = 66;

                    line(x1, y1, x2, y2, red, green, blue, 1 + size / 15, canvas);
                    size *= 0.83;

                    /*randomness added for the probability of the appearanace of the next berry*/
                    if (random.NextDouble() < 0.015)
                    {
                        berry(x2, y2, 2.25, canvas);

                    }
                    /*randomness added for the probabilyt of the appearanace of the next pentagon*/
                    if (random.NextDouble() < 0.003)
                    {
                        pentagon(x2, y2, 1, canvas);
                    }
                    /*recursively call the next left and right branch on the fern*/
                    fractal_fern(x2, y2, size / redux, redux, turnbias, direction + Math.PI / 3, canvas); // Right branch}
                    fractal_fern(x2, y2, size / redux, redux, turnbias, direction - Math.PI / 3, canvas); // Left branch}
                }
            }
        }

        /*draws a pot under the fern */
        private void pot(int x, int y, int width, int height, Canvas canvas)
        {
            // Calculate the coordinates for the trapezium-like pot
            int x1 = x - width / 2;
            int y1 = y;
            int x2 = x + width / 2;
            int y2 = y;
            int x3 = x + width / 4;
            int y3 = y + height;
            int x4 = x - width / 4;
            int y4 = y + height;

            // Create a Polygon for the pot
            Polygon potPolygon = new Polygon();
            potPolygon.Fill = Brushes.Brown; // Fill with brown color

            // Define the points of the pot
            PointCollection points = new PointCollection();
            points.Add(new Point(x1, y1));
            points.Add(new Point(x2, y2));
            points.Add(new Point(x3, y3));
            points.Add(new Point(x4, y4));

            potPolygon.Points = points;

            // Add the potPolygon to the canvas
            canvas.Children.Add(potPolygon);
        }


        /* draws berrries on the fractal fern*/
        private void berry(int x, int y, double radius, Canvas canvas)
        {
            // Create a radial gradient for the berry's appearance with purple colors
            RadialGradientBrush gradientBrush = new RadialGradientBrush();
            gradientBrush.GradientStops.Add(new GradientStop(Colors.DarkMagenta, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.MediumPurple, 0.5));

            Ellipse myEllipse = new Ellipse();
            myEllipse.Fill = gradientBrush;
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.DarkSlateBlue; // Outline color
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 2 * radius;
            myEllipse.Height = 2 * radius;
            myEllipse.SetCenter(x, y);

            // Add randomness to the berry's appearance
            Random random = new Random();
            double xOffset = (random.NextDouble() - 0.5) * radius;
            double yOffset = (random.NextDouble() - 0.5) * radius;
            myEllipse.SetCenter(x + xOffset, y + yOffset);

            canvas.Children.Add(myEllipse);
        }

        /* draws yellow pentagons on the fractal fern*/
        private void pentagon(int x, int y, double size, Canvas canvas)
        {
            SolidColorBrush pentagonColor = Brushes.LightGoldenrodYellow;
            double sideLength = 4; // Set a fixed size for the pentagon
            double rotationAngle = 72; // Angle between each side of the pentagon

            PointCollection pentagonPoints = new PointCollection();

            // Calculate the points of the pentagon
            for (int i = 0; i < 5; i++)
            {
                double angle = i * rotationAngle * Math.PI / 180;
                double xOffset = sideLength * Math.Cos(angle);
                double yOffset = sideLength * Math.Sin(angle);

                pentagonPoints.Add(new Point(x + xOffset, y + yOffset));
            }

            Polygon pentagon = new Polygon();
            pentagon.Fill = pentagonColor;
            pentagon.Points = pentagonPoints;

            canvas.Children.Add(pentagon);
        }


        /*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
        private void line(int x1, int y1, int x2, int y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }

        private void drawSun(Canvas canvas)
        {
            Rectangle sun = new Rectangle();
            sun.Width = 120;
            sun.Height = 120;

            Canvas.SetLeft(sun, 20);
            Canvas.SetTop(sun, 20);

            ImageBrush sunImg = new ImageBrush();
            sunImg.ImageSource = new BitmapImage(new Uri(@"..\..\data\sun.png", UriKind.Relative));
            sun.Fill = sunImg;
        
            canvas.Children.Add(sun);


        }


        private void drawGrass(Canvas canvas)
        {
            Rectangle grass= new Rectangle();
            grass.Width = canvas.Width;
            grass.Height = 100;

            ImageBrush grassImg = new ImageBrush();
            grassImg.ImageSource = new BitmapImage(new Uri(@"..\..\data\grass3.png", UriKind.Relative));

            grass.Fill = grassImg;
            Canvas.SetLeft(grass, 0);
            Canvas.SetBottom(grass, 0);
            canvas.Children.Add(grass);


        }
       
        private void ovals(Canvas canvas)
        {
            Random random = new Random();
            int numOvals = random.Next(80, 130); //  Adjust the range for the number of ovals

            double potBottom = (canvas.Height * 5 / 6) + 55; //Bottom of the pot(slightly over to make is look more natural)

            for (int i = 0; i < numOvals; i++)
            {
                double width = random.Next(10, 30);
                double height = random.Next(10, 20);
                double x = random.Next(0, (int)canvas.Width);
                double y = random.Next((int)canvas.Height / 2, (int)canvas.Height);

                // Ensure ovals do not go higher than the pot
                if (y - height / 2 < potBottom)
                {
                    y = potBottom + height / 2;
                }
                else if (y + height / 2 > canvas.Height)
                {
                    y = canvas.Height - height / 2;
                }

                byte red = (byte)random.Next(256);
                byte green = (byte)random.Next(256);
                byte blue = (byte)random.Next(256);

                Ellipse oval = new Ellipse();
                SolidColorBrush ovalBrush = new SolidColorBrush(Color.FromRgb(red, green, blue));
                oval.Fill = ovalBrush;
                oval.Width = width;
                oval.Height = height;
                oval.SetCenter(x, y);

                canvas.Children.Add(oval);
            }
        }
    }


    /*
     * this class is needed to enable us to set the center for an ellips
     */
    public static class EllipseX
    {
        public static void SetCenter(this Ellipse ellipse, double X, double Y)
        {
            Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
            Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
        }
    }
}