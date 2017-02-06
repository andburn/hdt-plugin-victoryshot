using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HDT.Plugins.VictoryCap.Models;

namespace HDT.Plugins.VictoryCap.Services
{
	public interface IImageCaptureService
	{
		Task<ObservableCollection<Screenshot>> CaptureSequence(int delay, string dir, int num, int delayBetween);

		Task SaveImage(Screenshot img);
	}
}