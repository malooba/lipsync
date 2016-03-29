/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:XamlFingerprintAnalyser"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;

namespace MaloobaFingerprint.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static string[] Args { get; set; }
        public IMainViewModel Main => main ?? (main = new MainViewModel());

        private static IMainViewModel main;

        public IConfigViewModel Configuration
        {
            get
            {
                if(configuration == null)
                {
                    if(ViewModelBase.IsInDesignModeStatic)
                        configuration = new DesignConfigViewModel(null);
                    else
                        configuration = new ConfigViewModel(Args);
                }
                return configuration;
            }
        }

        private static IConfigViewModel configuration;

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}