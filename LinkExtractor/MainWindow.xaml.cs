using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
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
            this.ProcessingLinkQueue = new ConcurrentQueue<String>();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ClipboardMonitor.ClipboardContentChanged += ClipboardMonitor_ClipboardContentChanged;

            Task.Run(ProcessLinkQueue);
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ClipboardMonitor.Dispose();
        }

        private void ClipboardMonitor_ClipboardContentChanged(object sender, EventArgs e)
        {
            if (!isCopyingLinks && Clipboard.ContainsText())
            {
                String clipboardText = Clipboard.GetText();

                if (clipboardText != lastClipboardText)
                {
                    String[] originalLinks = clipboardText.Split
                        (new String[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (String originalLink in originalLinks)
                    {
                        this.ProcessingLinkQueue.Enqueue(originalLink);

                        CountStatusBarItem.Content = String.Format(CountStatusBarItem.Tag.ToString(), this.ProcessingLinkQueue.Count);
                    }

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

        private void CopySelectedExtractedLinksButton_Click(object sender, RoutedEventArgs e)
        {
            isCopyingLinks = true;

            String links = String.Join(Environment.NewLine, LinkItemListView.SelectedItems.Cast<LinkItem>()
                .Select(item => item.ExtractedLink)
                .ToArray());

            Clipboard.SetText(links);

            isCopyingLinks = false;

            MainStatusBarItem.Content = $"Selected extracted links copied";
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

            lastClipboardText = String.Empty;

            MainStatusBarItem.Content = $"All extracted links cleared";
        }

        private void ProcessLinkQueue()
        {
            while (true)
            {
                while (this.ProcessingLinkQueue.TryDequeue(out String link))
                {
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBarStatusBarItem.Visibility = Visibility.Visible;
                        CountStatusBarItem.Content = String.Format(CountStatusBarItem.Tag.ToString(), this.ProcessingLinkQueue.Count);
                        MainStatusBarItem.Content = $"Parsing {link}...";
                    });

                    String content = WebOperations.GetContentAsStringAsync(link).Result;
                    IHandler handler = HandlerDispatcher.Current.GetLinkHandler(link);
                    String extractedLink = handler.GetEmbedLinks(content);

                    Dispatcher.Invoke(() =>
                    {
                        if (!String.IsNullOrEmpty(extractedLink))
                        {
                            LinkItemListView.Items.Add(new LinkItem(extractedLink, link, handler));
                        }

                        if (this.ProcessingLinkQueue.Count == 0)
                        {
                            MainStatusBarItem.Content = "Done";
                            ProgressBarStatusBarItem.Visibility = Visibility.Collapsed;
                        }
                    });
                }

                Thread.Sleep(100);
            }
        }

        private ClipboardMonitor ClipboardMonitor { get; }
        private ConcurrentQueue<String> ProcessingLinkQueue { get; }

        private String lastClipboardText = String.Empty;
        private Boolean isCopyingLinks = false;
    }
}