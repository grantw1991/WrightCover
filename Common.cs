using System.Collections.ObjectModel;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace WrightCover
{
    public class Common
    {
        private static Common _instance;

        private Common() { }

        public static Common Instance => _instance ?? (_instance = new Common());
        public IVsSolution Solution { get; set; }
        public DTE2 Dte2Object { get; set; }
        public MainWindowPackage Package { get; set; }
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();
        public uint SolutionCookie { get; set; }
    }
}
