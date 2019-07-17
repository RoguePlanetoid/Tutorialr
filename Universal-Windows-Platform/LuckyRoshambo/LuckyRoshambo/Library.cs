using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const int size = 3;
    private const int lost = 0;
    private const int win = 1;
    private const int draw = 2;
    private readonly int[,] match =
        new int[size, size]
    {
        { draw, lost, win },
        { win, draw, lost },
        { lost, win, draw }
    };
    private readonly string[] options = new string[]
    {
        "\U0000270A", "\U0000270B", "\U0000270C"
    };
    private readonly string[] values = new string[]
    {
        "You Lost!", "You Win!", "You Draw!"
    };

    private Random _random = new Random((int)DateTime.Now.Ticks);

    private Viewbox Option(int value)
    {
        TextBlock textblock = new TextBlock()
        {
            Text = options[value],
            IsColorFontEnabled = true,
            Margin = new Thickness(2),
            TextLineBounds = TextLineBounds.Tight,
            FontFamily = new FontFamily("Segoe UI Emoji"),
            HorizontalTextAlignment = TextAlignment.Center
        };
        return new Viewbox()
        {
            Child = textblock
        };
    }

    private async void Show(string title, int option)
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = title,
            PrimaryButtonText = "OK",
            Content = new Viewbox()
            {
                Width = 150,
                Height = 150,
                Child = Option(option)
            }
        };
        await dialog.ShowAsync();
    }

    private void Choose(int player)
    {
        int computer = _random.Next(0, size - 1);
        int result = match[player, computer];
        Show($"Computer Picked - {values[result]}", computer);
    }

    private void Add(StackPanel panel, int option)
    {
        Button button = new Button()
        {
            Width = 200,
            Height = 200,
            Tag = option,
            Content = Option(option),
            Margin = new Thickness(5)
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            button = (Button)sender;
            Choose((int)button.Tag);
        };
        panel.Children.Add(button);
    }

    private void Layout(Grid grid)
    {
        grid.Children.Clear();
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        for (int i = 0; i < size; i++)
        {
            Add(panel, i);
        }
        grid.Children.Add(panel);
    }

    public void New(Grid grid)
    {
        Layout(grid);
    }
}