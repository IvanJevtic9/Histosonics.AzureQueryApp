using Histosonics.AzureQueryApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace Histosonics.AzureQueryApp
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton<MainViewModel>()
                    .BuildServiceProvider());
        }

        public static MainViewModel Main
        {
            get
            {
                return Ioc.Default.GetService<MainViewModel>();
            }
        }

        public static void Cleanup()
        {
            Main.IsActive = false;
        }
    }
}
