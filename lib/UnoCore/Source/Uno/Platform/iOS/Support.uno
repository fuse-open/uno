using Uno;
using Uno.Graphics;
using Uno.Platform;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform.iOS
{
    [TargetSpecificType]
    public extern(IOS) struct uCGRect
    {
        public static uCGRect Zero { get { return extern<uCGRect>"CGRectZero"; } }
    }

    [TargetSpecificType]
    public extern(IOS) struct uCGSize
    {
    }

    [TargetSpecificType]
    public extern(IOS) struct uCGPoint
    {
    }

    [TargetSpecificType]
    public extern(IOS) struct UIImage
    {
    }


    [Require("Source.Include", "@{Uno.Platform.iOS.Application:Include}")]
    [Require("Source.Include", "Foundation/Foundation.h")]
    [Require("Source.Include", "CoreGraphics/CoreGraphics.h")]
    [Require("Source.Include", "UIKit/UIKit.h")]
    [Set("FileExtension", "mm")]
    extern(iOS)
    public static class Support
    {
        public static extern float2 CGPointToUnoFloat2(uCGPoint point, float scale)
        @{
            @{Uno.Float2} unoPoint;
            unoPoint.X = $0.x * $1;
            unoPoint.Y = $0.y * $1;
            return unoPoint;
        @}

        public static extern float2 CGSizeToUnoFloat2(uCGSize size, float scale)
        @{
            @{Uno.Float2} unoSize;
            unoSize.X = $0.width * $1;
            unoSize.Y = $0.height * $1;
            return unoSize;
        @}

        public static extern Rect CGRectToUnoRect(uCGRect rect, float scale)
        {
            var origin = CGPointToUnoFloat2(extern<uCGPoint>(rect)"$0.origin", scale);
            var size = CGSizeToUnoFloat2(extern<uCGSize>(rect)"$0.size", scale);
            return new Uno.Rect(origin, size);
        }

        public static extern uCGRect Pre_iOS8_HandleDeviceOrientation_Rect(uCGRect cgrect, ObjC.Object view)
        @{
            if (NSFoundationVersionNumber <= NSFoundationVersionNumber_iOS_7_1
                && @{Application.IsLandscape():Call()}
                && (!$1 || @{Application.IsRootView(ObjC.Object):Call($1)}))
            {
                // Transpose rect
                return CGRectMake(
                    $0.origin.y, $0.origin.x,
                    $0.size.height, $0.size.width);
            }

            return $0;
        @}
    }
}
