using System.Windows;

using LinkExtractor.Handlers;

namespace LinkExtractor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            HandlerDispatcher handlerDispatcher = new HandlerDispatcher();
            
            handlerDispatcher.RegisterLinkHandler(DmhyShareHandler.URLPrefix, new DmhyShareHandler());
        }
    }
}