using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorConvertTools
{
    public static int HexToDec(string hex)
    {
        int dec = System.Convert.ToInt32(hex, 16);
        return dec;
    }

    public static string DecToHex(int value)
    {
        string hex = value.ToString("X2");
        return hex;
    }

    public static string FloatNormalizedToHex(float value)
    {
        string hex = DecToHex(Mathf.RoundToInt(value * 255f));
        return hex;
    }

    public static float HexToFloatNormalized(string hex)
    {
        float floatNormalized = HexToDec(hex) / 255f;
        return floatNormalized;
    }

    public static Color GetColorFromString(string hexString)
    {
        float red = HexToFloatNormalized(hexString.Substring(0, 2));
        float green = HexToFloatNormalized(hexString.Substring(2, 2));
        float blue = HexToFloatNormalized(hexString.Substring(4, 2));
        float alpha = 1f;

        if (hexString.Length >= 8)
        {
            alpha = HexToFloatNormalized(hexString.Substring(6, 2));
        }

        return new Color(red, green, blue, alpha);
    }

    public static string GetStringFromColor(Color color, bool useAlpha = false)
    {
        string red = FloatNormalizedToHex(color.r);
        string green = FloatNormalizedToHex(color.g);
        string blue = FloatNormalizedToHex(color.b);

        if (!useAlpha)
        {
            return red + green + blue;
        }
        else
        {
            string alpha = FloatNormalizedToHex(color.a);
            return red + green + blue + alpha;
        }
    }
}
