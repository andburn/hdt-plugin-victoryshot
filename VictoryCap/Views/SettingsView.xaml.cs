using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HDT.Plugins.VictoryCap.Views
{
	public partial class SettingsView : UserControl
	{
		public SettingsView()
		{
			InitializeComponent();
		}

		private void Hyperlink_Navigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(e.Uri.ToString());
		}
	}
}