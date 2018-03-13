using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new TaskCompletionSource<bool>();
            //SlackJax.Program.RenderToFile( Renderer.GenerateImage );
            SlackJax.Program.RenderToUri( QuickLatex.Render);

            t.Task.Wait();

        }
    }
}
