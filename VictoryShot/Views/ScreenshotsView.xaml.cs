using System.Windows.Controls;
using System.Windows.Input;

namespace HDT.Plugins.VictoryShot.Views
{
	public partial class ScreenshotsView : UserControl
	{
		public ScreenshotsView()
		{
			InitializeComponent();
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