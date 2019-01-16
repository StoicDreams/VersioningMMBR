
using System.Threading.Tasks;

namespace VersioningMMRB.RepoControl
{
	/// <summary>
	/// Interface for interacting with version control system.
	/// </summary>
	public interface IRepoControl
	{
		RepoStatus Status { get; }
		bool Exists(ICommandInput commands);
		Task CommitAsync(ICommandInput commands, string commitMessage);
	}
	public enum RepoStatus
	{
		NoRepoFound = 0,
		RepoFoundNoChanges = 1,
		RepoFoundWithChanges = 2,
		ProcessingCommit = 3,
		CommitSucceeded = 4,
		CommitFailed = 5
	}
}
