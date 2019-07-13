using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const string title = "Lucky Roulette";
    private const int size = 400;
    private const int rim = 50;

    private readonly int[] wheel =
    {
        0, 32, 15, 19, 4, 21, 2, 25, 17,
        34, 6, 27, 13, 36, 11, 30, 8, 23,
        10, 5, 24, 16, 33, 1, 20, 14, 31,
        9, 22, 18, 29, 7, 28, 12, 35, 3, 26
    };
    private readonly List<int> _values = Enumerable.Range(0, 36).ToList();

    private int _spins = 0;
    private int _spinValue = 0;
    private int _pickValue = 0;
    private Random _random = new Random((int)DateTime.Now.Ticks);

    private void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private bool IsOdd(int value)
    {
        return value % 2 != 0;
    }

    private Color Fill(int value)
    {
        Color fill;
        if (value >= 1 && value <= 10 ||
            value >= 19 && value <= 28)
        {
            fill = IsOdd(value) ? Colors.Black : Colors.DarkRed;
        }
        else if (value >= 11 && value <= 18 ||
                 value >= 29 && value <= 36)
        {
            fill = IsOdd(value) ? Colors.DarkRed : Colors.Black;
        }
        else if (value == 0)
        {
            fill = Colors.DarkGreen;
        }
        return fill;
    }

    private Grid Pocket(int value)
    {
        Color fill = Fill(value);
        Grid grid = new Grid()
        {
            Width = size,
            Height = size
        };
        Grid pocket = new Grid()
        {
            Width = 26,
            Height = rim,
            CornerRadius = new CornerRadius(4),
            Background = new SolidColorBrush(fill),
            VerticalAlignment = VerticalAlignment.Top
        };
        TextBlock text = new TextBlock()
        {
            FontSize = 20,
            Text = $"{value}",
            VerticalAlignment = VerticalAlignment.Top,
            Foreground = new SolidColorBrush(Colors.Gold),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        Ellipse ball = new Ellipse()
        {
            Width = 20,
            Height = 20,
            Opacity = 0,
            Name = $"{value}",
            Margin = new Thickness(0, 0, 0, 4),
            Fill = new SolidColorBrush(Colors.Snow),
            VerticalAlignment = VerticalAlignment.Bottom
        };
        pocket.Children.Add(text);
        pocket.Children.Add(ball);
        grid.Children.Add(pocket);
        return grid;
    }

    private Viewbox Layout()
    {
        Canvas canvas = new Canvas()
        {
            Width = size,
            Height = size,
        };
        Ellipse ellipse = new Ellipse()
        {
            Width = size,
            Height = size,
            StrokeThickness = rim,
            Stroke = new SolidColorBrush(Colors.Peru)
        };
        canvas.Children.Add(ellipse);
        int index = 0;
        double radiusX = canvas.Width * 0.5;
        double radiusY = canvas.Height * 0.5;
        double delta = 2 * Math.PI / wheel.Length;
        Point centre = new Point(canvas.Width / 2, canvas.Height / 2);
        foreach (int value in wheel)
        {
            Grid pocket = Pocket(value);
            Size size = new Size(pocket.DesiredSize.Width,
                pocket.DesiredSize.Height);
            double angle = index * delta;
            double x = centre.X + radiusX *
                Math.Cos(angle) - size.Width / 2;
            double y = centre.Y + radiusY *
                Math.Sin(angle) - size.Height / 2;
            pocket.RenderTransformOrigin = new Point(0.5, 0.5);
            pocket.RenderTransform = new RotateTransform()
            {
                Angle = angle * 180 / Math.PI
            };
            pocket.Arrange(new Rect(x, y, size.Width, size.Height));
            canvas.Children.Add(pocket);
            index++;
        }
        Viewbox viewbox = new Viewbox()
        {
            Child = canvas
        };
        return viewbox;
    }

    private void Set(Grid grid, int value, byte opacity)
    {
        UIElement element = (UIElement)grid.FindName($"{value}");
        if (element != null) element.Opacity = opacity;
    }

    private async void Choose(Grid grid)
    {
        for (int i = 0; i < _values.Count; i++)
        {
            Set(grid, _values[i], 0);
        }
        ComboBox combobox = new ComboBox()
        {
            SelectedIndex = 0,
            ItemsSource = _values,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        ContentDialog dialog = new ContentDialog()
        {
            Content = combobox,
            Title = "Pick a Number",
            PrimaryButtonText = "Spin",
            SecondaryButtonText = "Cancel"
        };
        ContentDialogResult result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            _spins++;
            _spinValue = _values[_random.Next(0, _values.Count)];
            _pickValue = (int)((ComboBox)dialog.Content).SelectedValue;
            Set(grid, _spinValue, 1); // Show Ball
            if (_spinValue == _pickValue) // Check Win
            {
                _spins = 0;
                Show($"Won {_spins} with {_spinValue}", title);
            }
            else
            {
                Show($"Lost {_spins} with {_pickValue} was {_spinValue}", title);
            }
        }
    }

    public void New(Grid grid)
    {
        _spins = 0;
        grid.Children.Clear();
        grid.Children.Add(Layout());
    }

    public void Play(Grid grid)
    {
        if (!grid.Children.Any()) New(grid);
        Choose(grid);
    }
}

