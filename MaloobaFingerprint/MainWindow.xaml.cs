using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace MaloobaFingerprint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ConfigTabs.SelectionChanged += TabChanged;
        }

        // Change the tab header label text colour
        private void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(TabItem tab in ConfigTabs.Items)
                (tab.Header as Label).Foreground = tab.IsSelected ? Brushes.Black : Brushes.White;
        }
    }
}
