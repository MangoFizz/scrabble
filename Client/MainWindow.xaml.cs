using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client {
    public partial class MainWindow : Window {
        private const double BaseWindowWidth = 1280.0f;
        private const double BaseWindowHeight = 720.0f;

        public MainWindow() {
            InitializeComponent();
            MainFrame.Content = new LoginPage();
        }

        public void UpdateScale() {
            if(Content is Visual visual) {
                double frameWidthScale = (ActualWidth - SystemParameters.FixedFrameVerticalBorderWidth) / BaseWindowWidth;
                var scaledHeight = frameWidthScale * 720;
                frameWidthScale = ((scaledHeight - SystemParameters.FixedFrameVerticalBorderWidth * 2) * frameWidthScale) / scaledHeight;

                double frameHeightScale = (ActualHeight - SystemParameters.WindowCaptionHeight - SystemParameters.FixedFrameHorizontalBorderHeight) / BaseWindowHeight;
                var scaledWidth = frameHeightScale * 1280;
                frameHeightScale = ((scaledWidth - SystemParameters.WindowCaptionHeight) * frameHeightScale) / scaledWidth;

                double value = Math.Min(frameWidthScale, frameHeightScale);
                double scaleValue = double.IsNaN(value) ? 1d : Math.Max(0.25, value);

                visual.SetValue(Frame.LayoutTransformProperty, new ScaleTransform(scaleValue, scaleValue, 0, 0));
            }
        }

        public void MainFrameSizeChanged(object sender, EventArgs e) {
            UpdateScale();
        }
    }
}
