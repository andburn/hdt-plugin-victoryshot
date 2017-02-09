using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Services;
using HDT.Plugins.VictoryCap.Models;
using HDT.Plugins.VictoryCap.Services;

namespace HDT.Plugins.VictoryCap.ViewModels
{
	public class ScreenshotsViewModel : ViewModelBase
	{
		private IImageCaptureService _cap;
		private ILoggingService _log;

		public ObservableCollection<Screenshot> Screenshots { get; set; }

		private bool _hasScreenshots;

		public bool HasScreenshots
		{
			get { return _hasScreenshots; }
			set { Set(() => HasScreenshots, ref _hasScreenshots, value); }
		}

		private string _screenshotCountText;

		public string ScreenshotCountText
		{
			get { return _screenshotCountText; }
			set { Set(() => ScreenshotCountText, ref _screenshotCountText, value); }
		}

		public RelayCommand SaveCommand { get; private set; }

		public ScreenshotsViewModel()
			: this(VictoryCap.Logger, new TrackerCapture())
		{
		}

		public ScreenshotsViewModel(ILoggingService logger, IImageCaptureService capture)
		{
			_cap = capture;
			_log = logger;

			Screenshots = VictoryCap.Screenshots;
			UpdateCount();
			UpdateCountText();

			Screenshots.CollectionChanged += Screenshots_CollectionChanged;
			SaveCommand = new RelayCommand(async () => await Save());
		}

		private void Screenshots_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateCount();
			UpdateCountText();
		}

		private void UpdateCountText()
		{
			ScreenshotCountText = HasScreenshots ?
				$"{Screenshots.Count} Images Captured" :
				"No Captures Available";
		}

		private void UpdateCount()
		{
			HasScreenshots = Screenshots != null && Screenshots.Any();
		}

		private async Task Save()
		{
			var screenshot = Screenshots?.FirstOrDefault(s => s.IsSelected);
			if (screenshot != null)
			{
				_log.Debug($"Attempting to save screenshot #{screenshot.Index}");
				try
				{
					await ViewModelHelper.SaveImage(screenshot);
					// show saved message
					ScreenshotCountText = "Screenshot Saved";
					// wait then clear screenshots
					await Task.Delay(1000);
					Screenshots.Clear();
				}
				catch (Exception e)
				{
					_log.Error(e.Message);
				}
			}
			else
			{
				_log.Debug($"No screenshot selected (len={Screenshots?.Count})");
			}
		}
	}
}