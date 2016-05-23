//Copyright 2016 Malooba Ltd

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

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
