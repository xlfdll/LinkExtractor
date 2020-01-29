using System;
using System.Text.RegularExpressions;

namespace LinkExtractor.Handlers
{
    public class DmhyShareHandler : IHandler
    {
        public String Name => "share.dmhy.org";

        public String GetEmbedLinks(String content)
        {
            Match match = DmhyShareHandler.EmbedLinkRegex.Match(content);

            return match.Success ? match.Groups["EmbedLink"].Value : null;
        }

        public static readonly String URLPrefix = "https://share.dmhy.org/topics/view/";

        private static readonly Regex EmbedLinkRegex = new Regex(@"Magnet連接:.+?href=""(?<EmbedLink>magnet:.+?)"">",
            RegexOptions.Compiled);
    }
}