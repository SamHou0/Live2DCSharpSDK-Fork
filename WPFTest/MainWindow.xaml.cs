using Live2DCSharpSDK.WPF;
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
        Live2DWPFModel l2dwpf;
        public MainWindow()
        {
            InitializeComponent();
            l2dwpf = new Live2DWPFModel(@"F:\Downloads\七彩虹\七彩虹.moc3");
            BorderOpenTK.Child = l2dwpf.GLControl;
            l2dwpf.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            workloop();
        }
        private void workloop()
        {
            var ind = l2dwpf.StartMotion(@"F:\Downloads\七彩虹\走路1.motion3.json", onFinishedMotionHandler: (x, y) =>
              {
                  ticklab.Content = DateTime.Now.ToString() + " workloop";
                  workloop();
              });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            idelloop();
        }
        void idelloop()
        {
            l2dwpf.StartMotion(@"F:\Downloads\七彩虹\idle 复制_COPY.motion3.json", onFinishedMotionHandler: (x, y) =>
            {
                ticklab.Content = DateTime.Now.ToString() + " idelloop";
                idelloop();
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            l2dwpf.LModel.LoadBreath();
        }
    }
}