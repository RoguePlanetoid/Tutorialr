using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const int size = 3;

    private static readonly byte[][] layout =
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
    private readonly Color _accent =
        (Color)Application.Current.Resources["SystemAccentColor"];

    private Random _random = new Random((int)DateTime.UtcNow.Ticks);


    private void Add(ref Grid grid, int row, int column)
    {
        Ellipse element = new Ellipse()
        {
            Fill = new SolidColorBrush(_accent),
            Margin = new Thickness(5),
            Opacity = 0
        };
        element.SetValue(Grid.ColumnProperty, column);
        element.SetValue(Grid.RowProperty, row);
        grid.Children.Add(element);
    }

    private void Set(ref Grid grid, int row, int column, byte opacity)
    {
        Grid element = (Grid)((Viewbox)grid.Children
            .FirstOrDefault()).Child;
        Ellipse ellipse = element.Children.Cast<Ellipse>()
            .FirstOrDefault(f =>
        Grid.GetRow(f) == row && Grid.GetColumn(f) == column);
        if (ellipse != null) ellipse.Opacity = opacity;
    }

    private void Update(ref Grid grid, int value)
    {
        int count = 0;
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                Set(ref grid, row, column, layout[value][count]);
                count++;
            }
        }
    }

    public void New(ref Grid grid)
    {
        grid.Children.Clear();
        Grid element = new Grid()
        {
            Width = 100,
            Height = 100,
            Padding = new Thickness(5)
        };
        // Setup Grid
        for (int index = 0; index < size; index++)
        {
            element.RowDefinitions.Add(new RowDefinition());
            element.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                Add(ref element, row, column);
            }
        }
        Viewbox viewbox = new Viewbox()
        {
            Child = element
        };
        grid.Children.Add(viewbox);
        Update(ref grid, 0);
    }

    public void Get(ref Grid grid)
    {
        if (!grid.Children.Any()) New(ref grid);
        Update(ref grid, _random.Next(1, 7));
    }
}