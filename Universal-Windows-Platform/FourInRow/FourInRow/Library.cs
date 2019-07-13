using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string title = "Four In Row";
    private const string yellow = "\U0001F7E1";
    private const string red = "\U0001F534";
    private const int total = 3;
    private const int size = 7;
    private readonly string[] _players =
    {
        string.Empty, "Yellow", "Red"
    };

    private int _value = 0;
    private int _amend = 0;
    private int _player = 0;
    private bool _won = false;
    private int[,] _board = new int[size, size];

    private void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private async Task<bool> ConfirmAsync(string content, string title,
        string ok, string cancel)
    {
        bool result = false;
        MessageDialog dialog = new MessageDialog(content, title);
        dialog.Commands.Add(new UICommand(ok,
            new UICommandInvokedHandler((cmd) => result = true)));
        dialog.Commands.Add(new UICommand(cancel,
            new UICommandInvokedHandler((cmd) => result = false)));
        await dialog.ShowAsync(); return result;
    }

    private bool CheckVertical(int row, int column)
    {
        _value = 0;
        do
        {
            _value++;
        }
        while (row + _value < size &&
            _board[column, row + _value] == _player);
        if (_value > total)
        {
            return true;
        }
        return false;
    }

    private bool CheckHorizontal(int row, int column)
    {
        _value = 0;
        _amend = 0;
        // From Left
        do
        {
            _value++;
        }
        while (column - _value >= 0 &&
            _board[column - _value, row] == _player);
        if (_value > total)
        {
            return true;
        }
        // Deduct Middle - Prevent double count
        _value -= 1;
        // Then Right
        do
        {
            _value++;
            _amend++;
        }
        while (column + _amend < size &&
            _board[column + _amend, row] == _player);
        if (_value > total)
        {
            return true;
        }
        return false;
    }

    private bool CheckDiagonalTopLeft(int row, int column)
    {
        _value = 0;
        _amend = 0;
        // From Top Left
        do
        {
            _value++;
        }
        while (column - _value >= 0 && row - _value >= 0 &&
            _board[column - _value, row - _value] == _player);
        if (_value > total)
        {
            return true;
        }
        // Deduct Middle - Prevent double count
        _value -= 1;
        // To Bottom Right
        do
        {
            _value++;
            _amend++;
        }
        while (column + _amend < size && row + _amend < size &&
            _board[column + _amend, row + _amend] == _player);
        if (_value > total)
        {
            return true;
        }
        return false;
    }

    private bool CheckDiagonalTopRight(int row, int column)
    {
        _value = 0;
        _amend = 0;
        // From Top Right
        do
        {
            _value++;
        }
        while (column + _value < size && row - _value >= 0 &&
            _board[column + _value, row - _value] == _player);
        if (_value > total)
        {
            return true;
        }
        // Deduct Middle - Prevent double count
        _value -= 1;
        // To Bottom Left
        do
        {
            _value++;
            _amend++;
        }
        while (column - _amend >= 0 &&
            row + _amend < size &&
            _board[column - _amend,
            row + _amend] == _player);
        if (_value > total)
        {
            return true;
        }
        return false;
    }

    private bool Winner(int row, int column)
    {
        bool vertical = CheckVertical(row, column);
        bool horizontal = CheckHorizontal(row, column);
        bool diagonalTopLeft = CheckDiagonalTopLeft(row, column);
        bool diagonalTopRight = CheckDiagonalTopRight(row, column);
        return vertical || horizontal ||
            diagonalTopLeft || diagonalTopRight;
    }

    private bool Full()
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (_board[column, row] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private Viewbox Piece(int player)
    {
        TextBlock textblock = new TextBlock()
        {
            IsColorFontEnabled = true,
            Text = player == 1 ? yellow : red,
            TextLineBounds = TextLineBounds.Tight,
            FontFamily = new FontFamily("Segoe UI Emoji"),
            HorizontalTextAlignment = TextAlignment.Center
        };
        return new Viewbox()
        {
            Child = textblock
        };
    }

    private void Set(Grid grid, int row, int column)
    {
        for (int i = size - 1; i > -1; i--)
        {
            if (_board[column, i] == 0)
            {
                _board[column, i] = _player;
                Button button = (Button)grid.Children.Single(
                    w => Grid.GetRow((Button)w) == i
                    && Grid.GetColumn((Button)w) == column);
                button.Content = Piece(_player);
                row = i;
                break;
            }
        }
        if (Winner(row, column))
        {
            _won = true;
            Show($"{_players[_player]} has won!", title);
        }
        else if (Full())
            Show("Board Full!", title);
        _player = _player == 1 ? 2 : 1; // Set Player
    }

    private void Add(Grid grid, int row, int column)
    {
        Button button = new Button()
        {
            Width = 100,
            Height = 100,
            Name = $"{row}:{column}",
            Margin = new Thickness(5),
            Style = (Style)Application.Current.Resources
                ["ButtonRevealStyle"]
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            if (!_won)
            {
                button = (Button)sender;
                row = (int)button.GetValue(Grid.RowProperty);
                column = (int)button.GetValue(Grid.ColumnProperty);
                if (_board[column, 0] == 0) // Check Free Row
                    Set(grid, row, column);
            }
            else
                Show("Game Over!", title);
        };
        button.SetValue(Grid.ColumnProperty, column);
        button.SetValue(Grid.RowProperty, row);
        grid.Children.Add(button);
    }

    private void Layout(ref Grid Grid)
    {
        Grid.Children.Clear();
        Grid.ColumnDefinitions.Clear();
        Grid.RowDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            Grid.RowDefinitions.Add(new RowDefinition());
            Grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        // Setup Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                Add(Grid, row, column);
                _board[row, column] = 0;
            }
        }
    }

    public async void New(Grid grid)
    {
        Layout(ref grid);
        _won = false;
        _player = await ConfirmAsync("Who goes First?", title,
            _players[1], _players[2]) ? 1 : 2;
    }
}