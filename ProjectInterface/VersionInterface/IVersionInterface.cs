using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersioningMMRB.VersionInterface
{
	/// <summary>
	/// Interface for interacting with project versioning system.
	/// </summary>
	public interface IVersionInterface
	{
		Version Version { get; }
		bool VersionLoaded { get; }
		Task<bool> LoadVersionAsync(string projectPath);
		Task SaveVersionAsync();
	}
}
