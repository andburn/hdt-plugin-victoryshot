using System.Drawing;
using System.Windows.Media.Imaging;

namespace HDT.Plugins.VictoryShot
{
	public class Screenshot
	{
		public Bitmap Full { get; private set; }
		public BitmapImage Thumbnail { get; private set; }

		private bool _isSelected;

		public bool IsSelected
		{
			get { return _isSelected; }
			set { _isSelected = value; }
		}

		private int _index;

		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

		public Screenshot(Bitmap image, BitmapImage thumb, int index)
		{
			Index = index;
			Full = image;
			Thumbnail = thumb;
			IsSelected = false;
		}
	}
}