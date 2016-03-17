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
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static ViewModelLocator()
        {
            if(ViewModelBase.IsInDesignModeStatic)
            {
                main = new MainViewModel();
                configuration = new DesignConfigViewModel();
            }
            else
            {
                main = new MainViewModel();
                configuration = new ConfigViewModel();
            }
        }

        public ViewModelLocator()
        { }

        public IMainViewModel Main => main;
        private static readonly IMainViewModel main;

        public IConfigViewModel Configuration => configuration;
        private static readonly IConfigViewModel configuration;

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}