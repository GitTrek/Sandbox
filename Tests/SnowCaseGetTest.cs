using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Sandbox1.Tests
{
	public class SnowCaseGetTest : ISnowCaseGetTest
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public SnowCaseGetTest(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		private void SetClientAuthentication(HttpClient client)
		{
			var byteArray = Encoding.ASCII.GetBytes($"VSTSIntegration@aprimo.com:vsts");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
		}
		public string TestGet()
		{
			var url = "https://aprimodev.service-now.com/api/now/table/sn_customerservice_case/8ad70be9db16f700ead189ae3b961941?sysparm_display_value=all&sysparm_exclude_reference_link=true&sysparm_fields=u_mc_reproduction_steps";

			using (var httpClient = _httpClientFactory.CreateClient())
			{
				SetClientAuthentication(httpClient);

				using (var response = httpClient.GetAsync(url).Result)
				{
					return response.Content.ReadAsStringAsync().Result;
				}
			}
		}
	}
}
