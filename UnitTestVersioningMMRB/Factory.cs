using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestVersioningMMRB
{
	public static class Factory
	{
		public const string SandboxFolder = @"D:\UnitTestSandbox\";
		public static Dictionary<string, TestApp> TestProjects { get; } = new Dictionary<string, TestApp>()
		{
			{"FrameworkWebsiteDemo", new TestApp("FrameworkWebsite", "FrameworkWebsiteDemo") }
			, {"ConsoleAppDemo", new TestApp("CoreConsoleApp", "ConsoleAppDemo") }
			, {"CoreLibraryDemo", new TestApp("CoreLibrary", "CoreLibraryDemo") }
			, {"CoreUnitTest", new TestApp("CoreUnitTest", "CoreUnitTest") }
			, {"VSIXProjectDemo", new TestApp("CoreVSIX", "VSIXProjectDemo") }
			, {"CoreWebsiteDemo", new TestApp("CoreWebsite", "CoreWebsiteDemo") }
			, {"StandardLibraryDemo", new TestApp("StandardLibrary", "StandardLibraryDemo") }
		};
		public static Dictionary<string, TestRepo> TestMercurial { get; } = new Dictionary<string, TestRepo>()
		{
			{"MercurialRepo", new TestRepo("MercurialRepo", "Test File.txt") }
		};
		public static Dictionary<string, TestRepo> TestGit { get; } = new Dictionary<string, TestRepo>()
		{
			{"GitRepo", new TestRepo("GitRepo", "Test File.txt") }
		};
	}
	public struct TestRepo
	{
		public string RepoPath { get => $@"{Factory.SandboxFolder}{FolderName}"; }
		public string FolderName { get; }
		public string FileName { get; }
		public string TestFilePath { get => $@"{RepoPath}\{FileName}"; }
		public TestRepo(string folderName, string testFile)
		{
			FolderName = folderName;
			FileName = testFile;
		}
	}
	public struct TestApp
	{
		public string ProjectPath { get => $@"{Factory.SandboxFolder}{FolderName}\{ProjectName}"; }
		public string FolderName { get; }
		public string ProjectName { get; }
		public TestApp(string folderName, string projectName)
		{
			FolderName = folderName;
			ProjectName = projectName;
		}
	}
}
