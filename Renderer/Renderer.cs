using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMath.Controls;
using Brushes = System.Windows.Media.Brushes;
using Size = System.Windows.Size;

namespace Renderer
{
    public static class Renderer
    {
        private static readonly StaTaskScheduler _StaTaskScheduler = new StaTaskScheduler( 1 );

        public static Bitmap Convert( RenderTargetBitmap inmap )
        {
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(inmap));
            encoder.Save(stream);

            Bitmap bitmap = new Bitmap(stream);
            return bitmap;
        }

        public static async Task<string> GenerateImage(string formula)
        {
            string Build()
            {

                var control = new FormulaControl
                              {
                                  Formula    = formula
                                , Background = Brushes.White
                              };

                control.Measure(new Size(300, 300));
                control.Arrange(new Rect(new Size(300, 300)));

                var bmp = new RenderTargetBitmap(300, 300, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(control);

                var image = ImageCrop.AutoCrop( Convert( bmp ) );
                var file = @"test.png";
                image.Save( file,ImageFormat.Png );
                return file;

            }

            return await Task.Factory.StartNew
                ( Build, CancellationToken.None, TaskCreationOptions.None, _StaTaskScheduler );
        }
    }
}