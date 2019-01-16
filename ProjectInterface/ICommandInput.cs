using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersioningMMRB
{
	public interface ICommandInput
	{
		string Directory { get; }
		Task<string> RunProcessAsync(string executable, string arguments);
	}
}
