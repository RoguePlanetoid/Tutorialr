using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const short tracks = 1;
    private const short formatType = 1;
    private const short bitsPerSample = 16;
    private const int headerSize = 8;
    private const int formatChunkSize = 16;
    private const int samplesPerSecond = 44100;
    private const short frameSize =
        tracks * ((bitsPerSample + 7) / 8);
    private const int bytesPerSecond =
        samplesPerSecond * frameSize;
    private const int waveSize = 4;
    private const int riff = 0x46464952;
    private const int wave = 0x45564157;
    private const int data = 0x61746164;
    private const int format = 0x20746D66;
    private const int samples = 88200 * 4;
    private const int dataChunkSize =
        samples * frameSize;
    private const int fileSize =
        waveSize + headerSize + formatChunkSize +
        headerSize + dataChunkSize;
    private const string mime = "audio/wav";

    private readonly Dictionary<string, double>
        _notes = new Dictionary<string, double>()
    {
        { "C", 261.6 }, { "C#", 277.2 }, { "D", 293.7 },
        { "D#", 311.1 } , { "E", 329.6 }, { "F", 349.2 },
        { "F#", 370.0 }, { "G", 392.0 }, { "G#", 415.3 },
        { "A", 440.0 }, { "A#", 466.2 }, { "B", 493.9 }
    };
    private readonly MediaElement _playback = new MediaElement();
    private readonly Color _accent =
    (Color)Application.Current.Resources["SystemAccentColor"];

    private void Play(double note)
    {
        IRandomAccessStream stream = new InMemoryRandomAccessStream();
        BinaryWriter writer = new BinaryWriter(stream.AsStream());
        double frequency = note * 1.5;
        writer.Write(riff);
        writer.Write(fileSize);
        writer.Write(wave);
        writer.Write(format);
        writer.Write(formatChunkSize);
        writer.Write(formatType);
        writer.Write(tracks);
        writer.Write(samplesPerSecond);
        writer.Write(bytesPerSecond);
        writer.Write(frameSize);
        writer.Write(bitsPerSample);
        writer.Write(data);
        writer.Write(dataChunkSize);
        for (int index = 0; index < samples / 4; index++)
        {
            double time = index / (double)samplesPerSecond;
            short sample = (short)(10000 *
                Math.Sin(time * frequency * 2.0 * Math.PI));
            writer.Write(sample);
        }
        stream.Seek(0);
        _playback.SetSource(stream, mime);
        _playback.Play();
    }

    private void Add(Grid grid, int column)
    {
        Button button = new Button()
        {
            Width = 20,
            Height = 80,
            FontSize = 10,
            Margin = new Thickness(5),
            Padding = new Thickness(0),
            Content = _notes.Keys.ElementAt(column),
            Background = new SolidColorBrush(_accent),
            Foreground = new SolidColorBrush(Colors.White),
            Style = (Style)Application.Current.Resources
            ["ButtonRevealStyle"]
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            button = (Button)sender;
            int note = Grid.GetColumn(button);
            Play(_notes[_notes.Keys.ElementAt(note)]);
        };
        button.SetValue(Grid.ColumnProperty, column);
        grid.Children.Add(button);
    }

private void Layout(Grid Grid)
{
    Grid.Children.Clear();
    Grid.RowDefinitions.Clear();
    Grid.ColumnDefinitions.Clear();
    // Setup Grid
    for (int Column = 0; (Column < _notes.Count); Column++)
    {
        Grid.ColumnDefinitions.Add(new ColumnDefinition());
        Add(Grid, Column);
    }
}

    public void New(Grid grid)
    {
        Layout(grid);
    }
}