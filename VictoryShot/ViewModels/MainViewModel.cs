using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;

namespace HDT.Plugins.VictoryShot.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private Dictionary<string, ViewModelBase> viewModels = new Dictionary<string, ViewModelBase> {
			{ ViewModelHelper.SettingsString, new SettingsViewModel() },
			{ ViewModelHelper.ScreenshotsString, new ScreenshotsViewModel() }
		};

		private string _contentTitle;

		public string ContentTitle
		{
			get { return _contentTitle; }
			set { Set(() => ContentTitle, ref _contentTitle, value); }
		}

		private ViewModelBase _contentViewModel;

		public ViewModelBase ContentViewModel
		{
			get { return _contentViewModel; }
			set { Set(() => ContentViewModel, ref _contentViewModel, value); }
		}

		public RelayCommand<string> NavigateCommand { get; private set; }

		public MainViewModel()
		{
			// set default content to screenshot view
			ContentViewModel = viewModels[ViewModelHelper.ScreenshotsString];
			NavigateCommand = new RelayCommand<string>(x => OnNavigation(x));
		}

		public void OnNavigation(string location)
		{
			var key = location.ToLower();
			if (viewModels.ContainsKey(key))
			{
				// change only if different to current
				if (ContentViewModel != viewModels[key])
				{
					ContentViewModel = viewModels[key];
					if (key.Length > 2)
						ContentTitle = key.Substring(0, 1).ToUpper() + key.Substring(1);
				}
			}
		}
	}
}