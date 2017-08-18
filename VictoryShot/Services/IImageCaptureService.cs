using HDT.Plugins.VictoryShot.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HDT.Plugins.VictoryShot.Services
{
	public interface IImageCaptureService
	{
		Task CaptureSequence(ObservableCollection<Screenshot> list, int delay, string dir, int num, int delayBetween);
	}
}