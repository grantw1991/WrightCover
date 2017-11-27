using System;
using System.ComponentModel.Design;
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
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            if (!(ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)) return;
            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(ShowToolWindow, menuCommandId);
            commandService.AddCommand(menuItem);
        }
        
        public static MainWindowCommand Instance
        {
            get;
            private set;
        }
        
        private IServiceProvider ServiceProvider => package;

        public static void Initialize(Package package)
        {
            Instance = new MainWindowCommand(package);
        }
        
        private void ShowToolWindow(object sender, EventArgs e)
        {
            var window = package.FindToolWindow(typeof(MainWindow), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
