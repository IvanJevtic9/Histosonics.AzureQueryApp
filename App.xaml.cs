using System.Windows;

namespace Histosonics.AzureQueryApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ViewModelLocator.Main.Disconnect();

            ViewModelLocator.Cleanup();
        }
    }
}
