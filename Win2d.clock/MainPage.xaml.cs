using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win2d.clock
{
    /// <summary>
    /// The main pane.
    /// </summary>
    public sealed partial class MainPage : Page {
        private Vector2 midScreen;
        private float ray;
        private float polarFix;
        private CanvasTextFormat font;

        public MainPage() {
            this.InitializeComponent();

            polarFix = (float)Math.PI / 2; // Polar coordinates starts at 0, puts the start of the pointers at
                                           // PI/2
            ray = (float)pane.ActualWidth; // Default ray, to be replaced when a layout changed event happens

            // A 2D vector pointing to the middle of the device screen.
            midScreen = new Vector2() {
                X = (float)pane.ActualWidth / 2,
                Y = (float)pane.ActualHeight / 2
            };

            font = new CanvasTextFormat() {
                FontSize = 10,
                FontFamily = "Times New Roman"
            };
        }

        // Draws a clock with given ray in the middle of a canvas
        private void drawClock(CanvasAnimatedDrawEventArgs canvas, Vector2 middle, float ray) {
            canvas.DrawingSession.DrawCircle(midScreen, ray, Colors.White);
            canvas.DrawingSession.DrawText("12", midScreen.X - 5, midScreen.Y - ray, Colors.White, font);
            canvas.DrawingSession.DrawText("3", midScreen.X + ray - 10, midScreen.Y - 5, Colors.White, font);
            canvas.DrawingSession.DrawText("6", midScreen.X - 2.5f, midScreen.Y + ray - 12, Colors.White, font);
            canvas.DrawingSession.DrawText("9", midScreen.X - ray + 5, midScreen.Y - 5, Colors.White, font);
        }

        // Receives the angle and color to draw a line in a canvas, from the middle of it pointing to a position in a clock of given ray
        private void drawClockPointer(CanvasAnimatedDrawEventArgs canvas, Vector2 middle, float ray, float angle, Color color) {
            // Using polar coordinates, draws a line from the middle of the canvas
            // to the desired position of the clock
            canvas.DrawingSession.DrawLine(midScreen.X, midScreen.Y, midScreen.X + (float)(ray * Math.Cos(angle)),
                                                                   midScreen.Y + (float)(ray * Math.Sin(angle)), color);
        }

        // Updates the Win2D canvas for each Draw operation, those are usually many times per second
        private void CanvasControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args) {
            // Calculates the angles using using the time spent for each measure of time.    
            float secAngle = (float)(DateTime.Now.Second * 6.28) / 60 - polarFix;
            float minAngle = (float)(DateTime.Now.Minute * 6.28 + secAngle) / 60 - polarFix;
            float houAngle = (float)(DateTime.Now.Hour * 6.28 / 12) + (minAngle / 60) - polarFix;

            drawClock(args, midScreen, ray);

            drawClockPointer(args, midScreen, ray, secAngle, Colors.Red);
            drawClockPointer(args, midScreen, ray, minAngle, Colors.Blue);
            drawClockPointer(args, midScreen, ray, houAngle, Colors.Green);
        }

        // Updates the coordiantes for middle screen and the clock's ray size to a possible layout change
        private void pane_LayoutUpdated(object sender, object e) {
            ray = (pane.ActualHeight > pane.ActualWidth) ? (float)pane.ActualWidth / 2 : (float)pane.ActualHeight / 2;
            midScreen.X = (float)pane.ActualWidth / 2;
            midScreen.Y = (float)pane.ActualHeight / 2;
        }

    }
}
