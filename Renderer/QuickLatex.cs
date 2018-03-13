using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RestSharp;

namespace Renderer
{
    public static class QuickLatex
    {
        public static async Task<string> Render( string formula )
        {
            var client = new RestClient( "http://www.quicklatex.com" );
            IRestRequest request = new RestRequest("latex3.f", Method.POST);
            request = request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request = request.AddHeader("X-Requested-With", "XMLHttpRequest");

            // http://www.holoborodko.com/pavel/quicklatex/#comment-5906
            var preamble = 
@"\usepackage{amsmath}
\usepackage{amsfonts}
\usepackage{amssymb}";

            var context = new Dictionary<string,string>()
                          {
                              { "formula", formula },
                              { "fsize", "17px" },
                              { "fcolor", "000000" },
                              { "mode", "0" },
                              { "out", "1"},
                              { "preamble", preamble },
                          };

            string encode( string s )
            {
                s = s.Replace( "%", "%25" );
                s = s.Replace( "&", "%26" );
                return s;
            }
            var data = String.Join( "&", context.Select( kv => $"{kv.Key}={ encode(kv.Value) }" ) );
            request = request.AddParameter("text/plain", data, ParameterType.RequestBody);


            var response = await client.ExecutePostTaskAsync( request );
            return Parse( response.Content );
        }

        static string Parse( string response )
        {

            var lines =  response.Split( '\n' );
            if (lines.Length > 1 && lines[0].First() == '0')
            {
                var tokens = lines[1].Split( ' ' );
                var png = tokens[0];
                return png;
            }

            return null;
        }
    }
}
