using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Controls.SlidePanels;
using HDT.Plugins.Common.Plugin;
using HDT.Plugins.Common.Providers;
using HDT.Plugins.Common.Services;
using HDT.Plugins.Common.Settings;
using HDT.Plugins.VictoryCap.Models;
using HDT.Plugins.VictoryCap.Services;
using HDT.Plugins.VictoryCap.ViewModels;
using HDT.Plugins.VictoryCap.Views;

namespace HDT.Plugins.VictoryCap
{
	[Name("Victory Cap")]
	[Description("Takes screen shots of the victory/defeat screens.")]
	public class VictoryCap : PluginBase
	{
		public static readonly IUpdateService Updater;
		public static readonly ILoggingService Logger;
		public static readonly IDataRepository Data;
		public static readonly IEventsService Events;
		public static readonly IGameClientService Client;
		public static readonly IConfigurationRepository Config;
		public static readonly Settings Settings;		

		private static IImageCaptureService _capture;

		public static ObservableCollection<Screenshot> Screenshots;
		public static MainViewModel MainViewModel;

		static VictoryCap()
		{
			// initialize services
			var resolver = Injector.Instance.Container;
			Updater = resolver.GetInstance<IUpdateService>();
			Logger = resolver.GetInstance<ILoggingService>();
			Data = resolver.GetInstance<IDataRepository>();
			Events = resolver.GetInstance<IEventsService>();
			Client = resolver.GetInstance<IGameClientService>();
			Config = resolver.GetInstance<IConfigurationRepository>();
			// load settings
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "HDT.Plugins.VictoryCap.Resources.Default.ini";
			Settings = new Settings(assembly.GetManifestResourceStream(resourceName), "VictoryCap");
			// other
			_capture = new TrackerCapture();
			MainViewModel = new MainViewModel();
		}

		public VictoryCap()
			: base(Assembly.GetExecutingAssembly())
		{
		}

		private MenuItem _menuItem;

		public override MenuItem MenuItem
		{
			get
			{
				if (_menuItem == null)
					_menuItem = CreatePluginMenu();
				return _menuItem;
			}
		}

		private MenuItem CreatePluginMenu()
		{
			var pm = new PluginMenu("Victory Cap", "trophy", 
				new RelayCommand(() => ShowMainView(ViewModelHelper.SettingsString)));
			return pm.Menu;
		}

		public override void OnButtonPress()
		{
			ShowMainView(ViewModelHelper.SettingsString);
		}

		public override async void OnLoad()
		{
			// check for plugin update
			await UpdateCheck("VictoryCap", "hdt-plugin-victorycap");
			// set the action to run on the game end event
			Events.OnGameEnd(Run);
		}

		public override void OnUnload()
		{
			CloseMainView();
		}

		public async static void Run()
		{
			try
			{
				var mode = Data.GetGameMode();
				// close any already open views
				CloseMainView();
				// take the screenshots
				Screenshots = await Capture(mode);
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
				Notify("VictoryCap Error", e.Message, 15, "error", null);
			}
		}

		private static void ShowMainView(string location)
		{
			// close any open windows
			CloseMainView();
			// create view
			MainView view = new MainView();
			view.DataContext = MainViewModel;
			view.Show();
			view.Activate();
			// navigate to location
			MainViewModel.OnNavigation(location);
		}

		public static void CloseMainView()
		{
			foreach (var view in Application.Current.Windows.OfType<MainView>())
				view.Close();
		}

		public static void Notify(string title, string message, int autoClose, string icon = null, Action action = null)
		{
			SlidePanelManager
				.Notification(title, message, icon, action)
				.AutoClose(autoClose);
		}

		private static async Task<ObservableCollection<Screenshot>> Capture(string mode)
		{
			ObservableCollection<Screenshot> screenshots = null;
			try
			{
				if (Settings.Get("ScreenShot", "ScreenshotEnabled").Bool && IsModeEnabledForScreenshots(mode))
				{
					screenshots = await _capture.CaptureSequence(
						Settings.Get("ScreenShot", "Delay").Int,
						Settings.Get("ScreenShot", "OutputDir"),
						Settings.Get("ScreenShot", "NumberOfImages").Int,
						Settings.Get("ScreenShot", "DelayBetweenShots").Int);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
				Notify("Screen Capture Failed", e.Message, 15, "error", null);
			}
			return screenshots;
		}

		private static bool IsModeEnabledForScreenshots(string mode)
		{
			switch (mode.ToLowerInvariant())
			{
				case "ranked":
					return Settings.Get("ScreenShot", "RecordRanked").Bool;

				case "casual":
					return Settings.Get("ScreenShot", "RecordCasual").Bool;

				case "arena":
					return Settings.Get("ScreenShot", "RecordArena").Bool;

				case "brawl":
					return Settings.Get("ScreenShot", "RecordBrawl").Bool;

				case "friendly":
					return Settings.Get("ScreenShot", "RecordFriendly").Bool;

				case "practice":
					return Settings.Get("ScreenShot", "RecordPractice").Bool;

				case "spectator":
				case "none":
					return Settings.Get("ScreenShot", "RecordOther").Bool;

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

		private async Task UpdateCheck(string name, string repo)
		{
			var uri = new Uri($"https://api.github.com/repos/andburn/{repo}/releases");
			Logger.Debug("update uri = " + uri);
			try
			{
				var latest = await Updater.CheckForUpdate(uri, Version);
				if (latest.HasUpdate)
				{
					Logger.Info($"Plugin Update available ({latest.Version})");
					SlidePanelManager
						.Notification("Plugin Update Available",
							$"[DOWNLOAD]({latest.DownloadUrl}) {name} v{latest.Version}",
							"download3",
							() => Process.Start(latest.DownloadUrl))
						.AutoClose(10);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
	}
}