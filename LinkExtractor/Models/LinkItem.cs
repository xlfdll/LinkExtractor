using System;

using LinkExtractor.Handlers;

namespace LinkExtractor
{
    public class LinkItem
    {
        public LinkItem(String extractedLink, String source, IHandler handler)
        {
            this.ExtractedLink = extractedLink;
            this.Source = source;
            this.Handler = handler;
        }

        public String ExtractedLink { get; }
        public String Source { get; }
        public IHandler Handler { get; }
    }
}