﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HDT.Plugins.VictoryCap.Models;
using HDT.Plugins.VictoryCap.Utilities;

namespace HDT.Plugins.VictoryCap.Services
{
	public class TrackerCapture : IImageCaptureService
	{
		public async Task<ObservableCollection<Screenshot>> CaptureSequence(int delaySeconds, string dir, int num, int delayBetween)
		{
			VictoryCap.Logger.Info($"Capture Screen @ {delaySeconds}s then {delayBetween}ms");

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
					VictoryCap.Logger.Debug($"Saving image #{i}");
				}
				await Task.Delay(delayBetween);
			}

			// sort in reverse, last first
			return new ObservableCollection<Screenshot>(screenshots.OrderByDescending(s => s.Index));
		}

		private static async Task<Bitmap> CaptureScreenShot()
		{
			return await VictoryCap.Client.GameScreenshot(true);
		}

		public async Task SaveImage(Screenshot screenshot)
		{
			if (screenshot != null)
			{
				var dir = VictoryCap.Settings.Get("ScreenShot", "OutputDir").Value;
				if (!Directory.Exists(dir))
				{
					VictoryCap.Logger.Info($"Output dir does not exist ({dir}), using desktop");
					dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				}
				var filename = DateTime.Now.ToString("dd.MM.yyyy_HH.mm");

				var gameInfo = VictoryCap.Client.CurrentGameInfo();
				if (gameInfo.Length == 4)
				{
					// save with game details
					var pattern = VictoryCap.Settings.Get("ScreenShot", "FileNamePattern").Value;					
					NamingPattern np = null;
					if (!NamingPattern.TryParse(pattern, out np))
						VictoryCap.Logger.Info("Invalid file name pattern, using default");
					// TODO a cleaner way here
					filename = np.Apply(gameInfo[0], gameInfo[1], gameInfo[2], gameInfo[3]);
				}				
				var fn = Path.Combine(dir, filename + ".png");
				await SaveAsPng(screenshot.Full, fn);
			}
			else
			{
				throw new ArgumentNullException("Screenshot was null");
			}
		}

		private static async Task SaveAsPng(Bitmap bmp, string file)
		{
			VictoryCap.Logger.Info($"Saving screenshot to '{file}'");
			await Task.Run(() => bmp.Save(file + ".png", ImageFormat.Png));
		}
	}
}