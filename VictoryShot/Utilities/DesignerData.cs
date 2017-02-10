using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using HDT.Plugins.VictoryShot.Models;

namespace HDT.Plugins.VictoryShot.Utilities
{
	public class DesignerData
	{
		private static int count = 0;
		private static BitmapImage bmp;

		static DesignerData() {
			StreamResourceInfo sri = Application.GetResourceStream(
					new Uri("pack://application:,,,/VictoryShot;component/Resources/thumb_sample.bmp"));
			bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.StreamSource = sri.Stream;
			bmp.EndInit();
		}


		public static ObservableCollection<Screenshot> GenerateScreenshots()
		{
			return new ObservableCollection<Screenshot>() {
					CreateScreenshot(),
					CreateScreenshot(),
					CreateScreenshot(),
					CreateScreenshot(),
					CreateScreenshot()
				};
		}

		public static Screenshot CreateScreenshot()
		{
			return new Screenshot(null, bmp, count++);
		}
	}
}