using System.Threading.Tasks;

namespace VersioningMMRB.RepoControl
{
	public static class Controllers
	{
		public async static Task<IRepoControl> GetRepoControllerAsync(ICommandInput commandInput)
		{
			IRepoControl controller = null;
			await Task.Run(() =>
			{
				controller = new Mercurial();
				if (controller.Exists(commandInput)) { return; }
				controller = new Git();
				if (controller.Exists(commandInput)) { return; }
				controller = null;
			});
			return controller;
		}
	}
}
