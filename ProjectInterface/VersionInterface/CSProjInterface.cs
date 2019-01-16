using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace VersioningMMRB.VersionInterface
{
	public class CSProjInterface : IVersionInterface
	{
		public bool VersionLoaded { get; private set; } = false;
		private XmlDocument Document { get; set; }
		private string DocumentPath { get; set; }
		private XmlNode DocumentAssemblyVersion { get; set; }
		private XmlNode DocumentFileVersion { get; set; }
		public Version Version { get; set; }
		private string ProjectPath { get; set; }
		public async Task<bool> LoadVersionAsync(string projectPath)
		{
			ProjectPath = projectPath;
			return await Task<bool>.Run(() =>
			{
				if (!Directory.Exists(projectPath)) { return false; }
				return FindProjectFile();
			});
		}
		public async Task SaveVersionAsync()
		{
			if (!VersionLoaded) { return; }
			await Task.Run(() =>
			{
				DocumentAssemblyVersion.InnerText = Version.ToString();
				if(DocumentFileVersion!= null) 
				{
					DocumentFileVersion.InnerText = Version.ToString();
				}
				Document.Save(DocumentPath);
			});
		}
		private bool FindProjectFile()
		{
			foreach(string file in Directory.EnumerateFiles(ProjectPath, "*.csproj", SearchOption.AllDirectories))
			{
				if (ParseDocForVersion(file))
				{
					return true;
				}
			}
			return false;
		}
		private bool ParseDocForVersion(string filePath)
		{
			try
			{
				Document = new XmlDocument();
				Document.Load(filePath);
				XmlNodeList assemblyVersion = Document.GetElementsByTagName("AssemblyVersion");
				if(assemblyVersion.Count == 0) { return false; }
				DocumentAssemblyVersion = assemblyVersion[0];
				DocumentPath = filePath;
				Version = new Version(DocumentAssemblyVersion.InnerText);
				DocumentAssemblyVersion.InnerText = Version.ToString();
				VersionLoaded = true;
				XmlNodeList fileAssemblyVersions = Document.GetElementsByTagName("FileVersion");
				if (fileAssemblyVersions.Count == 0) { return true; }
				DocumentFileVersion = fileAssemblyVersions[0];
				DocumentFileVersion.InnerText = Version.ToString();
				return true;
			}
			catch { }
			return false;
		}
	}
}
