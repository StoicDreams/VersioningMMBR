using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VersioningMMRB;
using VersioningMMRB.RepoControl;
using VersioningMMRB.VersionInterface;

namespace UnitTestVersioningMMRB
{
	[TestClass]
	public class UnitTestRepoControl
	{
		[TestMethod]
		public async Task TestMercurial()
		{
			foreach(TestRepo repo in Factory.TestMercurial.Values)
			{
				ICommandInput commandInput = new TestCommands(repo.RepoPath);
				IRepoControl repoControl = new Mercurial();
				Assert.AreEqual(true, repoControl.Exists(commandInput));
				File.WriteAllText(repo.TestFilePath, $"Last Tested {DateTime.UtcNow.ToString()}\n{DateTime.UtcNow.Ticks}");
				await repoControl.CommitAsync(commandInput, "Unit Test");
				Assert.AreEqual(RepoStatus.CommitSucceeded, repoControl.Status);
			}
		}
		[TestMethod]
		public async Task TestGit()
		{
			foreach (TestRepo repo in Factory.TestGit.Values)
			{
				ICommandInput commandInput = new TestCommands(repo.RepoPath);
				IRepoControl repoControl = new Git();
				Assert.AreEqual(true, repoControl.Exists(commandInput));
				File.WriteAllText(repo.TestFilePath, $"Last Tested {DateTime.UtcNow.ToString()}\n{DateTime.UtcNow.Ticks}");
				await repoControl.CommitAsync(commandInput, "Unit Test");
				Assert.AreEqual(RepoStatus.CommitSucceeded, repoControl.Status);
			}
		}
		private class TestCommands : ICommandInput
		{
			public TestCommands(string directory)
			{
				Directory = directory;
			}

			public string Directory { get; }

			public async Task<string> RunProcessAsync(string executable, string arguments)
			{
				Process process = new Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.WorkingDirectory = Directory;
				process.StartInfo.FileName = executable;
				process.StartInfo.Arguments = arguments;
				process.Start();
				string result = await process.StandardOutput.ReadToEndAsync();
				string error = await process.StandardError.ReadToEndAsync();
				process.WaitForExit();
				return $"{result} {error}".Trim();
			}
		}
	}
}
