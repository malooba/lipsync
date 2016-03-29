using System.Windows;
using MaloobaLipSync.ViewModel;

namespace MaloobaLipSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ViewModelLocator.Args = e.Args;
            new MainWindow().ShowDialog();
            Shutdown();
        }
    }
}
