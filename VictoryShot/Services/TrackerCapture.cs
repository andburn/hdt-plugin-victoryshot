using HDT.Plugins.VictoryShot.Models;
using HDT.Plugins.VictoryShot.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HDT.Plugins.VictoryShot.Services
{
	public class TrackerCapture : IImageCaptureService
	{
		public async Task CaptureSequence(ObservableCollection<Screenshot> list, int delaySeconds, string dir, int num, int delayBetween)
		{
			VictoryShot.Logger.Info($"Capture Screen @ {delaySeconds}s then {delayBetween}ms");

			List<Screenshot> screenshots = new List<Screenshot>();

			// initial delay, after end of game is triggered
			await Task.Delay(delaySeconds * 1000);

			// take num screenshots
			for (int i = 0; i < num; i++)
			{
				Bitmap img = await CaptureScreenShot();
				if (img != null)
				{
					var thumb = img.ResizeImage();
					screenshots.Add(new Screenshot(img, thumb.ToMediaImage(), i + 1));
					VictoryShot.Logger.Debug($"Saving image #{i}");
				}
				await Task.Delay(delayBetween);
			}

			// sort in reverse, last first
			foreach (var s in screenshots.OrderByDescending(s => s.Index))
			{
				list.Add(s);
			}
		}

		private static async Task<Bitmap> CaptureScreenShot()
		{
			return await VictoryShot.Client.GameScreenshot(true);
		}
	}
}