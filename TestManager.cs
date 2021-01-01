using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Sandbox1
{
	class TestManager
	{
		private readonly ITestComposite _testComposite;

		public TestManager(ITestComposite testComposite)
		{
			_testComposite = testComposite;
		}

		public ILogger Logger { get; set; }
		public bool RunTests()
		{
			var loggerFactory = new LoggerFactory();
			Logger = loggerFactory.CreateLogger("Sandbox");  // use injection later for this.

			Console.WriteLine("Action List:");

			//typeof(TestTool).GetMethods().Where(m => m.CustomAttributes.Where(a => a.AttributeType == typeof(Tool)).Count() > 0)

			var methods = typeof(TestComposite).GetMethods()
				.Where(m => m.CustomAttributes
					.Where(a => a.AttributeType == typeof(Tool))
				.Count() > 0);

			Console.WriteLine("0: exit");

			foreach (var method in methods)
			{
				var methodAtt = method.GetCustomAttributes(typeof(Tool), true).FirstOrDefault() as Tool;

				Console.WriteLine($"{methodAtt.Number}: {methodAtt.Name}");
			}

			Console.WriteLine("select the number of the action you want to take");
			Console.WriteLine(); // whitespace

			var choice = Console.ReadLine();

			if (choice.ToLower() == "0")
				return false;
			else
			{
				// run test

				foreach (var method in methods)
				{
					var methodAtt = method.GetCustomAttributes(typeof(Tool), true).FirstOrDefault() as Tool;

					try
					{
						if (choice == methodAtt.Number.ToString())
							method.Invoke(_testComposite, null);
					}
					catch (Exception ex)
					{
						Console.WriteLine("--Error--");
						Console.WriteLine(ex.Message);
						Console.WriteLine(ex.StackTrace);
					}
				}

				Console.WriteLine(); // whitespace

				return true;
			}
		}
	}

}
