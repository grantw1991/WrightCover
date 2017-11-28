using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WrightCover
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(MainWindow))]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class MainWindowPackage : Package
    {
        private IVsSolution _solutionService;
        public const string PackageGuidString = "6673e1c7-cbb6-45fb-bd8c-d434f6778b20";

        public MainWindowPackage()
        {
        }

        protected override void Initialize()
        {
            _solutionService = GetService(typeof(SVsSolution)) as IVsSolution;

            var dte = GetService(typeof(DTE)) as DTE2;

            if (_solutionService != null)
            {
                Common.Instance.Package = this;
                Common.Instance.Solution = _solutionService;
                Common.Instance.Dte2Object = dte;
            }

            MainWindowCommand.Initialize(this);
            base.Initialize();
        }
    }
}
