using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Uno.Disasm
{
    static class Extensions
    {
        public static TContainer GetContainerAtPoint<TContainer>(this ItemsControl control, Point p) where TContainer : DependencyObject
        {
            var result = VisualTreeHelper.HitTest(control, p);
            var obj = result.VisualHit;

            while (VisualTreeHelper.GetParent(obj) != null && !(obj is TContainer))
                obj = VisualTreeHelper.GetParent(obj);

            return obj as TContainer;
        }
    }
}