/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:XamlCorrelator"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;

namespace MaloobaLipSync.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public IMainViewModel Main => main ?? (main = new MainViewModel());

        private static IMainViewModel main ;

        public IConfigViewModel Configuration => configuration ?? (configuration = new ConfigViewModel(Args));
        public static string[] Args { get; set; }

        private static IConfigViewModel configuration;

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}