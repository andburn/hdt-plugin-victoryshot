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

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scv = sender as ScrollViewer;
			if (scv != null)
			{
				scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
				e.Handled = true;
			}
		}
	}
}