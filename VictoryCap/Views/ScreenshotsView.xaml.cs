using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HDT.Plugins.VictoryCap.Views
{
	public partial class ScreenshotsView : UserControl
	{
		public ScreenshotsView()
		{
			InitializeComponent();
		}

		private void ViewLoaded(object sender, RoutedEventArgs e)
		{
			Width += ScreenshotList.ActualWidth;
			MinWidth += ScreenshotList.ActualWidth;
		}
	}
}