using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string title = "Light Game";
    private const int on = 1;
    private const int off = 0;
    private const int size = 7;
    private readonly Color lightOn = Colors.Gold;
    private readonly Color lightOff = Colors.Black;

    private int _moves = 0;
    private bool _won = false;
    private int[,] _board = new int[size, size];

    private void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private void Toggle(Grid grid, int row, int column)
    {
        _board[row, column] = _board[row, column] == on ? off : on;
        Button element = (Button)grid.FindName($"{row}:{column}");
        element.Background = _board[row, column] == on ?
            new SolidColorBrush(lightOn) :
            new SolidColorBrush(lightOff);
    }

    private void Set(Grid grid, int row, int column)
    {
        Toggle(grid, row, column);
        if (row > 0)
        {
            Toggle(grid, row - 1, column); // Toggle Left
        }
        if (row < (size - 1))
        {
            Toggle(grid, row + 1, column); // Toggle Right
        }
        if (column > 0)
        {
            Toggle(grid, row, column - 1); // Toggle Above
        }
        if (column < (size - 1))
        {
            Toggle(grid, row, column + 1); // Toggle Below
        }
    }

    private bool Winner()
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (_board[column, row] == on)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void Select(Grid grid, Button button)
    {
        if (!_won)
        {
            int row = (int)button.GetValue(Grid.RowProperty);
            int column = (int)button.GetValue(Grid.ColumnProperty);
            Set(grid, row, column);
            _moves++;
            if (Winner())
            {
                Show($"Well Done! You won in {_moves} moves!", title);
                _won = true;
            }
        }
        else
        {
            Show($"Game Over!", title);
        }
    }

    private void Add(Grid grid, int row, int column)
    {
        Button button = new Button()
        {
            Width = 50,
            Height = 50,
            Name = $"{row}:{column}",
            Margin = new Thickness(2),
            Background = new SolidColorBrush(lightOn),
            Style = (Style)Application.Current.Resources
            ["ButtonRevealStyle"]
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            Select(grid, button);
        };
        button.SetValue(Grid.ColumnProperty, column);
        button.SetValue(Grid.RowProperty, row);
        grid.Children.Add(button);
    }

    private void Layout(Grid grid)
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        // Setup Board
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        // Setup Layout
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(grid, row, column);
            }
        }
    }

    public void New(Grid grid)
    {
        _moves = 0;
        _won = false;
        Layout(grid);
        // Reset Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                _board[column, row] = on;
            }
        }
    }
}