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
    private const int total = 6;
    private const int maximum = 59;

    private Dictionary<int, Color> _style = new Dictionary<int, Color>()
    {
        { 0, Colors.White },
        { 10, Colors.RoyalBlue },
        { 20, Colors.HotPink },
        { 30, Colors.MediumSpringGreen },
        { 40, Colors.Gold },
        { 50, Colors.Indigo },
    };

    private Random _random = new Random((int)DateTime.UtcNow.Ticks);

    private List<int> Choose()
    {
        int number;
        List<int> numbers = new List<int>();
        while (numbers.Count < total) // Select Numbers
        {
            number = _random.Next(1, maximum + 1);
            if (!numbers.Contains(number) || numbers.Count < 1)
            {
                numbers.Add(number); // Add if not Chosen or None
            }
        }
        numbers.Sort();
        return numbers;
    }

    private void Add(ref StackPanel panel, int value)
    {
        Color fill = _style.Where(w => value > w.Key)
        .Select(s => s.Value).LastOrDefault();
        Grid element = new Grid()
        {
            Width = 75,
            Height = 75,
            Margin = new Thickness(5),
        };
        Ellipse ball = new Ellipse()
        {
            StrokeThickness = 5,
            Stroke = new SolidColorBrush(fill)
        };
        TextBlock label = new TextBlock()
        {
            FontSize = 32,
            Text = value.ToString(),
            TextLineBounds = TextLineBounds.Tight,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Colors.Black),
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        element.Children.Add(ball);
        element.Children.Add(label);
        panel.Children.Add(element);
    }

    public void New(ref Grid grid)
    {
        grid.Children.Clear();
        StackPanel panel = new StackPanel()
        {
            Height = 100,
            Orientation = Orientation.Horizontal
        };
        foreach (int number in Choose())
        {
            Add(ref panel, number);
        }
        Viewbox viewbox = new Viewbox()
        {
            Child = panel
        };
        grid.Children.Add(viewbox);
    }
}