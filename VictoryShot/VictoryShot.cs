using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HDT.Plugins.VictoryShot
{
	public class VictoryShot : IPlugin
	{
		public static List<Screenshot> Screenshots;

		public string Name => "Victory Shot (Simple)";

		public string Description => "Helps in capturing victory/defeat screen shots after a match.";

		public string ButtonText => null;

		public string Author => "andburn";

		public Version Version => new Version(2, 0);

		public MenuItem MenuItem => null;

		public VictoryShot()
		{
			Screenshots = new List<Screenshot>();
		}

		public void OnButtonPress()
		{
		}

		public void OnLoad()
		{
			GameEvents.OnGameEnd.Add(Run);
		}

		public void OnUnload()
		{
		}

		public void OnUpdate()
		{
		}

		public async static void Run()
		{
			try
			{
				// take screenshots
				await Capture(GetGameMode());
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private static string GetGameMode()
		{
			var game = Hearthstone_Deck_Tracker.API.Core.Game;
			if (game != null && game.IsRunning && game.CurrentGameStats != null)
			{
				Log.Info($"Mode {game.CurrentGameStats.GameMode}");
				return game.CurrentGameStats.GameMode.ToString().ToLowerInvariant();
			}
			Log.Info($"Mode not found");
			return string.Empty;
		}

		private static async Task Capture(string mode)
		{
			// do it for all modes don't bother checking
			try
			{
				// clear the screenshot list (possible error site)
				Screenshots.Clear();
				// no real universally correct values for this, can vary per user
				await CaptureSequence(Screenshots, 6, null, 8, 500, true);
				await SaveAll();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private static async Task CaptureSequence(List<Screenshot> list, int delaySeconds, string dir, int num, int delayBetween, bool altCapture)
		{
			Log.Info($"Capturing at {delaySeconds}s then {delayBetween}ms");

			List<Screenshot> screenshots = new List<Screenshot>();

			// initial delay, after end of game is triggered
			await Task.Delay(delaySeconds * 1000);

			// take num screenshots
			for (int i = 0; i < num; i++)
			{
				Bitmap img = await CaptureScreenShot(altCapture);
				if (img != null)
				{
					var thumb = img.ResizeImage();
					list.Add(new Screenshot(img, thumb.ToMediaImage(), i + 1));
					Log.Info($"Adding image #{i}");
				}
				await Task.Delay(delayBetween);
			}
		}

		private static async Task<Bitmap> CaptureScreenShot(bool alt)
		{
			var rect = Helper.GetHearthstoneRect(true);
			Log.Info($"Rectangle ({rect.X}, {rect.Y}, {rect.Width}, {rect.Height})");
			
			Log.Info($"Calling screen capture");
			return await ScreenCapture.CaptureHearthstoneAsync(
				new Point(0, 0), rect.Width, rect.Height, altScreenCapture: alt);
		}

		public static async Task SaveImage(Screenshot screenshot)
		{
			if (screenshot != null)
			{
				var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				var saveDir = Path.Combine(baseDir, "VictoryShot");
				if (!Directory.Exists(saveDir))
				{
					// save screenshots in the MyPictures directory, in a 'VictoryShot' sub-directory
					Log.Info($"Creating directory ({saveDir})");
					try
					{
						Directory.CreateDirectory(saveDir);
					}
					catch (Exception e)
					{
						Log.Error(e);
						return;
					}					
				}
				var filename = DateTime.Now.ToString("dd.MM.yyyy_HH.mm");
				await SaveAsPng(
					screenshot.Full, 
					Path.Combine(saveDir, $"{filename}_{screenshot.Index}.png"));
			}
			else
			{
				throw new ArgumentNullException("Screenshot was null");
			}
		}

		private static async Task SaveAsPng(Bitmap bmp, string file)
		{
			Log.Info($"Saving to '{file}'.png");
			await Task.Run(() => bmp.Save(file + ".png", ImageFormat.Png));
		}

		private static async Task SaveAll()
		{
			// now save them afterwards (mimic live plugin 'ish')
			foreach (var s in Screenshots)
			{
				if (s != null)
				{
					Log.Info($"Attempting to save screenshot #{s.Index}");
					try
					{
						await SaveImage(s);						
					}
					catch (Exception e)
					{
						Log.Error(e.Message);
					}
				}
				else
				{
					Log.Info($"Screenshot is null (idx={s.Index}, len={Screenshots?.Count})");
				}
			}

			// wait then clear screenshots
			await Task.Delay(1000);
			Screenshots.Clear();
		}
	}
}