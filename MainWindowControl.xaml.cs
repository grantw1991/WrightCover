using System.Windows.Controls;

namespace WrightCover
{
    public partial class MainWindowControl : UserControl
    {
        public MainWindowControl()
        {
            DataContext = new MainViewModel();
            
            InitializeComponent();
        }
    }
}