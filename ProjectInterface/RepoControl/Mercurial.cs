using System;
using System.Threading.Tasks;

namespace VersioningMMRB.RepoControl
{
	public class Mercurial : IRepoControl
	{
		public RepoStatus Status { get; private set; } = RepoStatus.NoRepoFound;
		public async Task CommitAsync(ICommandInput commands, string commitMessage)
		{
			await Task.Run(async () => {
				if(Status != RepoStatus.RepoFoundWithChanges) { return; }
				Status = RepoStatus.ProcessingCommit;
				string toRun = $"commit -m \"{commitMessage}\"";
				//Add new files to repo and remove deleted files from repo.
				await commands.RunProcessAsync("hg", "addremove");
				string result = await commands.RunProcessAsync("hg", toRun);
				if (String.IsNullOrWhiteSpace(result))
				{
					Status = RepoStatus.CommitSucceeded;
				}
				else
				{
					Status = RepoStatus.CommitFailed;
				}
			});
		}

		private const string MercurialMessageNoRepo = "no repository found";
		public bool Exists(ICommandInput commands)
		{
			Task<string> task = commands.RunProcessAsync("hg", "status");
			task.Wait();
			string result = task.Result;
			task.Dispose();
			if (result.Contains(MercurialMessageNoRepo))
			{
				Status = RepoStatus.NoRepoFound;
				return false;
			}
			else if (String.IsNullOrWhiteSpace(result))
			{
				Status = RepoStatus.RepoFoundNoChanges;
			}
			else
			{
				Status = RepoStatus.RepoFoundWithChanges;
			}
			return true;
		}
	}
}
