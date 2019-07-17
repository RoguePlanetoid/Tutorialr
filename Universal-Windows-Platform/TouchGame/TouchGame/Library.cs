using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string title = "Touch Game";
    private const int size = 2;
    private const int speed = 800;
    private const int light = 400;
    private const int click = 200;
    private const int level = 100;
    private readonly Dictionary<int, string> _square = 
        new Dictionary<int, string>()
    {
        { 0, "\U0001F7E5" }, // Red Square
        { 1, "\U0001F7E6" }, // Blue Square
        { 2, "\U0001F7E9" }, // Green Square
        { 3, "\U0001F7E8" }, // Yellow Square
    };

    private int _turn = 0;
    private int _count = 0;
    private bool _play = false;
    private bool _isTimer = false;
    private List<int> _items = new List<int>();
    private DispatcherTimer _timer = new DispatcherTimer();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private List<int> Choose(int start, int finish, int total)
    {
        int number;
        List<int> numbers = new List<int>();
        while (numbers.Count < total) // Select Numbers
        {
            // Random non-unique Number between Start and Finish
            number = _random.Next(start, finish + 1);
            numbers.Add(number); // Add Number
        }
        return numbers;
    }

    private Viewbox Square(int value)
    {
        TextBlock textblock = new TextBlock()
        {
            Text = _square[value],
            IsColorFontEnabled = true,
            TextLineBounds = TextLineBounds.Tight,
            FontFamily = new FontFamily("Segoe UI Emoji"),
            HorizontalTextAlignment = TextAlignment.Center
        };
        return new Viewbox()
        {
            Child = textblock
        };
    }

    private void Score(int value)
    {
        if (value == _items[_count])
        {
            if (_count < _turn)
            {
                _count++;
            }
            else
            {
                _isTimer = true;
                _play = false;
                _count = 0;
                _turn++;
            }
        }
        else
        {
            Show($"Game Over! You scored {_turn}!", title);
            _isTimer = false;
            _play = false;
            _count = 0;
            _turn = 0;
            _timer.Stop();
        }
    }

    private void Set(Grid grid, int value, int period)
    {
        Button button = (Button)grid.Children.Single(s =>
            (int)((Button)s).Tag == value);
        button.Opacity = 0.25;
        DispatcherTimer opacity = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(period)
        };
        opacity.Tick += (object sender, object e) =>
        {
            button.Opacity = 1.0;
            opacity.Stop();
        };
        opacity.Start();
    }

    private void Tick(Grid grid)
    {
        if (_isTimer)
        {
            if (_count <= _turn)
            {
                Set(grid, _items[_count], light);
                _count++;
            }
            if (_count > _turn)
            {
                _isTimer = false;
                _play = true;
                _count = 0;
            }
        }
    }

    private void Add(Grid grid, int row, int column, int count)
    {
        Button button = new Button()
        {
            Tag = count,
            Width = 100,
            Height = 100,
            Content = Square(count),
            Margin = new Thickness(5)
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            if (_play)
            {
                int value = (int)((Button)sender).Tag;
                Set(grid, value, click);
                Score(value);
            }
        };
        button.SetValue(Grid.ColumnProperty, column);
        button.SetValue(Grid.RowProperty, row);
        grid.Children.Add(button);
    }

    private void Layout(ref Grid grid)
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        int count = 0;
        // Setup Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                Add(grid, row, column, count);
                count++;
            }
        }
    }

    public void New(Grid grid)
    {
        Layout(ref grid);
        _items = Choose(0, 3, level);
        _play = false;
        _turn = 0;
        _count = 0;
        _isTimer = true;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(speed)
        };
        _timer.Tick += (object sender, object e) =>
        {
            Tick(grid);
        };
        _timer.Start();
    }
}