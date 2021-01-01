using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sandbox1.Tests;
using System;

namespace Sandbox1
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient();
			services.AddTransient<ISnowCaseGetTest, SnowCaseGetTest>();
			services.AddTransient<ITestComposite, TestComposite>();
		}

		public void Configure(IServiceProvider serviceProvider)
		{
		}
	}
}
