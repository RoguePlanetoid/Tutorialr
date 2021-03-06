﻿using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

public static class Library
{
    private const string animate_back = "AnimateBack";
    private const string animate_next = "AnimateNext";

    private static Windows.UI.Xaml.Media.Animation.
    ConnectedAnimation _animation;

    public static string Current { get; set; }

    public static void Back(ref ListView listview)
    {
        Rectangle rectangle = (Rectangle)listview.Items
        .SingleOrDefault(f => ((Rectangle)f).Tag.Equals(Current));
        _animation = ConnectedAnimationService.GetForCurrentView()
        .GetAnimation(animate_back);
        _animation?.TryStart(rectangle);
    }

    public static Brush Next(ref object selected)
    {
        Rectangle rectangle = (Rectangle)selected;
        Current = (string)rectangle.Tag;
        _animation = ConnectedAnimationService.GetForCurrentView()
        .PrepareToAnimate(animate_next, rectangle);
        return rectangle.Fill;
    }

    public static void From(ref Rectangle from)
    {
        _animation =
        ConnectedAnimationService.GetForCurrentView()
        .PrepareToAnimate(animate_back, from);
    }

    public static void Loaded(ref Rectangle rectangle)
    {
        _animation =
        ConnectedAnimationService.GetForCurrentView()
        .GetAnimation(animate_next);
        rectangle.Opacity = 1;
        _animation?.TryStart(rectangle);
    }
}