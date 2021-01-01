using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Sandbox1
{
	class VstsApiTestService
	{
		private static readonly string _serializedPatchDoc = @"[{""From"":"""",""op"":""Add"",""Path"":""/fields/System.AreaPath"",""Value"":""ServiceNowIntegration\\Bugs""},{""From"":"""",""op"":""Add"",""Path"":""/fields/System.Tags"",""Value"":""PM_Reviewed""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.ProductLine"",""Value"":""Aprimo""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.IncidentManagementSystemID"",""Value"":""CS0027602""},{""From"":"""",""op"":""Add"",""Path"":""/fields/System.Title"",""Value"":""AprimoDevTest: evard""},{""From"":"""",""op"":""Add"",""Path"":""/fields/Microsoft.VSTS.Common.Priority"",""Value"":""3""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.Client"",""Value"":""Test Account 1""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.AnalystName"",""Value"":""Root Meg Miller""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.SaaS"",""Value"":""Yes""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.SiteID"",""Value"":""TestAccount1""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.ApplicationUrl"",""Value"":""test.test""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.Module"",""Value"":""zzz_SNOW_VSTS_Test""},{""From"":"""",""op"":""Add"",""Path"":""/fields/Custom.IncidentManagementSystemSyncType"",""Value"":""Major Case""},{""From"":"""",""op"":""Add"",""Path"":""/fields/Microsoft.VSTS.Scheduling.TargetDate"",""Value"":""4/20/2020 9:40:51 PM""},{""From"":"""",""op"":""Add"",""Path"":""/fields/MODevelopmentProcess.RedZone"",""Value"":""None""}]";
		public async Task ReadWorkItem()
		{
			using (var connection = new VssConnection(new Uri("https://dev.azure.com/marketingops"),
				new VssBasicCredential(string.Empty, "uq74dz3l5upjjnv6fzuy2tl4kv44afjyx3kz7elp3nb3im2vipxa")))
			using (var client = connection.GetClient<WorkItemTrackingHttpClient>())
			{
				var workItem = await client.GetWorkItemAsync(222384);

				foreach (var field in workItem.Fields)
					Console.WriteLine($"{field.Key}:{field.Value}");
			}
		}
		public async Task CreateWorkItem()
		{
			using (var connection = new VssConnection(new Uri("https://dev.azure.com/marketingops"),
				new VssBasicCredential(string.Empty, "uq74dz3l5upjjnv6fzuy2tl4kv44afjyx3kz7elp3nb3im2vipxa")))
			using (var client = connection.GetClient<WorkItemTrackingHttpClient>())
			{
				try
				{
					var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument>(_serializedPatchDoc);

					// mess with patchDoc here
					patchDoc.Add(new JsonPatchOperation()
					{
						Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
						Path = "/fields/xyz",
						Value = "test val"
					});

					var workItem = await client.CreateWorkItemAsync(patchDoc, "ServiceNowIntegration", "Bug", null, false, null, null, false);

					Console.WriteLine($"ID is {workItem.Id} for new Work Item");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error:");
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}
				Console.WriteLine("end");
			}
		}
	}
}
