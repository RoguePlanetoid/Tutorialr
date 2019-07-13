using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string title = "Hit or Miss";
    private const string miss = "\U0001F573";
    private const string hit = "\U0001F4A5";
    private const int score = 18;
    private const int size = 6;

    private int _go = 0;
    private int _hits = 0;
    private int _misses = 0;
    private bool _won = false;
    private string[,] _board = new string[size, size];
    private Random _random = new Random((int)DateTime.Now.Ticks);

    private void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private List<int> Choose(int minimum, int maximum, int total)
    {
        int number;
        List<int> numbers = new List<int>();
        while (numbers.Count < total) // Select Numbers
        {
            number = _random.Next(minimum, maximum + 1);
            if (!numbers.Contains(number) || numbers.Count < 1)
            {
                numbers.Add(number); // Add if not Chosen or None
            }
        }
        return numbers;
    }

    private Viewbox Piece(string value)
    {
        TextBlock textblock = new TextBlock()
        {
            Text = value,
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

    private void Add(ref Grid grid, int row, int column)
    {
        Button button = new Button()
        {
            Width = 50,
            Height = 50,
            Margin = new Thickness(5),
            Style = (Style)Application.Current.Resources
                ["ButtonRevealStyle"]
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            if (!_won)
            {
                button = (Button)(sender);
                string selected = _board[(int)button.GetValue(Grid.RowProperty),
                    (int)button.GetValue(Grid.ColumnProperty)];
                if (button.Content == null)
                {
                    button.Content = (Piece(selected));
                    if (selected == hit)
                        _hits++;
                    else if (selected == miss)
                        _misses++;
                    _go++;
                }
                if (_go < (size * size) && _misses < score)
                {
                    if (_hits == score)
                    {
                        Show($"You Won! With {_hits} hits and {_misses} misses", 
                        title);
                        _won = true;
                    }
                }
                else
                {
                    Show($"You Lost! With {_hits} hits and {_misses} misses", 
                    title);
                    _won = true;
                }
            }
        };
        button.SetValue(Grid.ColumnProperty, column);
        button.SetValue(Grid.RowProperty, row);
        grid.Children.Add(button);
    }

    private void Layout(ref Grid grid)
    {
        _go = 0;
        _hits = 0;
        _misses = 0;
        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(ref grid, row, column);
            }
        }
    }

    public void New(ref Grid grid)
    {
        Layout(ref grid);
        _won = false;
        int index = 0;
        // Setup Values
        List<string> values = new List<string>();
        while (values.Count < (size * size))
        {
            values.Add(hit);
            values.Add(miss);
        }
        List<int> indices = Choose(1, (size * size), (size * size));
        // Setup Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                _board[column, row] = values[indices[index] - 1];
                index++;
            }
        }
    }
}
