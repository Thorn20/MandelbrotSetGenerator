using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotSetProgram
{
    class MandelbrotSet
    {
        Rectangle SCREEN_SPACE;
        RectangleF MANDELBROT_SPACE = new RectangleF(-2.5f, -1.5f, 3.5f, 3.0f);

        public float focusZoom;
        public PointF focusPoint;
        public RectangleF focusArea;

        public int[,] exitValues;
        public Bitmap pixelValues;

        public MandelbrotSet(Rectangle screenSpace)
        {
            SCREEN_SPACE = screenSpace;

            focusZoom = 1.0f;
            focusPoint = new PointF(MANDELBROT_SPACE.X + (MANDELBROT_SPACE.Width / 2),
                                    MANDELBROT_SPACE.Y + (MANDELBROT_SPACE.Height / 2));
            focusArea = new RectangleF(MANDELBROT_SPACE.Location, MANDELBROT_SPACE.Size);

            exitValues = new int[SCREEN_SPACE.Width, SCREEN_SPACE.Height];
            pixelValues = new Bitmap(SCREEN_SPACE.Width, SCREEN_SPACE.Height);
        }

        public void Generate( PointF focus, float zoom = 0.0f, int maxIterationsPerPixel = 1000)
        {
            focusPoint = focus;
            focusZoom += zoom;
            SizeF focusHalfZoomed = new SizeF((focusArea.Width / 2)/focusZoom, (focusArea.Height / 2) / focusZoom);
            focusArea = new RectangleF(focusPoint - focusHalfZoomed, focusHalfZoomed + focusHalfZoomed);

            int[] iterationTallys = new int[maxIterationsPerPixel + 1];

            Parallel.For(0, exitValues.GetLength(0), px =>
            {
                Parallel.For(0, exitValues.GetLength(1), py =>
                {
                    double x0 = focusArea.X + ((focusArea.Width / SCREEN_SPACE.Width) * px);
                    double y0 = focusArea.Y + ((focusArea.Height / SCREEN_SPACE.Height) * py);

                    double x, y, x2, y2;
                    x = y = x2 = y2 = 0.0;

                    int iteration = 0;

                    while (x2 + y2 <= 4 && iteration < maxIterationsPerPixel)
                    {
                        y = (x + x) * y + y0;
                        x = x2 - y2 + x0;
                        x2 = x * x;
                        y2 = y * y;
                        iteration++;
                    }

                    exitValues[px, py] = iteration;

                    if (iteration < maxIterationsPerPixel)
                        iterationTallys[iteration]++;
                });
            });

            int total = exitValues.Length;
            double hue;
            int R, G, B;
            R = G = B = 0;

            for (int px = 0; px < exitValues.GetLength(0); px++)
                for (int py = 0; py < exitValues.GetLength(1); py++)
                {
                    if (exitValues[px, py] < maxIterationsPerPixel)
                    {
                        hue = 0.0;
                        for (int i = 0; i <= exitValues[px, py]; i++)
                            hue += (double)iterationTallys[i] / total;

                        R = (int)(255 * (hue * 0.33));
                        G = (int)(255 * (hue * 0.66));
                        B = (int)(255 * (hue * 0.99));

                        pixelValues.SetPixel(px, py, Color.FromArgb(R, G, B));
                        //pixelValues.SetPixel(px, py, Color.FromArgb((int)(Int32.MaxValue * hue)));
                    }
                    else pixelValues.SetPixel(px, py, Color.Black);
                }
        }
    }
}
