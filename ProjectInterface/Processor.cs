using System.Threading;
using System.Threading.Tasks;
using VersioningMMRB.RepoControl;
using VersioningMMRB.VersionInterface;

namespace VersioningMMRB
{
	public class Processor
	{
		public string Result { get; private set; } = "Initialized";
		public bool CommitOnBuildUpdates { get; set; } = false;
		public bool CommitOnReleaseUpdates { get; set; } = true;
		private string ProjectPath { get; set; }
		public Processor(string projectPath)
		{
			ProjectPath = projectPath.Replace(@"\\", @"\");
			CommandInput = new CommandInput(projectPath);
			Task.Run(SetupControllerAsync);
			Task.Run(SetupInterfaceAsync);
		}
		private async Task SetupControllerAsync()
		{
			Controller = await Controllers.GetRepoControllerAsync(CommandInput);
			if(Controller == null)
			{
				IsMissingController = true;
			}
			else
			{
				IsMissingController = false;
			}
		}
		private async Task SetupInterfaceAsync()
		{
			Interface = await ProjectInterface.GetProjectInterfaceAsync(ProjectPath);
			if(Interface == null)
			{
				IsMissingInterface = true;
			}
			else
			{
				await Interface.LoadVersionAsync(ProjectPath);
				IsMissingInterface = false;
			}
		}
		private ICommandInput CommandInput { get; set; }
		private IRepoControl Controller { get; set; }
		private IVersionInterface Interface { get; set; }
		private bool IsMissingController = false;
		private bool IsMissingInterface = false;
		private const int WaitIntervalInMilliseconds = 3;
		private async Task<bool> WaitForRepoControllerAsync()
		{
			await Task.Run(() => {
				while (Controller == null && !IsMissingController)
				{
					Thread.Sleep(WaitIntervalInMilliseconds);
				}
			});
			return Controller != null && !IsMissingController;
		}
		private async Task<bool> WaitForInterfaceAsync()
		{
			await Task.Run(() => {
				while(Interface == null && !IsMissingInterface)
				{
					Thread.Sleep(WaitIntervalInMilliseconds);
				}
			});
			return Interface != null && !IsMissingInterface;
		}
		public enum VersionScope
		{
			Major = 1,
			Minor = 2,
			Release = 3,
			Build = 4
		}

		/// <summary>
		/// Run after successful, user triggered, build events to update Build number.
		/// </summary>
		/// <returns></returns>
		public async Task UpdateVersionIfFilesChangedAsync()
		{
			if (!await CurrentBuildHasChangesAsync()) { return; }
			await IncrementVersionAndCommitChangesAsync(VersionScope.Build);
		}
		private async Task<bool> CurrentBuildHasChangesAsync()
		{
			if(!await WaitForRepoControllerAsync()) { return false; }
			bool buildHasChanges = false;
			await Task.Run(() =>
			{

			});
			return buildHasChanges;
		}
		/// <summary>
		/// Release Versions (Major, Minor, Release) should be triggered after manual input from user.
		/// Build version should be triggered from Build events
		/// </summary>
		/// <param name="scope"></param>
		public async Task IncrementVersionAndCommitChangesAsync(VersionScope scope)
		{
			if(!await WaitForInterfaceAsync())
			{
				Result = "Failed to update version.";
				return;
			}
			//Update Project Version
			await IncrementVersionAsync(scope);
			bool isRelease = false;
			if (isRelease)
			{
				//Switch project to RELEASE
			}
			//Build project with updated version
			//Return if not committing changes
			switch (scope)
			{
				case VersionScope.Build when !CommitOnBuildUpdates:
					return;
				case VersionScope.Release when !CommitOnReleaseUpdates:
					return;
			}
			await CommitChangesAsync($"{scope.ToString()} Release");
		}
		public bool IsRelease { get; private set; } = false;
		private async Task IncrementVersionAsync(VersionScope scope)
		{
			if(!await WaitForInterfaceAsync()) { return; }
			switch (scope)
			{
				case VersionScope.Major:
					Interface.Version.IncrementMajor();
					break;
				case VersionScope.Minor:
					Interface.Version.IncrementMinor();
					break;
				case VersionScope.Release:
					Interface.Version.IncrementRelease();
					break;
				case VersionScope.Build:
					Interface.Version.IncrementBuild();
					break;
			}
			if (scope != VersionScope.Build) { IsRelease = true; }
			await Interface.SaveVersionAsync();
			Result = $"Incremented {scope.ToString()}: {Interface.Version.ToString()}";
		}
		private async Task CommitChangesAsync(string commitMessage)
		{
			if (!await WaitForRepoControllerAsync())
			{
				Result += "\nRepo not found";
				return;
			}
			await Controller.CommitAsync(CommandInput, $"{Interface.Version.ToString()}\n{commitMessage}");
			Result = $"Committed: {Interface.Version.ToString()}\n{commitMessage}";
		}
	}
}
