using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string title = "Order Game";
    private const int size = 6;

    private DateTime _timer;
    private ObservableCollection<int> _list =
        new ObservableCollection<int>();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    private void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private List<int> Choose(int start, int total)
    {
        return Enumerable.Range(start, total)
        .OrderBy(r => _random.Next(start, total)).ToList();
    }

    private void Layout(GridView grid)
    {
        int index = 0;
        _list.Clear();
        grid.IsEnabled = true;
        grid.ItemsSource = null;
        _timer = DateTime.UtcNow;
        List<int> numbers = Choose(1, size * size);
        while (index < numbers.Count)
        {
            _list.Add(numbers[index]);
            index++;
        }
        grid.ItemsSource = _list;
    }

    public void New(GridView grid)
    {
        Layout(grid);
    }

    public void Order(GridView grid)
    {
        if (_list.OrderBy(o => o).SequenceEqual(_list))
        {
            TimeSpan duration = (DateTime.UtcNow - _timer).Duration();
            Show($"Completed in {duration:hh\\:mm\\:ss}", title);
            grid.IsEnabled = false;
        }
    }
}