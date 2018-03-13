using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Renderer;

namespace RendererSpecs
{
    public class QuickLatexSpecs
    {
        [Fact]
        public async Task CanReachQuickLatexWebService()
        {
            var formula = 
@"\begin{align*}
x^2   y^2 &= 1 \\
y &= \sqrt{1 - x^2}
\end{align*}";

            var foo = await QuickLatex.Render( formula );
            foo.Should().StartWith( "http" );
            foo.Should().EndWith(".png");

        }
    }
}
