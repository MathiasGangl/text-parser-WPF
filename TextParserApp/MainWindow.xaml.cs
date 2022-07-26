using System.Windows;
using TextParserApp.ViewModel;

namespace TextParserApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Constructor for MainWindow. Sets the DataContext to the MainViewModel.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}

