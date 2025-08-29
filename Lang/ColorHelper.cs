namespace GKAPI.Lang;

public static class ColorHelper
{
    public static string WrapInColorHex(string text, int colorHex)
    {
        return $"<color=#{colorHex:X}><b>{text}</b></color>";
    }
    
    public static string WrapInColorRGB(string text, int r, int g, int b)
    {
        return $"<color=#{r:X2}{g:X2}{b:X2}><b>{text}</b></color>";
    }
}