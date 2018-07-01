using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Controls;
using HDT.Plugins.Common.Providers.Metro;
using HDT.Plugins.Common.Providers.Tracker;
using HDT.Plugins.Common.Providers.Web;
using HDT.Plugins.Common.Services;
using HDT.Plugins.Common.Settings;
using HDT.Plugins.Common.Utils;
using HDT.Plugins.VictoryShot.Models;
using HDT.Plugins.VictoryShot.Services;
using HDT.Plugins.VictoryShot.Utilities;
using HDT.Plugins.VictoryShot.ViewModels;
using HDT.Plugins.VictoryShot.Views;
using Hearthstone_Deck_Tracker.Plugins;
using Ninject;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HDT.Plugins.VictoryShot
{
	public class VictoryShot : IPlugin
	{
		private static IKernel _kernel;
		private static IImageCaptureService _capture;

		public static IUpdateService Updater;
		public static ILoggingService Logger;
		public static IDataRepository Data;
		public static IEventsService Events;
		public static IGameClientService Client;
		public static IConfigurationRepository Config;
		public static Settings Settings;

		public static ObservableCollection<Screenshot> Screenshots;
		public static MainViewModel MainViewModel;

		public string Name => "Victory Shot";

		public string Description => "Helps in capturing victory/defeat screen shots after a match.";

		public string ButtonText => "Settings";

		public string Author => "andburn";

		private Version _version;

		public Version Version
		{
			get
			{
				if (_version == null)
					_version = GetVersion() ?? new Version(0, 0, 0, 0);
				return _version;
			}
		}

		public VictoryShot()
		{
			_kernel = GetKernel();
			// initialize services
			Updater = _kernel.Get<IUpdateService>();
			Logger = _kernel.Get<ILoggingService>();
			Data = _kernel.Get<IDataRepository>();
			Events = _kernel.Get<IEventsService>();
			Client = _kernel.Get<IGameClientService>();
			Config = _kernel.Get<IConfigurationRepository>();
			NotificationManager.SetService(_kernel.Get<IToastService>());
			// load settings
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "HDT.Plugins.VictoryShot.Resources.Default.ini";
			Settings = new Settings(assembly.GetManifestResourceStream(resourceName), "VictoryShot");
			// other
			_capture = new TrackerCapture();
			Screenshots = new ObservableCollection<Screenshot>();
			MainViewModel = new MainViewModel();
		}

		private MenuItem _menuItem;

		public MenuItem MenuItem
		{
			get
			{
				if (_menuItem == null)
					_menuItem = CreateMenu();
				return _menuItem;
			}
		}

		public void OnButtonPress()
		{
			ShowMainView(ViewModelHelper.SettingsString);
		}

		public async void OnLoad()
		{
			// check for plugin update
			await UpdateCheck("andburn", "hdt-plugin-victoryshot");
			// set the action to run on the game end event
			Events.OnGameEnd(Run);
		}

		public void OnUnload()
		{
			CloseMainView();
		}

		public void OnUpdate()
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
				Notify("VictoryShot Error", e.Message, IcoMoon.Warning);
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

		public static void Notify(string title, string message, string icon = null, string url = null)
		{
			NotificationManager.ShowToast(title, message, icon, url);
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
				Notify("Screen Capture Failed", e.Message, "error");
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
				var latest = await Updater.CheckForUpdate(user, repo, Version);
				if (latest.HasUpdate)
				{
					Logger.Info($"Plugin Update available ({latest.Version})");
					Notify("Plugin Update Available",
						$"{Name} v{latest.Version}",
						IcoMoon.Download3, latest.DownloadUrl);
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Github update failed: {e.Message}");
			}
		}

		private IKernel GetKernel()
		{
			var kernel = new StandardKernel();
			kernel.Bind<IDataRepository>().To<TrackerDataRepository>().InSingletonScope();
			kernel.Bind<IUpdateService>().To<GitHubUpdateService>().InSingletonScope();
			kernel.Bind<ILoggingService>().To<TrackerLoggingService>().InSingletonScope();
			kernel.Bind<IEventsService>().To<TrackerEventsService>().InSingletonScope();
			kernel.Bind<IGameClientService>().To<TrackerClientService>().InSingletonScope();
			kernel.Bind<IConfigurationRepository>().To<TrackerConfigRepository>().InSingletonScope();
			kernel.Bind<ISlidePanel>().To<MetroSlidePanel>();
			kernel.Bind<IToastService>().To<TrackerToastService>().InSingletonScope();
			return kernel;
		}

		private MenuItem CreateMenu()
		{
			var pm = new PluginMenu("Victory Shot", IcoMoon.Trophy,
				new RelayCommand(() => ShowMainView(ViewModelHelper.SettingsString)));
			return pm.Menu;
		}

		private Version GetVersion()
		{
			return GitVersion.Get(Assembly.GetExecutingAssembly(), this);
		}
	}
}