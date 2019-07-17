using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string title = "Cards Game";
    private const string one = "one";
    private const string two = "two";
    private const int clubs = 127184;
    private const int diamonds = 127168;
    private const int hearts = 127152;
    private const int spades = 127136;
    private const int maximum = 52;
    private const int amount = 13;
    private readonly Dictionary<int, int> _suits =
        new Dictionary<int, int>()
    {
        { 1, clubs },
        { 14, diamonds },
        { 28, hearts },
        { 40, spades },
    };
    private readonly List<int> _faces = new List<int>()
    {
       14, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14
    };

    private int _first, _second;
    private int _score, _counter;
    private int _cardOne, _cardTwo;
    private List<int> _deckOne = new List<int>();
    private List<int> _deckTwo = new List<int>();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Show(string content, string title)
    {
        _ = new MessageDialog(content, title).ShowAsync();
    }

    private List<int> Choose(int total)
    {
        return Enumerable.Range(1, total)
        .OrderBy(r => _random.Next(0, total)).ToList();
    }

    private Viewbox Face(int face, Color fill)
    {
        TextBlock textblock = new TextBlock()
        {
            IsColorFontEnabled = true,
            Text = char.ConvertFromUtf32(face),
            TextLineBounds = TextLineBounds.Tight,
            Foreground = new SolidColorBrush(fill),
            FontFamily = new FontFamily("Segoe UI Emoji"),
            HorizontalTextAlignment = TextAlignment.Center
        };
        return new Viewbox()
        {
            Margin = new Thickness(0, 0, 0, 2),
            Child = textblock
        };
    }

    private Viewbox Add(int value, Color? fill = null)
    {
        int index = value % amount;
        int suit = _suits.Where(w => value >= w.Key)
            .Select(s => s.Value).LastOrDefault();
        int face = spades;
        if (suit > 0)
        {
            fill = (suit == hearts || suit == diamonds) ?
                Colors.Red : Colors.Black;
            face = suit + _faces[index];
        }
        return Face(face, fill.Value);
    }

    private void Card(Grid grid, string name, int value, Color fill)
    {
        Grid card = new Grid()
        {
            Name = name,
            CornerRadius = new CornerRadius(5),
            Background = new SolidColorBrush(Colors.WhiteSmoke)
        };
        card.Children.Add(Add(value, fill));
        grid.Children.Clear();
        grid.Children.Add(card);
    }

    private void Set(Grid grid, string name, int value)
    {
        Grid card = (Grid)grid.FindName(name);
        card.Children.Clear();
        card.Children.Add(Add(value));
    }

    public void New(Grid deckOne, Grid deckTwo)
    {
        _score = 0;
        _counter = 0;
        _cardOne = 0;
        _cardTwo = 0;
        _deckOne = Choose(maximum);
        _deckTwo = Choose(maximum);
        Card(deckOne, one, 0, Colors.DarkRed);
        Card(deckTwo, two, 0, Colors.DarkBlue);
    }

    public void Play(Grid deckOne, Grid deckTwo)
    {
        if (_deckOne != null && _deckTwo != null)
        {
            if (_cardOne < maximum && _cardTwo < maximum)
            {
                _first = _deckOne[_cardOne];
                Set(deckOne, one, _first);
                _cardOne++;
                _second = _deckTwo[_cardTwo];
                Set(deckTwo, two, _second);
                _cardTwo++;
                // Ignore Suit or Face for Match
                if (_first % amount == _second % amount)
                {
                    _score++;
                    Show("Match!", title);
                }
                _counter++;
            }
            else
            {
                Show($"Game Over! Matched {_score} of {_counter}!", title);
            }
        }
    }
}