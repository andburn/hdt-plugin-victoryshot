using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Services;
using HDT.Plugins.Common.Settings;
using HDT.Plugins.VictoryShot.Models;
using HDT.Plugins.VictoryShot.Services;
using HDT.Plugins.VictoryShot.ViewModels;
using HDT.Plugins.VictoryShot.Views;
using HDT.Plugins.VictoryShot.Utilities;
using HDT.Plugins.Common.Controls;
using Ninject;
using HDT.Plugins.Common.Utils;

namespace HDT.Plugins.VictoryShot
{
	public class VictoryShot : IPluggable
	{
		public static IUpdateService Updater;
		public static ILoggingService Logger;
		public static IDataRepository Data;
		public static IEventsService Events;
		public static IGameClientService Client;
		public static IConfigurationRepository Config;
		public static Settings Settings;

		private static IImageCaptureService _capture;

		public static ObservableCollection<Screenshot> Screenshots;
		public static MainViewModel MainViewModel;

		private static IKernel _kernel;
		private static Version _version;

		public static readonly string Name = "Victory Shot";

		public VictoryShot(IKernel kernel, Version version)
		{
			_kernel = kernel;
			_version = version;

			// initialize services
			Updater = _kernel.Get<IUpdateService>();
			Logger = _kernel.Get<ILoggingService>();
			Data = _kernel.Get<IDataRepository>();
			Events = _kernel.Get<IEventsService>();
			Client = _kernel.Get<IGameClientService>();
			Config = _kernel.Get<IConfigurationRepository>();
			// load settings
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "HDT.Plugins.VictoryShot.Resources.Default.ini";
			Settings = new Settings(assembly.GetManifestResourceStream(resourceName), "VictoryShot");
			// other
			_capture = new TrackerCapture();
			Screenshots = new ObservableCollection<Screenshot>();
			MainViewModel = new MainViewModel();
		}				

		public MenuItem CreateMenu()
		{
			var pm = new PluginMenu("Victory Shot", IcoMoon.Trophy,
				new RelayCommand(() => ShowMainView(ViewModelHelper.SettingsString)));
			return pm.Menu;
		}

		public void ButtonPress()
		{
			ShowMainView(ViewModelHelper.SettingsString);
		}

		public async void Load()
		{
			// check for plugin update
			await UpdateCheck("andburn", "hdt-plugin-victoryshot");
			// set the action to run on the game end event
			Events.OnGameEnd(Run);
		}

		public void Unload()
		{
			CloseMainView();
		}

		public void Repeat()
		{

		}

		public async static void Run()
		{
			try
			{
				var mode = Data.GetGameMode();
				// take the screenshots
				await Capture(mode);
				// check what game modes are enabled
				if (IsModeEnabledForScreenshots(mode))
				{
					await WaitUntilInMenu();
					ShowMainView(ViewModelHelper.ScreenshotsString);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
				Notify("VictoryShot Error", e.Message, 15, IcoMoon.Warning, null);
			}
		}

		private static void ShowMainView(string location)
		{
			MainView view = null;
			// check for any open windows
			var open = Application.Current.Windows.OfType<MainView>();
			if (open.Count() == 1)
			{
				view = open.FirstOrDefault();
			}
			else
			{
				CloseMainView();
				// create view
				view = new MainView();
				view.DataContext = MainViewModel;
			}
			// show the window, and restore if needed
			view.Show();
			Logger.Info("window state: " + view.WindowState);
			if (view.WindowState == WindowState.Minimized)
				view.WindowState = WindowState.Normal;
			view.Activate();
			// navigate to location
			MainViewModel.OnNavigation(location);
		}

		public static void CloseMainView()
		{
			foreach (var view in Application.Current.Windows.OfType<MainView>())
			{
				view.Close();
			}
		}

		public static void Notify(string title, string message, int autoClose, string icon=null, Action action=null)
		{
			SlidePanelManager
				.Notification(_kernel.Get<ISlidePanel>(), title, message, icon, action)
				.AutoClose(autoClose);
		}

		private static async Task Capture(string mode)
		{
			try
			{
				if (IsModeEnabledForScreenshots(mode))
				{
					Screenshots.Clear();
					await _capture.CaptureSequence(Screenshots,
						Settings.Get(Strings.Delay).Int,
						Settings.Get(Strings.OutputDir),
						Settings.Get(Strings.NumberOfImages).Int,
						Settings.Get(Strings.DelayBetweenShots).Int);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
				Notify("Screen Capture Failed", e.Message, 15, "error", null);
			}
		}

		private static bool IsModeEnabledForScreenshots(string mode)
		{
			switch (mode.ToLowerInvariant())
			{
				case "ranked":
					return Settings.Get(Strings.RecordRanked).Bool;

				case "casual":
					return Settings.Get(Strings.RecordCasual).Bool;

				case "arena":
					return Settings.Get(Strings.RecordArena).Bool;

				case "brawl":
					return Settings.Get(Strings.RecordBrawl).Bool;

				case "friendly":
					return Settings.Get(Strings.RecordFriendly).Bool;

				case "practice":
					return Settings.Get(Strings.RecordPractice).Bool;

				case "spectator":
				case "none":
					return Settings.Get(Strings.RecordOther).Bool;

				default:
					return false;
			}
		}

		private static async Task WaitUntilInMenu()
		{
			var timeout = 30000;
			var wait = 1000;
			var elapsed = 0;
			while (!Client.IsInMenu())
			{
				await Task.Delay(wait);
				elapsed += wait;
				if (elapsed >= timeout)
					return;
			}
		}

		private async Task UpdateCheck(string user, string repo)
		{
			try
			{
				var latest = await Updater.CheckForUpdate(user, repo, _version);
				if (latest.HasUpdate)
				{
					Logger.Info($"Plugin Update available ({latest.Version})");
					Notify("Plugin Update Available",
						$"[DOWNLOAD]({latest.DownloadUrl}) {Name} v{latest.Version}",
						10, IcoMoon.Download3, () => Process.Start(latest.DownloadUrl));
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Github update failed: {e.Message}");
			}
		}
	}
}