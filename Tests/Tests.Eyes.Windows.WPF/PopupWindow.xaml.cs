using System.Windows;

namespace Tests.Eyes.Windows.WPF
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        private PopupWindow()
        {
            InitializeComponent();
        }

        public static PopupWindow Instance => new PopupWindow();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
        }
    }
}
