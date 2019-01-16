using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VersioningMMRB;

namespace UnitTestVersioningMMRB
{
	[TestClass]
	public class UnitTestProcessor
	{
		[TestMethod]
		public async Task TestSandboxApps()
		{
			foreach(TestApp app in Factory.TestProjects.Values)
			{
				await TestProcessor(app.ProjectPath);
			}
		}
		public async Task TestProcessor(string path)
		{
			Processor processor = new Processor(path);
			await processor.UpdateVersionIfFilesChangedAsync();
			Assert.AreEqual(false, processor.IsRelease);
			await processor.IncrementVersionAndCommitChangesAsync(Processor.VersionScope.Release);
			Assert.AreEqual(true, processor.IsRelease);
		}
	}
}
