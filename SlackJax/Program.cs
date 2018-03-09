using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SlackAPI;
using Slackbot;
using Bot = Slackbot.Bot;


namespace SlackJax
{
    public static class Program
    {
        // https://api.slack.com/custom-integrations/legacy-tokens
        private static string token = System.Environment.GetEnvironmentVariable( "SLACKJAXTOKEN" );

        static Program()
        {
            SBot = new SlackAPI.Bot(){bot_access_token = token, name = "slackjax"};
        }

        public static SlackAPI.Bot SBot;

        public static void Main(Func<string,Task<string>> render)
        {

            var clientReady = new ManualResetEventSlim( false );
            var    client      = new SlackSocketClient( token );
            client.Connect
                ( ( connected ) => clientReady.Set()
                , () => { } );
            client.OnMessageReceived += async ( message ) =>
            {
                var mentioned = SlackMessage.FindMentionedUsers( message.text );
                if (!mentioned.Contains( client.MySelf.id ) || message.text.Contains( "uploaded" ))
                    return;
                var eq       = Regex.Replace( message.text, "^<@\\S+>", "" );
                var image    = await render( eq );
                client.UploadFile( r => { }, System.IO.File.ReadAllBytes( image ), image, new[]{message.channel});
            };
            clientReady.Wait();
        }
    }
}
