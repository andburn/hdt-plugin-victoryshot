using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.VictoryShot.Utilities;
using Ookii.Dialogs.Wpf;

namespace HDT.Plugins.VictoryShot.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public string OutputDir
		{
			get
			{
				return VictoryShot.Settings.Get("OutputDir").Value;
			}
			set
			{
				VictoryShot.Settings.Set("OutputDir", value);
				RaisePropertyChanged("OutputDir");
			}
		}

		public int Delay
		{
			get
			{
				return VictoryShot.Settings.Get("Delay").Int;
			}
			set
			{
				VictoryShot.Settings.Set("Delay", value);
				RaisePropertyChanged("Delay");
			}
		}

		public int NumberOfImages
		{
			get
			{
				return VictoryShot.Settings.Get("NumberOfImages").Int;
			}
			set
			{
				VictoryShot.Settings.Set("NumberOfImages", value);
				RaisePropertyChanged("NumberOfImages");
			}
		}

		public int DelayBetweenShots
		{
			get
			{
				return VictoryShot.Settings.Get("DelayBetweenShots").Int;
			}
			set
			{
				VictoryShot.Settings.Set("DelayBetweenShots", value);
				RaisePropertyChanged("DelayBetweenShots");
			}
		}

		public bool RecordArena
		{
			get
			{
				return VictoryShot.Settings.Get("RecordArena").Bool;
			}
			set
			{
				VictoryShot.Settings.Set("RecordArena", value);
				RaisePropertyChanged("RecordArena");
			}
		}

		public bool RecordBrawl
		{
			get
			{
				return VictoryShot.Settings.Get("RecordBrawl").Bool;
			}
			set
			{
				VictoryShot.Settings.Set("RecordBrawl", value);
				RaisePropertyChanged("RecordBrawl");
			}
		}

		public bool RecordCasual
		{
			get
			{
				return VictoryShot.Settings.Get("RecordCasual").Bool;
			}
			set
			{
				VictoryShot.Settings.Set("RecordCasual", value);
				RaisePropertyChanged("RecordCasual");
			}
		}

		public bool RecordFriendly
		{
			get
			{
				return VictoryShot.Settings.Get("RecordFriendly").Bool;
			}
			set
			{
				VictoryShot.Settings.Set("RecordFriendly", value);
				RaisePropertyChanged("RecordFriendly");
			}
		}

		public bool RecordOther
		{
			get
			{
				return VictoryShot.Settings.Get("RecordOther").Bool;
			}
			set
			{
				VictoryShot.Settings.Set("RecordOther", value);
				RaisePropertyChanged("RecordOther");
			}
		}

		public bool RecordPractice
		{
			get
			{
				return VictoryShot.Settings.Get("RecordPractice").Bool;
			}
			set
			{
				VictoryShot.Settings.Set("RecordPractice", value);
				RaisePropertyChanged("RecordPractice");
			}
		}

		public bool RecordRanked
		{
			get
			{
				return VictoryShot.Settings.Get("RecordRanked").Bool;
			}
			set
			{
				VictoryShot.Settings.Set("RecordRanked", value);
				RaisePropertyChanged("RecordRanked");
			}
		}

		public string FileNamePattern
		{
			get
			{
				return VictoryShot.Settings.Get("FileNamePattern").Value;
			}
			set
			{
				VictoryShot.Settings.Set("FileNamePattern", value);
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
			var current = VictoryShot.Settings.Get("OutputDir").Value;
			if (Directory.Exists(current))
				dialog.SelectedPath = current;

			if ((bool)dialog.ShowDialog())
			{
				VictoryShot.Settings.Set("OutputDir", dialog.SelectedPath);
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