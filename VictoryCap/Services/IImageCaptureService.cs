using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HDT.Plugins.VictoryCap.Models;

namespace HDT.Plugins.VictoryCap.Services
{
	public interface IImageCaptureService
	{
		Task CaptureSequence(ObservableCollection<Screenshot> list, int delay, string dir, int num, int delayBetween);

		bool IsCapturing { get; }
	}
}