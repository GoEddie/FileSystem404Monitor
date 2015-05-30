using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _404;
namespace FileSystem_404_Mon.Pages
{
    static class NullExtensions
{
        public static bool IsNull(this object src)
        {
            return src == null;
        }
}

    /// <summary>
    /// Interaction logic for DisplayOutput.xaml
    /// </summary>
    public partial class DisplayOutput : Page
    {
        private const int MaxDisplayedEntries= 1000;

        public DisplayOutput()
        {
            InitializeComponent();

            _monitor = new FileMonitor(DisplayFile, RemoveFile);
            Files.ItemsSource = _displayFiles;
        }

        private void RemoveFile(MissingFile file)
        {
            Dispatcher.Invoke(() =>
            {
                _displayFiles.Remove(file);
            });
        }

        private readonly ObservableCollection<MissingFile> _displayFiles = new ObservableCollection<MissingFile>();

        private void DisplayFile(MissingFile file)
        {
            Dispatcher.Invoke(() =>
            {
                _displayFiles.Insert(0, file);

                while(_displayFiles.Count >= MaxDisplayedEntries)
                {
                    _displayFiles.RemoveAt(MaxDisplayedEntries-1);
                }
            });
        }

        private void ShowSearchedDirectories(object sender, SelectionChangedEventArgs e)
        {
            var file = Files.SelectedItem as MissingFile;
            
            Dispatcher.Invoke(() =>
            {
                Lookups.ItemsSource = file.IsNull() ? null : file.Lookups;
            });
        }

        private readonly FileMonitor _monitor;

        private void Start(object sender, RoutedEventArgs e)
        {
            var t = new Task(() => _monitor.Start());
            t.Start();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            _monitor.Stop();
        }
    }
}
