using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sandbox1
{
	class Program
	{
		static void Main(string[] args)
		{
			// setup IoC
			var services = new ServiceCollection();
			var startup = new Startup(null);
			startup.ConfigureServices(services);
			var serviceProvider = services.BuildServiceProvider();
			startup.Configure(serviceProvider);

			var testComposite = serviceProvider.GetService<ITestComposite>();

			var testManager = new TestManager(testComposite);
			var continueTesting = false;

			do
			{
				continueTesting = testManager.RunTests();
			} while (continueTesting);
		}

		static void ConfigureServices(IServiceCollection serviceCollection)
		{
			var loggerFactory = new LoggerFactory();
			//serviceCollection.AddSingleton<I>
		}
	}

	class Tool : System.Attribute
	{
		public int Number { get; set; }
		public string Name { get; set; }

		public Tool(int number, string name)
		{
			Number = number;
			Name = name;
		}
	}

}
