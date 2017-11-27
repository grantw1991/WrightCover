using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WrightCover
{
    internal sealed class MainWindowCommand
    {
        public const int CommandId = 0x0100;
        
        public static readonly Guid CommandSet = new Guid("02e58986-644d-4604-a09c-b905eb48dd6c");
        
        private readonly Package package;
        
        private MainWindowCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.ShowToolWindow, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }
        
        public static MainWindowCommand Instance
        {
            get;
            private set;
        }
        
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }
        
        public static void Initialize(Package package)
        {
            Instance = new MainWindowCommand(package);
        }
        
        private void ShowToolWindow(object sender, EventArgs e)
        {
            ToolWindowPane window = this.package.FindToolWindow(typeof(MainWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
