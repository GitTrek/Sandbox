using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sandbox1.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Json = System.Text.Json;

namespace Sandbox1
{
	class TestComposite : ITestComposite
	{
		private readonly ISnowCaseGetTest _snowCaseGetTest;

		public TestComposite(ISnowCaseGetTest snowCaseGetTest)
		{
			_snowCaseGetTest = snowCaseGetTest;
		}

		[Tool(1, "Null Cast")]
		public void TestNullCast()
		{
			Console.WriteLine($"[{null as string}]");
			Console.WriteLine($"[{(null as string) is ""}]");
		}

		[Tool(2, "IDictionary TryGet")]
		public void TestIDictionaryTryGet()
		{
			var colorList = new Dictionary<string, object>();

			var containsColor = colorList.TryGetValue("blue", out object color);

			Console.WriteLine($"[{color}]");
			Console.WriteLine($"[{color is null}]");
		}

		[Tool(3, "String Trim")]
		public void TestStringTrim()
		{
			var text = "abc def ghi";

			Console.WriteLine(text);
			Console.WriteLine(text.TrimStart("bc".ToCharArray()));
			Console.WriteLine(text.TrimStart("abc".ToCharArray()));
		}

		private string GetPatchString(JsonPatchDocument patch)
		{
			var patchString = new StringBuilder();
			patchString.Append("{\"operations\":[");

			foreach (var operation in patch)
			{
				patchString.Append("{");
				patchString.Append($"\"From\":\"{operation.From}\",");
				patchString.Append($"\"Operation\":\"{operation.Operation}\",");
				patchString.Append($"\"Path\":\"{operation.Path}\",");
				patchString.Append($"\"Value\":\"{operation.Value}\"");
				patchString.Append("},");
			}

			patchString.Remove(patchString.Length - 1, 1); // drop comma
			patchString.Append("]}");

			return patchString.ToString();
		}

		[Tool(4, "PatchDocument.ToString")]
		public void TestPatchDocToString()
		{
			var patch = new JsonPatchDocument();

			patch.Add(new JsonPatchOperation
			{
				Operation = Operation.Add,
				Path = "blabPath",
				Value = "blabVal"
			});
			patch.Add(new JsonPatchOperation
			{
				Operation = Operation.Add,
				Path = "blabPath2",
				Value = "blabVal2"
			});
			Console.WriteLine(GetPatchString(patch));
		}

		[Tool(5, "Read WorkItem")]
		public async Task TestReadWorkItem()
		{
			var vstsTestService = new VstsApiTestService();

			await vstsTestService.ReadWorkItem();
		}

		[Tool(6, "Create WorkItem")]
		public async Task TestCreateWorkItem()
		{
			var vstsTestService = new VstsApiTestService();

			await vstsTestService.CreateWorkItem();
		}

		[Tool(7, "Test Logging")]
		public void TestLogging()
		{
			//Console.
		}

		/// <summary>
		/// check if the two enumerables are equivalent.  Effectively, check if they hold the same values.  
		/// </summary>
		private bool EnumerablesAreEquivalent<T>(IEnumerable<T> firstEnumerable, IEnumerable<T> secondEnumerable)
		{
			return
				!firstEnumerable.Except(secondEnumerable).Any() && // does second have any differences
				!secondEnumerable.Except(firstEnumerable).Any() && // does first have any differences (redundant?)
				firstEnumerable.Count() == secondEnumerable.Count() && // are they the same size?
				firstEnumerable.Intersect(secondEnumerable).Count() == secondEnumerable.Count(); // does where they match equate to their size?
		}

		[Tool(8, "List Compare")]
		public void TestListCompare()
		{
			var firstDictionary = new Dictionary<string, string>
			{
				{ "a", "1" },
				{ "c", "3" },
				{ "b", "2" }
			};
			var secondDictionary = new Dictionary<string, string>
			{
				{ "c", "3" },
				{ "a", "1" },
				{ "b", "2" }
			};
			var thirdDictionary = new Dictionary<string, string>
			{
				{ "a", "5" },
				{ "c", "3" },
				{ "b", "2" }
			};
			var fourthDictionray = new Dictionary<string, string>
			{
				{ "a", "1" },
				{ "c", "3" },
				{ "b", "2" },
				{ "d", "4" }
			};

			Console.WriteLine($"Object.Equals: {firstDictionary.Equals(secondDictionary)}");
			Console.WriteLine($"Enumerable.SequenceEqual: {firstDictionary.SequenceEqual(secondDictionary)}");

			Console.WriteLine($"firstDictionary.Except: {!firstDictionary.Except(secondDictionary).Any()}");
			Console.WriteLine($"secondDictionary.Except: {!secondDictionary.Except(firstDictionary).Any()}");
			Console.WriteLine($"firstDictionary.Intersect.Except: {firstDictionary.Intersect(secondDictionary).Count() == secondDictionary.Count()}");

			Console.WriteLine($"first compared to second: {EnumerablesAreEquivalent(firstDictionary, secondDictionary)}");
			Console.WriteLine($"first compared to third: {EnumerablesAreEquivalent(firstDictionary, thirdDictionary)}");
			Console.WriteLine($"first compared to fourth: {EnumerablesAreEquivalent(firstDictionary, fourthDictionray)}");
		}

		class Box
		{
			public Colors Color { get; set; }
		}

		enum Colors
		{
			red = 1,
			blue = 2,
			green = 3
		}
		[Tool(9, "Null Coalescing")]
		public void TestNullCoalescing()
		{
			Colors? color = Colors.blue;
			Console.WriteLine(color ?? Colors.green);
			color = null;
			Console.WriteLine(color ?? Colors.green);
			var box = new Box();
			Console.WriteLine(box?.Color ?? Colors.green);
			var boxColor = box?.Color;
			Console.WriteLine(boxColor);
		}

		private JsonPatchDocument GetDoc(string fieldName, string fieldValue)
		{
			var document = new JsonPatchDocument();
			document.Add(new JsonPatchOperation()
			{
				Operation = Operation.Add,
				Path = $"/fields/{fieldName}",
				Value = fieldValue
			});

			return document;
		}
		private WorkItem UpdateField(string fieldName, string fieldValue)
		{
			var cred = new VssBasicCredential(string.Empty, "uq74dz3l5upjjnv6fzuy2tl4kv44afjyx3kz7elp3nb3im2vipxa");
			using (var conn = new VssConnection(new Uri("https://dev.azure.com/marketingops"), cred))
			using (var client = conn.GetClient<WorkItemTrackingHttpClient>())
			{
				WorkItem workItemResult = null;

				try
				{
					// stay synchronous for now.
					workItemResult = client.UpdateWorkItemAsync(GetDoc(fieldName, fieldValue), 226552, bypassRules: true).Result;
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($"Error:{ex.Message}");
				}

				if (workItemResult != null)
					Console.WriteLine("update succeeded");

				return workItemResult;
			}
		}
		public static string GetValueOfSNFieldWithSpecialChars(string fieldValue)
		{
			var newValue = !String.IsNullOrEmpty(fieldValue) ? fieldValue : null;
			if (newValue != null)
			{
				var utfBytes = Encoding.Default.GetBytes(fieldValue.Replace(Convert.ToString((char)10), "<BR>").Replace(Convert.ToString((char)13), String.Empty));
				newValue = Encoding.UTF8.GetString(utfBytes);
			}
			return newValue;
		}

		[Tool(10, "Test VSTS Work Item API")]
		public void TestVstsWorkItemAPI()
		{
			var supportTestString = "Reproduced on QA3.<br><br>Copying the steps Joris provided on the case, as I can confirm the result. Please find PSD file and dev tools error in the artifact location folder.<br><br>one more note: I tested this a bit more to confirm what Joris reports, and using<br> <br><span>&nbsp;&lt;add name=&quot;16:9&quot; outputSize=&quot;aspectRatio&quot; width=&quot;16&quot; height=&quot;9&quot; format=&quot;jpg&quot; resolution=&quot;72&quot; colorSpace=&quot;RGB&quot; /&gt;<br></span><div>instead of<br></div><div>&nbsp;&lt;add name=&quot;16:9&quot; outputSize=&quot;aspectRatio&quot; width=&quot;16&quot; height=&quot;9&quot; /&gt;&nbsp;</div>  <br><br>worked as expected.<br><br>----------<br><br>Steps to reproduce:<br>1. Logon to the System Studio as administrator<br><div>2. Change the .downloadOrderPresets  setting and include a preset that does not include the format attribute e.g.</div><div>&nbsp;&lt;<span>add name=&quot;16:9&quot; outputSize=&quot;aspectRatio&quot; width=&quot;16&quot; height=&quot;9&quot; /&gt; </span><br></div><br>(format is optional according to the documentation: https://jostens.dam.aprimo.com/System/WebHelp/ADAMHelp.htm#Admin%20Guide/Ordering/CroppedImagePresets.htm)<br><br>3. Go to the Assets Studio and Upload a PSD file<br>4. Make the new Asset available and add it to your basket<br>5. Submit a maintenance job to (re)create preset cropped images<br><br>Expected behavior:<br>The system generates a crop with the same extension as the original file and i'm able to explore the cropped images in the new UI<br><br>Actual behavior:<br>A cropped PSD seems to be created, but no preview as additional file<br>When navigating to the asset in the New DAM UI, you get a blank screen and using the web developer toolbar you can see the browser logs the error";

			var convertedTestString = GetValueOfSNFieldWithSpecialChars(supportTestString);
			//await UpdateField("System.History", "hello &lt;br&gt; world");
			UpdateField("Microsoft.VSTS.TCM.ReproSteps", convertedTestString);
		}
		[Tool(11, "Test Snow Get API")]
		public void TestSnowCaseGet()
		{
			Console.WriteLine(_snowCaseGetTest.TestGet());
		}
		[Tool(12, "Test Encoding")]
		public void TestEncoding()
		{
			var decodedXmlString = "<p>hello world123</p><br><br>";
			var encodedXmlString = "&lt;p&gt;hello world123&lt;/p&gt;&lt;br&gt;&lt;br&gt;";

			Console.WriteLine($"test encoding: {HttpUtility.HtmlEncode(decodedXmlString)}");
			Console.WriteLine($"test decoding: {HttpUtility.HtmlDecode(encodedXmlString)}");
		}
		[Tool(13, "Test Convert Char")]
		public void TestConvertChar()
		{
			var fieldValue =
@"Test
new
lines";
			var convertedValue = fieldValue.Replace(Convert.ToString((char)10), "<BR>").Replace(Convert.ToString((char)13), String.Empty);

			Console.WriteLine(convertedValue);

			var utfBytes = Encoding.Default.GetBytes(convertedValue);

			Console.WriteLine(Encoding.UTF8.GetString(utfBytes));
		}
		[Tool(14, "Test Regex Or")]
		public void TestRegexOr()
		{
			var text =
@"test
this

integration
<abc>test</abc>

one
two
three";

			var regex = new Regex(@"(.*\n|.*\z)");

			var textReplaced = regex.Replace(text, new MatchEvaluator((Match m) =>
			{
				return $"<div>{m}</div>";
			}));

			Console.WriteLine(textReplaced);
		}
		public string JsonTestString => @"{
    ""schemaId"": ""azureMonitorCommonAlertSchema"",
    ""data"": {
        ""essentials"": {
            ""alertId"": ""/subscriptions/3a0b2801-2ab5-4b2d-8ce7-426aa48f826f/providers/Microsoft.AlertsManagement/alerts/af4e328a-ce74-4d2c-9900-95d97988c181"",
            ""alertRule"": ""generalwebhooktest"",
            ""severity"": ""Sev3"",
            ""signalType"": ""Log"",
            ""monitorCondition"": ""Fired"",
            ""monitoringService"": ""Application Insights"",
            ""alertTargetIDs"": [
                ""/subscriptions/3a0b2801-2ab5-4b2d-8ce7-426aa48f826f/resourcegroups/prod-03us1-cloudmanager-rg/providers/microsoft.insights/components/prod-03us1-cloudmanager-rc-insights""
            ],
            ""originAlertId"": ""1f480629-b52f-4cf6-b96e-2d8003bfdcdb"",
            ""firedDateTime"": ""2020-03-26T18:58:45.8793401Z"",
            ""description"": """",
            ""essentialsVersion"": ""1.0"",
            ""alertContextVersion"": ""1.1""
        },
        ""alertContext"": {
            ""SearchQuery"": ""traces \n| where timestamp > ago(1h)\n| where operation_Name == 'Alerting-Health-Check'\n| summarize count() "",
            ""SearchIntervalStartTimeUtc"": ""2020-03-26T17:58:44Z"",
            ""SearchIntervalEndtimeUtc"": ""2020-03-26T18:58:44Z"",
            ""ResultCount"": 1,
            ""LinkToSearchResults"": ""https://portal.azure.com#@d05954c1-36eb-40b2-8f23-7f2ce352faf6/blade/Microsoft_OperationsManagementSuite_Workspace/AnalyticsBlade/initiator/AnalyticsShareLinkToQuery/isQueryEditorVisible/true/scope/%7B%22resources%22%3A%5B%7B%22resourceId%22%3A%22%2Fsubscriptions%2F3a0b2801-2ab5-4b2d-8ce7-426aa48f826f%2FresourceGroups%2Fprod-03us1-cloudmanager-rg%2Fproviders%2Fmicrosoft.insights%2Fcomponents%2Fprod-03us1-cloudmanager-rc-insights%22%7D%5D%7D/query/traces%20%0A%7C%20where%20timestamp%20%3E%20%28datetime%282020-03-26T18%3A58%3A44.0000000%29%20-%201h%29%0A%7C%20where%20operation_Name%20%3D%3D%20%27Alerting-Health-Check%27%0A%7C%20summarize%20count%28%29%20/isQuerybase64Compressed/false/timespanInIsoFormat/2020-03-26T17%3a58%3a44.0000000Z%2f2020-03-26T18%3a58%3a44.0000000Z"",
            ""LinkToSearchResultsApi"": ""https://api.applicationinsights.io/v1/apps/7f03b0f6-83cc-4262-9429-a66985fd0bd1/query?query=traces%20%0A%7C%20where%20timestamp%20%3E%20%28datetime%282020-03-26T18%3A58%3A44.0000000%29%20-%201h%29%0A%7C%20where%20operation_Name%20%3D%3D%20%27Alerting-Health-Check%27%0A%7C%20summarize%20count%28%29%20×pan=2020-03-26T17%3a58%3a44.0000000Z%2f2020-03-26T18%3a58%3a44.0000000Z"",
            ""SearchIntervalDurationMin"": ""60"",
            ""AlertType"": ""Number of results"",
            ""IncludeSearchResults"": true,
            ""SearchIntervalInMinutes"": ""60"",
            ""SearchResults"": {
                ""tables"": [
                    {
                        ""name"": ""PrimaryResult"",
                        ""columns"": [
                            {
                                ""name"": ""count_"",
                                ""type"": ""long""
                            }
                        ],
                        ""rows"": [
                            [
                                236
                            ]
                        ]
                    }
                ],
                ""dataSources"": [
                    {
                        ""resourceId"": ""/subscriptions/3a0b2801-2ab5-4b2d-8ce7-426aa48f826f/resourcegroups/prod-03us1-cloudmanager-rg/providers/microsoft.insights/components/prod-03us1-cloudmanager-rc-insights"",
                        ""region"": ""eastus"",
                        ""tables"": [
                            ""traces""
                        ]
                    }
                ]
            },
            ""Threshold"": 0,
            ""Operator"": ""Greater Than"",
            ""ApplicationId"": ""7f03b0f6-83cc-4262-9429-a66985fd0bd1"",
            ""IncludedSearchResults"": ""True""
        }
    }
}";
		[Tool(15, "Test Json")]
		public void TestJson()
		{
			var path = "$.data.essentials.alertRule";
			//var schema = JsonSchema.Parse(JsonTestString);
			var jObj = JsonConvert.DeserializeObject<JObject>(JsonTestString);
			var value = jObj.SelectToken(path).Value<string>();

			Console.WriteLine(value);
		}
		public string PatchDocumentJsonForNewWorkItemString => @"{
    ""operations"": [
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/System.AreaPath"",
            ""Value"":""MarketingOperations\\MO Team Team42""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.IncidentManagementSystemID"",
            ""Value"": ""CS0029352""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/System.Title"",
            ""Value"": ""TEST this is a test - team 42 vsts creation and routing""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/Microsoft.VSTS.Common.Priority"",
            ""Value"": ""4""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.Client"",
            ""Value"": ""Test Account 1""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.AnalystName"",
            ""Value"": ""Root Meg Miller""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.SaaS"",
            ""Value"": ""Yes""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.SiteID"",
            ""Value"": ""TestAccount1""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.ApplicationUrl"",
            ""Value"": ""test.test""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/System.Description"",
            ""Value"": ""this is a test to see where and what type of item is created in vsts when team 42 is selected in SN. ""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.IncidentManagementTaskID"",
            ""Value"": ""CSTASK0010262""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/Custom.IncidentManagementSystemSyncType"",
            ""Value"": ""Tasks""
        },
        {
            ""From"": """",
            ""Operation"": ""Add"",
            ""Path"": ""/fields/MODevelopmentProcess.Module"",
            ""Value"": ""REQUEST - ServiceNow""
        }
    ]
}";

		private string GetPatchDocumentValue(string areaPath)
		{
			var path = $"$.operations[?(@.Path == '{areaPath}')].Value";
			var jObj = JsonConvert.DeserializeObject<JObject>(PatchDocumentJsonForNewWorkItemString);

			return jObj.SelectToken(path).Value<string>(); ;
		}
		[Tool(16, "Test Patch Doc")]
		public void TestPatchDocJson()
		{
			Console.WriteLine(GetPatchDocumentValue("/fields/MODevelopmentProcess.IncidentManagementSystemID"));
			Console.WriteLine(GetPatchDocumentValue("/fields/MODevelopmentProcess.IncidentManagementTaskID"));
		}
		[Tool(17, "Test Deserialize Fail")]
		public void TestDeserializeFail()
		{
			var jsonStringMisc = "{\"test\":\"one\"}";
			var jsonStringCat = "{\"Name\":\"Socks\", \"Breed\":\"Siamese\"}";
			var resultObj = Json.JsonSerializer.Deserialize<Cat>(jsonStringMisc);
			Console.WriteLine(resultObj);
			resultObj = Json.JsonSerializer.Deserialize<Cat>(jsonStringCat);
			Console.WriteLine(resultObj);
		}
		private JsonPatchDocument GetPatchDocWithHtml()
		{
			var document = new JsonPatchDocument();
			document.Add(new JsonPatchOperation()
			{
				Operation = Operation.Add,
				Path = "/fields/System.Title",
				Value = "p17us1index001 - INF - DAM Disk Space Less Than 20 Percent on Non Shared Utility Server Was Triggered For Being Below 20"
			});
			document.Add(new JsonPatchOperation()
			{
				Operation = Operation.Add,
				Path = "/fields/System.Description",
				Value = "fake val"
			});
			document.Add(new JsonPatchOperation()
			{
				Operation = Operation.Add,
				Path = "/fields/MODevelopmentProcess.IncidentManagementSystemIDttt",
				Value = "CS001234"
			});

			return document;
		}
		[Tool(18, "Test Patch Doc Iteration")]
		public void TestPatchDocIteration()
		{
			var areaPath = "/fields/MODevelopmentProcess.IncidentManagementSystemID";
			var patchDoc = GetPatchDocWithHtml();
			var caseId = patchDoc.Where(op => op.Path == areaPath).Select(op => op.Value).FirstOrDefault();

			Console.WriteLine(caseId);
		}
	}

	class Cat
	{
		public string Name { get; set; }
		public string Breed { get; set; }
	}
}
