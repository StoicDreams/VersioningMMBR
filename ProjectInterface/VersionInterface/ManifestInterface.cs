using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace VersioningMMRB.VersionInterface
{
	public class ManifestInterface : IVersionInterface
	{
		public bool VersionLoaded { get; private set; } = false;
		private XmlDocument Document { get; set; }
		private string DocumentPath { get; set; }
		private XmlAttribute VersionAttribute { get; set; }
		public Version Version { get; private set; }
		private string ProjectPath { get; set; }
		public async Task<bool> LoadVersionAsync(string projectPath)
		{
			ProjectPath = projectPath;
			return await Task<bool>.Run(() => {
				if (!Directory.Exists(projectPath)) { return false; }
				return FindManifestFile();
			});
		}
		public async Task SaveVersionAsync()
		{
			if (!VersionLoaded) { return; }
			await Task.Run(() =>
			{
				VersionAttribute.Value = Version.ToString();
				Document.Save(DocumentPath);
			});
		}
		private bool FindManifestFile()
		{
			foreach(string file in Directory.EnumerateFiles(ProjectPath, "*manifest", SearchOption.AllDirectories))
			{
				if (ParseXMLForVersion(file))
				{
					return true;
				}
			}
			return false;
		}
		private bool ParseXMLForVersion(string fileName)
		{
			try
			{
				Document = new XmlDocument();
				Document.Load(fileName);
				XmlNodeList metaData = Document.GetElementsByTagName("Metadata");
				for(int index = 0;index < metaData.Count; ++index)
				{
					for(int child=0;child < metaData[index].ChildNodes.Count;++child)
					{
						if(metaData[index].ChildNodes[child].Name != "Identity") { continue; }
						VersionAttribute = metaData[index].ChildNodes[child].Attributes["Version"];
						if (VersionAttribute == null) { continue; }
						Version = new Version(VersionAttribute.Value);
						VersionLoaded = true;
						VersionAttribute.Value = Version.ToString();
						DocumentPath = fileName;
						Document.Save(fileName);
						return true;
					}
				}
			}
			catch { }
			return false;
		}
	}
}
