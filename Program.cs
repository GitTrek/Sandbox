using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox1
{
	class Program
	{
		static void Main(string[] args)
		{
			var testManager = new TestManager();
			var continueTesting = false;

			do
			{
				continueTesting = testManager.RunTests();
			} while (continueTesting);
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

	class TestManager
	{
		public bool RunTests()
		{
			Console.WriteLine("Action List:");

			//typeof(TestTool).GetMethods().Where(m => m.CustomAttributes.Where(a => a.AttributeType == typeof(Tool)).Count() > 0)

			var methods = typeof(TestComposite).GetMethods()
				.Where(m => m.CustomAttributes
				.Where(a => a.AttributeType == typeof(Tool))
				.Count() > 0);

			var testComposite = new TestComposite();

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

					if (choice == methodAtt.Number.ToString())
						method.Invoke(testComposite, null);
				}

				Console.WriteLine(); // whitespace

				return true;
			}
		}
	}

	class TestComposite
	{
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
	}
}
