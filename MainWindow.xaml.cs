using System.IO;
using System.Windows;
using Microsoft.Win32;
using Histosonics.AzureQueryApp.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Histosonics.AzureQueryApp.Commands;
using System;

namespace Histosonics.AzureQueryApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
            WeakReferenceMessenger.Default.Register<LockCommand>(this, HandleLockCommand);
        }

        private void HandleLockCommand(object recipient, LockCommand message)
        {
            this.IsEnabled = !message.Lock;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                ((MainViewModel)DataContext).LogFilePath = saveFileDialog.FileName;
                File.WriteAllText(saveFileDialog.FileName, string.Empty);
            }
        }
    }
}
