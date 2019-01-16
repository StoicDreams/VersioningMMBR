using System.IO;
using System.Threading.Tasks;

namespace VersioningMMRB.VersionInterface
{
	public class CSAssemblyInterface : IVersionInterface
	{
		public bool VersionLoaded { get; private set; } = false;
		private string CSFileContent { get; set; }
		private string CSFilePath { get; set; }
		public Version Version { get; private set; }
		private string ProjectPath { get; set; }
		public async Task<bool> LoadVersionAsync(string projectPath)
		{
			ProjectPath = projectPath;
			return await Task<bool>.Run(() => {
				if (!Directory.Exists(projectPath)) { return false; }
				return FindAssemblyFile();
			});
		}
		public async Task SaveVersionAsync()
		{
			if (!VersionLoaded) { return; }
			await Task.Run(() =>
			{
				CSFileContent = CSFileContent.Remove(indexAssemblyVersion, indexAssemblyVersionEnd - indexAssemblyVersion);
				CSFileContent = CSFileContent.Insert(indexAssemblyVersion, $"{startOfAssemblyVersion}{Version.ToString()}");
				indexFileVersion = CSFileContent.IndexOf(startOfAssemblyFileVersion);
				if (indexFileVersion != -1)
				{
					indexFileVersionEnd = CSFileContent.IndexOf('"', indexFileVersion + startOfAssemblyFileVersion.Length);
					CSFileContent = CSFileContent.Remove(indexFileVersion, indexFileVersionEnd - indexFileVersion);
					CSFileContent = CSFileContent.Insert(indexFileVersion, $"{startOfAssemblyFileVersion}{Version.ToString()}");
				}
				File.WriteAllText(CSFilePath, CSFileContent);
			});
		}
		private bool FindAssemblyFile()
		{
			foreach (string file in Directory.EnumerateFiles(ProjectPath, "*.cs", SearchOption.AllDirectories))
			{
				if (ParseCSForVersion(file))
				{
					return true;
				}
			}
			return false;
		}
		private const string startOfAssemblyVersion = "[assembly: AssemblyVersion(\"";
		private const string startOfAssemblyFileVersion = "[assembly: AssemblyFileVersion(\"";
		private int indexAssemblyVersion = -1;
		private int indexFileVersion = -1;
		private int indexAssemblyVersionEnd = -1;
		private int indexFileVersionEnd = -1;
		private bool ParseCSForVersion(string filePath)
		{
			try
			{
				CSFileContent = File.ReadAllText(filePath);
				indexAssemblyVersion = CSFileContent.IndexOf(startOfAssemblyVersion);
				if(indexAssemblyVersion == -1) { return false; }
				indexAssemblyVersionEnd = CSFileContent.IndexOf('"', indexAssemblyVersion + startOfAssemblyVersion.Length);
				Version = new Version(CSFileContent.Substring(indexAssemblyVersion, indexAssemblyVersionEnd - indexAssemblyVersion));
				VersionLoaded = true;
				CSFilePath = filePath;
				return true;
			}
			catch { }
			return false;
		}
	}
}
