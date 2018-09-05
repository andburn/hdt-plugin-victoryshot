using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.VictoryShot.Utilities;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace HDT.Plugins.VictoryShot.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public string OutputDir
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.OutputDir).Value;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.OutputDir, value);
				RaisePropertyChanged(Strings.OutputDir);
			}
		}

		public int Delay
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.Delay).Int;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.Delay, value);
				RaisePropertyChanged(Strings.Delay);
			}
		}

		public int NumberOfImages
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.NumberOfImages).Int;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.NumberOfImages, value);
				RaisePropertyChanged(Strings.NumberOfImages);
			}
		}

		public int DelayBetweenShots
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.DelayBetweenShots).Int;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.DelayBetweenShots, value);
				RaisePropertyChanged(Strings.DelayBetweenShots);
			}
		}

		public bool RecordArena
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.RecordArena).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.RecordArena, value);
				RaisePropertyChanged(Strings.RecordArena);
			}
		}

		public bool RecordBrawl
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.RecordBrawl).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.RecordBrawl, value);
				RaisePropertyChanged(Strings.RecordBrawl);
			}
		}

		public bool RecordCasual
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.RecordCasual).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.RecordCasual, value);
				RaisePropertyChanged(Strings.RecordCasual);
			}
		}

		public bool RecordFriendly
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.RecordFriendly).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.RecordFriendly, value);
				RaisePropertyChanged(Strings.RecordFriendly);
			}
		}

		public bool RecordOther
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.RecordOther).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.RecordOther, value);
				RaisePropertyChanged(Strings.RecordOther);
			}
		}

		public bool RecordPractice
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.RecordPractice).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.RecordPractice, value);
				RaisePropertyChanged(Strings.RecordPractice);
			}
		}

		public bool RecordRanked
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.RecordRanked).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.RecordRanked, value);
				RaisePropertyChanged(Strings.RecordRanked);
			}
		}

		public bool AltCapture
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.AltCapture).Bool;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.AltCapture, value);
				RaisePropertyChanged(Strings.AltCapture);
			}
		}

		public string FileNamePattern
		{
			get
			{
				return VictoryShot.Settings.Get(Strings.FileNamePattern).Value;
			}
			set
			{
				VictoryShot.Settings.Set(Strings.FileNamePattern, value);
				RaisePropertyChanged(Strings.FileNamePattern);
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
			var current = VictoryShot.Settings.Get(Strings.OutputDir).Value;
			if (Directory.Exists(current))
				dialog.SelectedPath = current;

			if ((bool)dialog.ShowDialog())
			{
				VictoryShot.Settings.Set(Strings.OutputDir, dialog.SelectedPath);
				RaisePropertyChanged(Strings.OutputDir);
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