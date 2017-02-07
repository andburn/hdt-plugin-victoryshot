using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Services;
using HDT.Plugins.VictoryCap.Models;
using HDT.Plugins.VictoryCap.Services;
using HDT.Plugins.VictoryCap.Utilities;

namespace HDT.Plugins.VictoryCap.ViewModels
{
	public class ScreenshotsViewModel : ViewModelBase
	{
		private IDataRepository _repository;
		private IImageCaptureService _cap;
		private ILoggingService _log;

		public ObservableCollection<Screenshot> Screenshots { get; set; }

		private string _note;

		public string Note
		{
			get { return _note; }
			set { Set(() => Note, ref _note, value); }
		}

		private string _playerClass;

		public string PlayerClass
		{
			get { return _playerClass; }
			set { Set(() => PlayerClass, ref _playerClass, value); }
		}

		private bool _hasScreenshots;

		public bool HasScreenshots
		{
			get { return _hasScreenshots; }
			set { Set(() => HasScreenshots, ref _hasScreenshots, value); }
		}

		public bool HasNoScreenshots
		{
			get { return !_hasScreenshots; }
			set { Set(() => HasNoScreenshots, ref _hasScreenshots, !value); }
		}

		private string _screenshotCountText;

		public string ScreenshotCountText
		{
			get { return _screenshotCountText; }
			set { Set(() => ScreenshotCountText, ref _screenshotCountText, value); }
		}

		public RelayCommand WindowClosingCommand { get; private set; }

		public ScreenshotsViewModel()
			: this(VictoryCap.Data, VictoryCap.Logger, new TrackerCapture())
		{
		}

		public ScreenshotsViewModel(IDataRepository track, ILoggingService logger, IImageCaptureService capture)
		{
			HasScreenshots = false;

			if (IsInDesignMode)
			{
				Screenshots = DesignerData.GenerateScreenshots();
				HasScreenshots = true;
			}
			else
			{
				_repository = track;
			}
			_cap = capture;
			_log = logger;

			Screenshots = DesignerData.GenerateScreenshots();//VictoryCap.Screenshots;
			HasScreenshots = Screenshots?.Count > 0;
			if (HasScreenshots)
			{
				ScreenshotCountText = $"{Screenshots.Count} Images Captured";
			}

			PropertyChanged += ScreenshotsViewModel_PropertyChanged;

			WindowClosingCommand = new RelayCommand(async () => await Closing());
		}

		public ScreenshotsViewModel(ObservableCollection<Screenshot> screenshots)
			: this()
		{
			Screenshots = screenshots;
			HasScreenshots = Screenshots != null && Screenshots.Any();
		}

		private void ScreenshotsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// TODO remove
			if (e.PropertyName == "Note")
				_repository.UpdateGameNote(Note);
		}

		private async Task Closing()
		{
			var screenshot = Screenshots?.FirstOrDefault(s => s.IsSelected);
			if (screenshot != null)
			{
				_log.Debug($"Attempting to save screenshot #{screenshot.Index}");
				try
				{
					await ViewModelHelper.SaveImage(screenshot);
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