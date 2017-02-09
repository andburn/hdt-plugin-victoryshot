using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.VictoryCap.Utilities;
using Ookii.Dialogs.Wpf;

namespace HDT.Plugins.VictoryCap.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public string OutputDir
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "OutputDir").Value;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "OutputDir", value);
				RaisePropertyChanged("OutputDir");
			}
		}

		public int Delay
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "Delay").Int;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "Delay", value);
				RaisePropertyChanged("Delay");
			}
		}

		public int NumberOfImages
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "NumberOfImages").Int;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "NumberOfImages", value);
				RaisePropertyChanged("NumberOfImages");
			}
		}

		public int DelayBetweenShots
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "DelayBetweenShots").Int;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "DelayBetweenShots", value);
				RaisePropertyChanged("DelayBetweenShots");
			}
		}

		public bool RecordArena
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "RecordArena").Bool;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "RecordArena", value);
				RaisePropertyChanged("RecordArena");
			}
		}

		public bool RecordBrawl
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "RecordBrawl").Bool;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "RecordBrawl", value);
				RaisePropertyChanged("RecordBrawl");
			}
		}

		public bool RecordCasual
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "RecordCasual").Bool;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "RecordCasual", value);
				RaisePropertyChanged("RecordCasual");
			}
		}

		public bool RecordFriendly
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "RecordFriendly").Bool;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "RecordFriendly", value);
				RaisePropertyChanged("RecordFriendly");
			}
		}

		public bool RecordOther
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "RecordOther").Bool;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "RecordOther", value);
				RaisePropertyChanged("RecordOther");
			}
		}

		public bool RecordPractice
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "RecordPractice").Bool;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "RecordPractice", value);
				RaisePropertyChanged("RecordPractice");
			}
		}

		public bool RecordRanked
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "RecordRanked").Bool;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "RecordRanked", value);
				RaisePropertyChanged("RecordRanked");
			}
		}

		public string FileNamePattern
		{
			get
			{
				return VictoryCap.Settings.Get("ScreenShot", "FileNamePattern").Value;
			}
			set
			{
				VictoryCap.Settings.Set("ScreenShot", "FileNamePattern", value);
				RaisePropertyChanged("FileNamePattern");
			}
		}

		private string _patternPreview;

		public string PatternPreview
		{
			get { return _patternPreview; }
			set { Set(() => PatternPreview, ref _patternPreview, value); }
		}

		public RelayCommand<string> PatternChangedCommand { get; private set; }
		public RelayCommand ChooseDirCommand { get; private set; }

		public SettingsViewModel()
		{
			UpdatePattern(FileNamePattern);
			PatternChangedCommand = new RelayCommand<string>(x => UpdatePattern(x));
			ChooseDirCommand = new RelayCommand(() => ChooseOuputDir());
		}

		private void ChooseOuputDir()
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.Description = "Select a folder";
			dialog.UseDescriptionForTitle = true;

			// set initial directory to setting if exists
			var current = VictoryCap.Settings.Get("ScreenShot", "OutputDir").Value;
			if (Directory.Exists(current))
				dialog.SelectedPath = current;

			if ((bool)dialog.ShowDialog())
			{
				VictoryCap.Settings.Set("ScreenShot", "OutputDir", dialog.SelectedPath);
				RaisePropertyChanged("OutputDir");
			}
		}

		private void UpdatePattern(string x)
		{
			NamingPattern np = null;
			var success = NamingPattern.TryParse(x, out np);
			if (success)
				PatternPreview = np.Apply("Mage", "Druid", "Player", "Opponent");
			else
				PatternPreview = "the pattern is invalid";
		}
	}
}