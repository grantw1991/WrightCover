using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace WrightCover
{
    [Guid("ac0137a9-ab33-4f38-b569-e0b036c6118a")]
    public class MainWindow : ToolWindowPane
    {
        public MainWindow() : base(null)
        {
            this.Caption = "Wright Cover";
            
            this.Content = new MainWindowControl();
        }
    }
}
