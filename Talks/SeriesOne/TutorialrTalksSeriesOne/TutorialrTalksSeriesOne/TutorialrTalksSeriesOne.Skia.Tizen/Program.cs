using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace TutorialrTalksSeriesOne.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new TutorialrTalksSeriesOne.App(), args);
            host.Run();
        }
    }
}
