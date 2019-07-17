using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const int size = 3;
    private const string set_one = "one";
    private const string set_two = "two";
    private const string name_upper = "upper";
    private const string name_lower = "lower";

    private readonly string[] tiles =
    {
        "0,0",
        "0,1", "1,1",
        "0,2", "1,2", "2,2",
        "0,3", "1,3", "2,3", "3,3",
        "0,4", "1,4", "2,4", "3,4", "4,4",
        "0,5", "1,5", "2,5", "3,5", "4,5", "5,5",
        "0,6", "1,6", "2,6", "3,6", "4,6", "5,6", "6,6"
    };
    private readonly byte[][] layout =
    {
                  // a, b, c, d, e, f, g, h, i
        new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 0
        new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 0 }, // 1
        new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 1 }, // 2
        new byte[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 }, // 3
        new byte[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 }, // 4
        new byte[] { 1, 0, 1, 0, 1, 0, 1, 0, 1 }, // 5
        new byte[] { 1, 0, 1, 1, 0, 1, 1, 0, 1 }, // 6
    };
    private readonly string[] tags =
    {
        "a", "b", "c", "d", "e", "f", "g", "h", "i"
    };

    private int _turns = 0;
    private List<int> _one = new List<int>();
    private List<int> _two = new List<int>();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    private Color Colour(string resource)
    {
        return (Color)Application.Current.Resources[resource];
    }

    private Brush Background()
    {
        return new LinearGradientBrush(new GradientStopCollection()
        {
            new GradientStop()
            {
                Color = Colour("SystemAccentColorLight3"),
                Offset = 0.0
            },
            new GradientStop()
            {
                Color = Colour("SystemAccentColorDark3"),
                Offset = 1.0
            }
        }, 90);
    }

    private List<int> Choose(int total)
    {
        return Enumerable.Range(0, total)
        .OrderBy(r => _random.Next(0, total)).ToList();
    }

    private void Add(Grid grid, int row, int column, string name)
    {
        Ellipse element = new Ellipse()
        {
            Name = name,
            Opacity = 0,
            Margin = new Thickness(5),
            Fill = new SolidColorBrush(Colors.WhiteSmoke)
        };
        element.SetValue(Grid.ColumnProperty, column);
        element.SetValue(Grid.RowProperty, row);
        grid.Children.Add(element);
    }

    private Grid Portion(string name)
    {
        Grid grid = new Grid()
        {
            Name = name,
            Width = 100,
            Height = 100,
            Background = Background(),
            Padding = new Thickness(5),
        };
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        int count = 0;
        // Setup Layout
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(grid, row, column, $"{name}.{tags[count]}");
                count++;
            }
        }
        return grid;
    }

    private StackPanel Domino(string name)
    {
        StackPanel panel = new StackPanel()
        {
            Margin = new Thickness(25),
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(Portion($"{name}.{name_upper}"));
        panel.Children.Add(Portion($"{name}.{name_lower}"));
        return panel;
    }

    private void Layout(Grid grid)
    {
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Horizontal
        };
        panel.Children.Add(Domino(set_one));
        panel.Children.Add(Domino(set_two));
        grid.Children.Add(panel);
    }

    private void SetPortion(Grid grid, string name, int value)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            ((UIElement)grid.FindName($"{name}.{tags[i]}"))
            .Opacity = layout[value][i];
        }
    }

    private void Set(Grid grid, string name, string tile)
    {
        string[] pair = tile.Split(',');
        SetPortion(grid, $"{name}.{name_upper}", int.Parse(pair[0]));
        SetPortion(grid, $"{name}.{name_lower}", int.Parse(pair[1]));
    }

    public void New(Grid grid)
    {
        Layout(grid);
        _turns = tiles.Count() - 1;
        _one = Choose(tiles.Count());
        _two = Choose(tiles.Count());
    }

    public void Play(Grid grid)
    {
        if (!grid.Children.Any()) New(grid);
        if (_turns > 0)
        {
            Set(grid, set_one, tiles[_one[_turns]]);
            Set(grid, set_two, tiles[_two[_turns]]);
            _turns--;
        }
        else
        {
            New(grid);
        }
    }
}