using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string title = "Fruit Game";
    private const int size = 3;
    private readonly string[] values =
    {
        "\U0001F34E", // Apple
        "\U0001F347", // Grapes
        "\U0001F34B", // Lemon
        "\U0001F352", // Cherry
        "\U0001F34C", // Banana
        "\U0001F349", // Melon
        "\U0001F34A", // Orange
        "\U0001F514", // Bell
    };

    private int _spins = 0;
    private int[] _board = new int[size];
    private Random _random = new Random((int)DateTime.Now.Ticks);

    private Viewbox Fruit(int type)
    {
        TextBlock textblock = new TextBlock()
        {
            Text = values[type],
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

    private async void Show(string content, int type)
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = $"{content} {title}",
            PrimaryButtonText = "OK",
            Content = new Viewbox()
            {
                Width = 150,
                Height = 150,
                Child = Fruit(type)
            }
        };
        await dialog.ShowAsync();
    }

    private void Add(Grid grid, int column, int type)
    {
        Viewbox viewbox = new Viewbox()
        {
            Width = 100,
            Height = 100,
            Child = Fruit(type),
            Stretch = Stretch.Uniform
        };
        viewbox.SetValue(Grid.ColumnProperty, column);
        grid.Children.Add(viewbox);
    }

    private void Layout(Grid grid)
    {
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        // Setup Grid
        for (int column = 0; (column < size); column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            _board[column] = _random.Next(0, values.Count());
            Add(grid, column, _board[column]);
        }
    }

    private void Winner()
    {
        _spins++;
        if (_board.All(item => item == _board.First()))
        {
            Show($"Spin {_spins} matched", _board.First());
            _spins = 0;
        }
    }

    public void New(Grid grid)
    {
        Layout(grid);
        Winner();
    }
}