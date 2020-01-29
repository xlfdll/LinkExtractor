using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Xlfdll.Network;
using Xlfdll.Windows.Presentation;

using LinkExtractor.Handlers;

namespace LinkExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.ClipboardMonitor = new ClipboardMonitor(this);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ClipboardMonitor.ClipboardContentChanged += ClipboardMonitor_ClipboardContentChanged;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ClipboardMonitor.Dispose();
        }

        private async void ClipboardMonitor_ClipboardContentChanged(object sender, EventArgs e)
        {
            if (!isCopyingLinks && Clipboard.ContainsText())
            {
                String clipboardText = Clipboard.GetText();

                if (clipboardText != lastClipboardText)
                {
                    await ParseClipboardLink(clipboardText);

                    MainStatusBarItem.Content = "Done";

                    lastClipboardText = clipboardText;
                }
            }
        }

        private void CopySourceButton_Click(object sender, RoutedEventArgs e)
        {
            LinkItem linkItem = LinkItemListView.SelectedItem as LinkItem;

            if (linkItem != null)
            {
                isCopyingLinks = true;
                Clipboard.SetText(linkItem.Source);
                isCopyingLinks = false;

                MainStatusBarItem.Content = $"Source link copied";
            }
        }

        private void CopyExtractedLinkButton_Click(object sender, RoutedEventArgs e)
        {
            LinkItem linkItem = LinkItemListView.SelectedItem as LinkItem;

            if (linkItem != null)
            {
                isCopyingLinks = true;
                Clipboard.SetText(linkItem.ExtractedLink);
                isCopyingLinks = false;

                MainStatusBarItem.Content = $"Extracted link copied";
            }
        }

        private void CopyAllExtractedLinksButton_Click(object sender, RoutedEventArgs e)
        {
            isCopyingLinks = true;

            String links = String.Join(Environment.NewLine, LinkItemListView.Items.Cast<LinkItem>()
                .Select(item => item.ExtractedLink)
                .ToArray());

            Clipboard.SetText(links);

            isCopyingLinks = false;

            MainStatusBarItem.Content = $"All extracted links copied";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            LinkItemListView.Items.Clear();

            MainStatusBarItem.Content = $"All extracted links cleared";
        }

        private async Task ParseClipboardLink(string clipboardText)
        {
            MainStatusBarItem.Content = $"Parsing {clipboardText}...";

            try
            {
                String content = await WebOperations.GetContentAsStringAsync(clipboardText);
                IHandler handler = HandlerDispatcher.Current.GetLinkHandler(clipboardText);
                String extractedLink = handler.GetEmbedLinks(content);

                LinkItemListView.Items.Add(new LinkItem(extractedLink, clipboardText, handler));

                MainStatusBarItem.Content = "Done";
            }
            catch (Exception ex)
            {
                MainStatusBarItem.Content = $"ERROR: {ex.Message}";
            }
        }

        private ClipboardMonitor ClipboardMonitor { get; }

        private String lastClipboardText = String.Empty;
        private Boolean isCopyingLinks = false;
    }
}