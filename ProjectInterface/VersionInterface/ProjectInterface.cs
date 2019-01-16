using System.Threading.Tasks;

namespace VersioningMMRB.VersionInterface
{
	public static class ProjectInterface
	{
		public static async Task<IVersionInterface> GetProjectInterfaceAsync(string projectPath)
		{
			IVersionInterface project = new ManifestInterface();
			if (await project.LoadVersionAsync(projectPath))
			{
				return project;
			}
			project = new CSProjInterface();
			if(await project.LoadVersionAsync(projectPath))
			{
				return project;
			}
			project = new CSAssemblyInterface();
			if (await project.LoadVersionAsync(projectPath))
			{
				return project;
			}
			return null;
		}
	}
}
