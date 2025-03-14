using Live2DCSharpSDK.WPF;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Live2DWPFModel live2dModel;
        string path = "";
        public MainWindow()
        {
            InitializeComponent();
            OpenFileDialog dialog = new();
            dialog.Filter = "moc3|*.moc3";
            dialog.ShowDialog();
            path = dialog.FileName;
            if (path == "")
            {
                Close();
                return;
            }
            live2dModel = new Live2DWPFModel(path);
            BorderOpenTK.Child = live2dModel.GLControl;
            live2dModel.Start();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var mp = e.GetPosition(BorderOpenTK);
            live2dModel.LModel.SetDragging((float)(mp.X / BorderOpenTK.ActualWidth * 2 - 1),
                -(float)(mp.Y / BorderOpenTK.ActualHeight * 2 - 0.5));
        }

        private void SX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //l2dwpf.LModel.SetDragging((float)(SX.Value), (float)(SY.Value));
        }
    }
}