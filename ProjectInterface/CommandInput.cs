using System.Diagnostics;
using System.Threading.Tasks;

namespace VersioningMMRB
{
	public class CommandInput : ICommandInput
	{
		public CommandInput(string projectPath)
		{
			Directory = projectPath;
		}
		public string Directory { get; private set; }

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
