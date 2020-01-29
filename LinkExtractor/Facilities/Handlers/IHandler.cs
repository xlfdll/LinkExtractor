using System;

namespace LinkExtractor.Handlers
{
    public interface IHandler
    {
        String Name { get; }

        String GetEmbedLinks(String content);
    }
}