using System;
using System.Threading.Tasks;

namespace VersioningMMRB.RepoControl
{
	public class Git : IRepoControl
	{
		public RepoStatus Status { get; private set; } = RepoStatus.NoRepoFound;
		public async Task CommitAsync(ICommandInput commands, string commitMessage)
		{
			await Task.Run(async () => {
				if (Status != RepoStatus.RepoFoundWithChanges) { return; }
				Status = RepoStatus.ProcessingCommit;
				string toRun = $"commit -a -m \"{commitMessage}\"";
				string result = await commands.RunProcessAsync("git", toRun);
				if (result.Contains(commitMessage))
				{
					Status = RepoStatus.CommitSucceeded;
				}
				else
				{
					Status = RepoStatus.CommitFailed;
				}
			});
		}

		private const string GitMessageNoRepo = "Not a git repository";
		public bool Exists(ICommandInput commands)
		{
			Task<string> task = commands.RunProcessAsync("git", "status");
			task.Wait();
			string result = task.Result;
			task.Dispose();
			if (result.Contains(GitMessageNoRepo))
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
