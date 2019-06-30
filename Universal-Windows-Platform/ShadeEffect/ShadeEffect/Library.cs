using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private SpriteVisual _visual;

    public void Accept(ref Path path, ref Border border)
    {
        Compositor compositor = ElementCompositionPreview
        .GetElementVisual(path).Compositor;
        _visual = compositor.CreateSpriteVisual();
        _visual.Size = new Vector2((float)path.ActualWidth,
        (float)path.ActualHeight);
        DropShadow shadow = compositor.CreateDropShadow();
        shadow.Offset = new Vector3(10, 10, 0);
        shadow.Mask = path.GetAlphaMask();
        shadow.Color = Colors.Black;
        _visual.Shadow = shadow;
        ElementCompositionPreview.SetElementChildVisual(border, _visual);
    }

    public void Clear()
    {
        _visual.Shadow = null;
    }
}