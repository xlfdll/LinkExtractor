using System;
using System.Collections.Generic;
using System.Linq;

using LinkExtractor.Handlers;

namespace LinkExtractor
{
    public class HandlerDispatcher
    {
        public HandlerDispatcher()
        {
            HandlerDispatcher.Current = this;
        }

        private Dictionary<String, IHandler> LinkHandlers { get; }
            = new Dictionary<String, IHandler>();

        public void RegisterLinkHandler(String urlPrefix, IHandler handler)
        {
            this.LinkHandlers.Add(urlPrefix, handler);
        }

        public IHandler GetLinkHandler(String url)
        {
            return this.LinkHandlers.Where(pair => url.StartsWith(pair.Key))
                .Select(pair => pair.Value)
                .FirstOrDefault();
        }

        public Boolean IsLinkHandlerRegistered(String url)
        {
            return this.LinkHandlers.Keys.Where(key => url.StartsWith(key)).Count() > 0;
        }

        public static HandlerDispatcher Current { get; private set; }
    }
}