using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Histosonics.AzureQueryApp.Commands;
using Histosonics.AzureQueryApp.Services;
using System.Windows.Controls;

namespace Histosonics.AzureQueryApp.ViewModels
{
    public class MainViewModel : ObservableRecipient
    {
        private bool isTokenValid = false;

        private string _email;
        private string _userToken;
        private string _logFilePath;
        private string _consoleOutput;

        private double _progress;

        private readonly AzureDevOpsService _azureDevOpsService;

        public MainViewModel()
        {
            if (!string.IsNullOrEmpty(Settings.Default.Email))
            {
                Email = Settings.Default.Email;
            }

            if (!string.IsNullOrEmpty(Settings.Default.UserToken))
            {
                UserToken = Settings.Default.UserToken;
            }


            _azureDevOpsService = new();
        }

        public string Email
        {
            get => _email;
            set
            {
                Settings.Default.Email = value;
                Settings.Default.Save();

                SetProperty(ref _email, value);
            }
        }

        public string UserToken
        {
            get => _userToken;
            set => SetProperty(ref _userToken, value);
        }

        public string LogFilePath
        {
            get => _logFilePath;
            set => SetProperty(ref _logFilePath, value);
        }

        public string ConsoleOutput
        {
            get => _consoleOutput;
            set
            {
                var appendText = string.IsNullOrEmpty(_consoleOutput) ? value : string.Concat(_consoleOutput, "\n", value);

                SetProperty(ref _consoleOutput, appendText);
            }
        }

        public double ProgressBar
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public RelayCommand ValidateTokenCommand => new RelayCommand(async () => await ValidateToken());
        public RelayCommand ExecuteGetChangeSetsWithoutCodeReviewQuery => new RelayCommand(async () => await GetChangeSetsWithoutCodeReviewQuery());

        public async Task ValidateToken()
        {
            if (await _azureDevOpsService.IsTokenValid(Email, UserToken))
            {
                isTokenValid = true;
                ConsoleOutput = "VALID TOKEN!";

                Settings.Default.UserToken = UserToken;
                Settings.Default.Save();
            }
            else
            {
                isTokenValid = false;
                ConsoleOutput = "INVALID TOKEN!";

                Settings.Default.UserToken = string.Empty;
                Settings.Default.Save();
            }
        }

        public async Task GetChangeSetsWithoutCodeReviewQuery()
        {
            if (!isTokenValid)
            {
                ConsoleOutput = "First validate token before execute query.";
                return;
            }

            if (string.IsNullOrEmpty(LogFilePath))
            {
                ConsoleOutput = "Please choose a file to store the results.";
                return;
            }

            ProgressBar = 0;

            ConsoleOutput = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] GetChangeSetsWithoutCodeReview has been started.";
            WeakReferenceMessenger.Default.Send(new LockCommand(true));

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("------------------------------ LOG - Get all Changesets without code reviews ------------------------------");

            var allChangeSets = await _azureDevOpsService.GetAllChangesets();

            int i = 1;
            foreach (var changeSet in allChangeSets.value)
            {
                ProgressBar = (double)(i * 100 / allChangeSets.value.Count);

                var result = await _azureDevOpsService.GetCodeReviewForChangeset(changeSet.changesetId);

                if (result.workItems.Count == 0)
                {
                    var item = await _azureDevOpsService.GetWorkItemDetails(changeSet.changesetId);

                    if (item is not null)
                    {
                        strBuilder.AppendLine(
                            $"ChangeSetID: {changeSet.changesetId} Author: {changeSet.author?.displayName} Task name: {item?.title} Task type: {item?.workItemType}");
                    }
                }

                ++i;
            }

            try
            {
                await File.AppendAllTextAsync(LogFilePath, strBuilder.ToString());
            }
            catch (System.Exception ex)
            {
                ConsoleOutput = ex.Message;
            }


            WeakReferenceMessenger.Default.Send(new LockCommand(false));
            ConsoleOutput = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] GetChangeSetsWithoutCodeReview has been finished.";
        }

        public void Disconnect()
        {
            _azureDevOpsService.Dispose();
        }
    }
}
