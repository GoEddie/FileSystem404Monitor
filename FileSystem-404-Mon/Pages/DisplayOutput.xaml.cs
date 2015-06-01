using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using _404;

namespace FileSystem_404_Mon.Pages
{
    internal static class NullExtensions
    {
        public static bool IsNull(this object src)
        {
            return src == null;
        }
    }

    /// <summary>
    ///     Interaction logic for DisplayOutput.xaml
    /// </summary>
    public partial class DisplayOutput : Page
    {
        private const int MaxDisplayedEntries = 1000;
        private readonly FileMonitor _monitor;
        private readonly ICollectionView _view;
        private ObservableCollection<MissingFile> _displayFiles;

        public DisplayOutput()
        {
            InitializeComponent();

            Dispatcher.Invoke(() => { _displayFiles = new ObservableCollection<MissingFile>(); });

            _monitor = new FileMonitor(DisplayFile, RemoveFile, AddEvent);

            _view = CollectionViewSource.GetDefaultView(_displayFiles);
            _view.Filter = FilterOutput;
            Files.ItemsSource = _view;
        }

        private bool FilterOutput(object obj)
        {
            var file = obj as MissingFile;
            if (file != null)
                return String.IsNullOrEmpty(Filter.Text) || file.Name.IndexOf(Filter.Text, StringComparison.OrdinalIgnoreCase) > 0;

            return false;
        }

        private void AddEvent(MissingFile missingFile, SearchEvent searchEvent)
        {
            Dispatcher.Invoke(() =>
            {
                lock (_displayFiles)
                {
                    if (missingFile.Lookups == null)
                    {
                        missingFile.Lookups = new ObservableCollection<SearchEvent>();
                    }
                    missingFile.Lookups.Insert(0, searchEvent);
                }
            });
        }

        private void RemoveFile(MissingFile file)
        {
            Dispatcher.Invoke(() =>
            {
                lock (_displayFiles)
                {
                    _displayFiles.Remove(file);
                }
            });
        }

        private void DisplayFile(MissingFile file)
        {
            Dispatcher.Invoke(() =>
            {
                lock (_displayFiles)
                {
                    _displayFiles.Insert(0, file);

                    while (_displayFiles.Count >= MaxDisplayedEntries)
                    {
                        _displayFiles.RemoveAt(MaxDisplayedEntries - 1);
                    }
                }
            });
        }

        private void ShowSearchedDirectories(object sender, SelectionChangedEventArgs e)
        {
            var file = Files.SelectedItem as MissingFile;

            Dispatcher.Invoke(() => { Lookups.ItemsSource = file.IsNull() ? null : file.Lookups; });
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            var t = new Task(() => _monitor.Start());
            t.Start();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            _monitor.Stop();
        }

        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
           _view.Refresh(); 
        }
    }
}