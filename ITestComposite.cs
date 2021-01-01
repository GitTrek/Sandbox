using System.Threading.Tasks;

namespace Sandbox1
{
	interface ITestComposite
	{
		Task TestCreateWorkItem();
		void TestIDictionaryTryGet();
		void TestListCompare();
		void TestLogging();
		void TestNullCast();
		void TestNullCoalescing();
		void TestPatchDocToString();
		Task TestReadWorkItem();
		void TestSnowCaseGet();
		void TestStringTrim();
		void TestVstsWorkItemAPI();
	}
}