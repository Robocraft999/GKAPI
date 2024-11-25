namespace GKAPI.Lang;

public class ColorHelper
{
    public static string WrapInColor(string text, Colors color)
    {
        return $"<color=#{(int)color:X}><b>{text}</b></color>";
    }
}