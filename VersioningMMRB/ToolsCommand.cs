using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace VersioningMMRB
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class ToolsCommand
	{
		private const int CommandIncrementRelease = 0x0100;
		private const int CommandIncrementMinor = 0x0101;
		private const int CommandIncrementMajor = 0x0102;
		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("82c538cc-32cc-41ad-a8ad-aa93a908f1ad");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;
		private readonly OleMenuCommandService commandService;
		/// <summary>
		/// Initializes a new instance of the <see cref="IncrementRelease"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private ToolsCommand(AsyncPackage package, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			this.commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
			BuildCommand(CommandIncrementMajor, this.IncrementMajor);
			BuildCommand(CommandIncrementMinor, this.IncrementMinor);
			BuildCommand(CommandIncrementRelease, this.IncrementRelease);
		}
		private void BuildCommand(int commandID, EventHandler handler)
		{
			var menuCommandID = new CommandID(CommandSet, commandID);
			var menuItem = new MenuCommand(handler, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static ToolsCommand Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
		{
			get
			{
				return this.package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in ToolsCommand's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
			OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
			Instance = new ToolsCommand(package, commandService);
		}
		
		public void InitiateProcessorAndIncrementVersionAndCommit(Processor.VersionScope scope)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Project project = GetProject();
			Processor processor = new Processor(System.IO.Path.GetDirectoryName(project.FullName));
			Task task = Task.Run(async () =>
			{
				await processor.IncrementVersionAndCommitChangesAsync(scope);
			});
			Task.WaitAll(task);
			DisplayMessage(processor.Result);
		}

		private Project GetProject()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Object selectedObject = null;
			IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
			monitorSelection.GetCurrentSelection(out IntPtr hierarchyPointer, out uint projectItemId, out IVsMultiItemSelect multiItemSelect, out IntPtr selectionContainerPointer);
			if (Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) is IVsHierarchy selectedHierarchy)
			{
				ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject));
			}
			return selectedObject as Project;
		}

		private void IncrementMajor(object sender, EventArgs e)
		{
			InitiateProcessorAndIncrementVersionAndCommit(Processor.VersionScope.Major);
		}

		private void IncrementMinor(object sender, EventArgs e)
		{
			InitiateProcessorAndIncrementVersionAndCommit(Processor.VersionScope.Minor);
		}

		private void IncrementRelease(object sender, EventArgs e)
		{
			InitiateProcessorAndIncrementVersionAndCommit(Processor.VersionScope.Release);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		private void DisplayMessage(string message, string title = "Versioning MMRB Result")
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Debug.WriteLine($"{title}:{message}");
		}
	}
}
