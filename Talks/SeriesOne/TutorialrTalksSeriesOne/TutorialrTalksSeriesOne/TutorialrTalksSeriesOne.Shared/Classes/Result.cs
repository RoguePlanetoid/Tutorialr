using System.Windows.Input;

namespace TutorialrTalksSeriesOne
{
    public enum ResultType
    {
        Category,
        Playlist,
        Album,
        Item
    }

    public class Result
    {
        public ResultType Type { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public ICommand Command { get; set; }
    }
}
