using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotSetProgram
{
    class Program : Form
    {
        const int RENDER_WIDTH = 1120;
        const int RENDER_HEIGHT = 960;
        Rectangle RENDER_WINDOW = new Rectangle(0, 0, RENDER_WIDTH, RENDER_HEIGHT);

        const int ITERATIONS_PER_PIXEL = 1000;

        BufferedGraphicsContext gfxContext;
        BufferedGraphics gfxBuffer;
        Graphics gfx;

        MandelbrotSet mandelbrotSet;

        public Program() : base()
        {
            this.Text = "Mandelbrot Set";
            this.Size = new Size(RENDER_WIDTH, RENDER_HEIGHT);
            this.CenterToScreen();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.MouseDoubleClick += new MouseEventHandler(WindowDoubleClick);

            mandelbrotSet = new MandelbrotSet(RENDER_WINDOW);
            mandelbrotSet.Generate( mandelbrotSet.focusPoint);

            InitializeGraphics();           
            RenderWindow();           
        }

        void InitializeGraphics()
        {
            gfxContext = BufferedGraphicsManager.Current;
            gfxContext.MaximumBuffer = this.Size;
            if (gfxBuffer != null)
            {
                gfxBuffer.Dispose();
                gfxBuffer = null;
            }            
            gfxBuffer = gfxContext.Allocate(this.CreateGraphics(), RENDER_WINDOW);            
        }

        void RenderWindow()
        {
            gfx = gfxBuffer.Graphics;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            gfx.FillRectangle(new SolidBrush(Color.Black), RENDER_WINDOW);
            gfx.DrawImage(mandelbrotSet.pixelValues, 0, 0);
            gfxBuffer.Render(Graphics.FromHwnd(Handle));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            gfxBuffer.Render(e.Graphics);
        }

        void WindowDoubleClick( object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF focus = new PointF(mandelbrotSet.focusArea.X + (mandelbrotSet.focusArea.Width * ((float)e.X / RENDER_WINDOW.Width)),
                                          mandelbrotSet.focusArea.Y + (mandelbrotSet.focusArea.Height * ((float)e.Y / RENDER_WINDOW.Height)));

                mandelbrotSet.Generate(focus, 1.0f / mandelbrotSet.focusZoom);
                RenderWindow();
            }
        }

        static void Main(string[] args)
        {
            Application.Run(new Program());
        }
    }
}
