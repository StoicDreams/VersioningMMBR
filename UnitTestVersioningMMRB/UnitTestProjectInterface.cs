using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VersioningMMRB.VersionInterface;

namespace UnitTestVersioningMMRB
{
	[TestClass]
	public class UnitTestProjectInterface
	{
		[TestMethod]
		public async Task TestGetProjectInterfaceAsync()
		{
			foreach(TestApp app in Factory.TestProjects.Values)
			{
				await TestInterface(app.ProjectPath);
			}
		}
		[TestMethod]
		public async Task TestIncrementAndSave()
		{
			foreach (TestApp app in Factory.TestProjects.Values)
			{
				IVersionInterface project = await TestInterface(app.ProjectPath);
				string oldVersion = project.Version.ToString();
				project.Version.IncrementBuild();
				await project.SaveVersionAsync();
				project = await TestInterface(app.ProjectPath);
				Assert.AreNotEqual(oldVersion, project.Version.ToString());
			}
		}
		private async Task<IVersionInterface> TestInterface(string projectPath)
		{
			IVersionInterface projectInterface = await ProjectInterface.GetProjectInterfaceAsync(projectPath);
			Assert.AreNotEqual(null, projectInterface);
			return projectInterface;
		}
	}
}
