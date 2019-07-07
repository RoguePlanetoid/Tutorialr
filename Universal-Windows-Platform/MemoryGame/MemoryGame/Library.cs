using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string title = "Memory Game";
    private const int size = 4;

    private int _moves = 0;
    private int _firstId = 0;
    private int _secondId = 0;
    private Button _first;
    private Button _second;
    private int[,] _board = new int[size, size];
    private List<int> _matches = new List<int>();
    private Random _random = new Random((int)DateTime.Now.Ticks);
    private Dictionary<int, string> _moon = new Dictionary<int, string>()
    {
        { 1, "\U0001F311" }, // New
        { 2, "\U0001F312" }, // Waxing Crescent
        { 3, "\U0001F313" }, // First Quarter
        { 4, "\U0001F314" }, // Waxing Gibbous
        { 5, "\U0001F315" }, // Full
        { 6, "\U0001F316" }, // Waning Gibbous
        { 7, "\U0001F317" }, // Last Quarter
        { 8, "\U0001F318" }  // Waning Crescent
    };

    private void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private List<int> Choose(int start, int maximum, int total)
    {
        int number;
        List<int> numbers = new List<int>();
        while ((numbers.Count < total)) // Select Numbers
        {
            // Random Number between Start and Finish
            number = _random.Next(start, maximum + 1);
            if ((!numbers.Contains(number)) || (numbers.Count < 1))
            {
                numbers.Add(number); // Add if number Chosen or None
            }
        }
        return numbers;
    }

    private Viewbox Phase(int value)
    {
        TextBlock textblock = new TextBlock()
        {
            Text = _moon[value],
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

    private void Match()
    {
        _matches.Add(_firstId);
        _matches.Add(_secondId);
        if (_first != null)
        {
            _first.Background = null;
            _first = null;
        }
        if (_second != null)
        {
            _second.Background = null;
            _second = null;
        }
        if (_matches.Count == size * size)
        {
            Show($"Matched all moon phases in {_moves} moves!", title);
        }
    }

    private async void NoMatch()
    {
        await Task.Delay(TimeSpan.FromSeconds(1.5));
        if (_first != null)
        {
            _first.Content = null;
            _first = null;
        }
        if (_second != null)
        {
            _second.Content = null;
            _second = null;
        };
    }

    private void Compare()
    {
        if (_firstId == _secondId)
            Match();
        else
            NoMatch();
        _moves++;
        _firstId = 0;
        _secondId = 0;
    }

    private void Add(ref Grid grid, int row, int column)
    {
        Button button = new Button()
        {
            Width = 75,
            Height = 75,
            Margin = new Thickness(10),
            Style = (Style)Application.Current.Resources
            ["ButtonRevealStyle"]
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            int selected;
            button = (Button)(sender);
            row = (int)button.GetValue(Grid.RowProperty);
            column = (int)button.GetValue(Grid.ColumnProperty);
            selected = _board[row, column];
            if ((_matches.IndexOf(selected) < 0))
            {
                if (_firstId == 0) // No Match
                {
                    _first = button;
                    _firstId = selected;
                    _first.Content = Phase(selected);
                }
                else if (_secondId == 0)
                {
                    _second = button;
                    if (!_first.Equals(_second)) // Different
                    {
                        _secondId = selected;
                        _second.Content = Phase(selected);
                        Compare();
                    }
                }
            }
        };
        button.SetValue(Grid.ColumnProperty, column);
        button.SetValue(Grid.RowProperty, row);
        grid.Children.Add(button);
    }

    private void Layout(ref Grid grid)
    {
        _moves = 0;
        _matches.Clear();
        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        // Setup Board
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(ref grid, row, column);
            }
        }
    }

    public void New(Grid grid)
    {
        Layout(ref grid);
        int counter = 0;
        List<int> values = new List<int>();
        // Pairs : Random 1 - 8
        while (values.Count <= size * size)
        {
            List<int> numbers = Choose(1, size * 2, size * 2);
            for (int number = 0; number < size * 2; number++)
            {
                values.Add(numbers[number]);
            }
        }
        // Board : Random 1 - 16
        List<int> indices = Choose(1, size * size, size * size);
        // Setup Board
        for (int column = 0; column < size; column++)
        {
            for (int row = 0; row < size; row++)
            {
                _board[column, row] = values[indices[counter] - 1];
                counter++;
            }
        }
    }
}
