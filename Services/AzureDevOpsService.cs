using Histosonics.AzureQueryApp.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Histosonics.AzureQueryApp.Services
{
    public class AzureDevOpsService
    {
        private readonly HttpClient _client;

        public AzureDevOpsService()
        {
            _client = new HttpClient();
        }

        public async Task<bool> IsTokenValid(string email, string personalAccessToken)
        {
            var authenticationString = $"{email}:{personalAccessToken}";
            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            var url = "https://dev.azure.com/histosonics/_apis/projects?api-version=7.0";

            var checkRequest = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _client.SendAsync(checkRequest, HttpCompletionOption.ResponseHeadersRead);

            return response.IsSuccessStatusCode;
        }

        public async Task<ChangeSetResult> GetAllChangesets()
        {
            // request
            var getAllChagesetsUrl = "https://dev.azure.com/histosonics/Edison/_apis/tfvc/changesets/?$top=100000000&api-version=7.0";
            var getAllChangesetsRequest = new HttpRequestMessage(HttpMethod.Get, getAllChagesetsUrl);

            // response
            var responseMessage = await _client.SendAsync(
                getAllChangesetsRequest,
                HttpCompletionOption.ResponseHeadersRead);

            var body = await responseMessage.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ChangeSetResult>(body);
        }

        public async Task<CodeReviewResult> GetCodeReviewForChangeset(int changesetId)
        {
            var getAllcodeReviewRequestUrl = "https://dev.azure.com/histosonics/Edison/_apis/wit/wiql?api-version=7.0";
            var getAllCodeReviewRequest = new HttpRequestMessage(
                HttpMethod.Post,
                getAllcodeReviewRequestUrl);

            getAllCodeReviewRequest.Content = JsonContent.Create(new
            {
                query = string.Format(@"
                    SELECT
                        [System.Id],
                        [System.WorkItemType],
                        [System.Title],
                        [System.AssignedTo],
                        [System.State],
                        [System.Tags]
                    FROM workitems
                    WHERE
                        [System.TeamProject] = @project
                        AND [System.WorkItemType] = 'Code Review Request'
                        AND [Microsoft.VSTS.CodeReview.ContextType] = 'Changeset'
                        AND [Microsoft.VSTS.CodeReview.Context] = '{0}'
                    ORDER BY [System.Id]",
                        changesetId)
            });

            // response
            var responseMessage = await _client.SendAsync(
                getAllCodeReviewRequest,
                HttpCompletionOption.ResponseHeadersRead);

            var body = await responseMessage.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CodeReviewResult>(body);
        }

        public async Task<WorkItemQ> GetWorkItemDetails(int changesetId)
        {
            var workItemRequestUrl = $"https://dev.azure.com/histosonics/_apis/tfvc/changesets/{changesetId}/workItems?api-version=7.0";
            var workItemRequest = new HttpRequestMessage(HttpMethod.Get, workItemRequestUrl);

            var responseMessage = await _client.SendAsync(workItemRequest, HttpCompletionOption.ResponseHeadersRead);
            
            var body = await responseMessage.Content.ReadAsStringAsync();

            var workItemResult = JsonSerializer.Deserialize<WorkItemResult>(body);
            
            return workItemResult.value.FirstOrDefault();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
