using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using HDT.Plugins.VictoryCap.Models;

namespace HDT.Plugins.VictoryCap.Utilities
{
	public class DesignerData
	{
		public static ObservableCollection<Screenshot> GenerateScreenshots()
		{
			StreamResourceInfo sri = Application.GetResourceStream(
					new Uri("pack://application:,,,/VictoryCap;component/Resources/thumb_sample.bmp"));

			BitmapImage bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.StreamSource = sri.Stream;
			bmp.EndInit();

			return new ObservableCollection<Screenshot>() {
					new Screenshot(null, bmp, 1),
					new Screenshot(null, bmp, 2),
					new Screenshot(null, bmp, 3),
					new Screenshot(null, bmp, 4),
					new Screenshot(null, bmp, 5),
					new Screenshot(null, bmp, 6),
					new Screenshot(null, bmp, 7),
					new Screenshot(null, bmp, 8),
					new Screenshot(null, bmp, 9),
					new Screenshot(null, bmp, 10),
					new Screenshot(null, bmp, 11),
					new Screenshot(null, bmp, 12)
				};
		}
	}
}