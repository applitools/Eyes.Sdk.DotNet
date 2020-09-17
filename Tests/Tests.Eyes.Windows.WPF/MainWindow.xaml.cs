using System;
using System.Collections.Generic;
using System.Windows;

namespace Tests.Eyes.Windows.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<string> args = new List<string>(Environment.GetCommandLineArgs());

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopupWindow.Instance.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            if (args.Contains("--show-popup"))
            {
                PopupWindow.Instance.ShowDialog();
            }
        }
    }
}
