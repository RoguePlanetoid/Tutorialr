using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private PointLight _light;

    public void Play(ref Path path)
    {
        Compositor compositor = ElementCompositionPreview
        .GetElementVisual(path).Compositor;
        Visual visual =
        ElementCompositionPreview.GetElementVisual(path);
        _light = compositor.CreatePointLight();
        _light.Offset = new Vector3(-(float)path.ActualWidth * 2,
        (float)path.ActualHeight / 2, (float)path.ActualHeight);
        _light.CoordinateSpace = visual;
        _light.Color = Colors.White;
        _light.Targets.Add(visual);
        ScalarKeyFrameAnimation animation =
        compositor.CreateScalarKeyFrameAnimation();
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        animation.InsertKeyFrame(1, 2 * (float)path.ActualWidth);
        animation.Duration = TimeSpan.FromSeconds(5.0f);
        _light.StartAnimation("Offset.X", animation);
    }

    public void Stop()
    {
        if (_light != null)
        {
            _light.Targets.RemoveAll();
        }
    }
}