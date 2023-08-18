using DataStructure;
using NetworkFunctionality;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

/// <summary>
/// Static class containing multiple different useful methods, like converting the Color to its string RGBA counterpart
/// </summary>
public static class UtilitiesToolbox
{
    #region Color
    /// <summary>
    /// Method translating string, hexadecimal value to its integer, decimal counterpart
    /// </summary>
    /// <param name="hex">Hexadecimal number as string</param>
    /// <returns>Decimal counterpart of given number</returns>
    public static int HexToDec(string hex)
    {
        int dec = Convert.ToInt32(hex, 16);
        return dec;
    }

    /// <summary>
    /// Method translating received integer, decimal value to its string, hexadecimal counterpart
    /// </summary>
    /// <param name="value">Decimal value as integer</param>
    /// <returns>Hexadecimal value as string</returns>
    public static string DecToHex(int value)
    {
        string hex = value.ToString("X2");
        return hex;
    }

    /// <summary>
    /// Method converting the normalized float value to its hexadecimal, string counterpart.
    /// Is necessery since, the unity Color type objects use normalized float values in their notation (received by ToString() method).
    /// </summary>
    /// <param name="value">Float number between 0 and 1</param>
    /// <returns>Hexadecimal value as string</returns>
    public static string FloatNormalizedToHex(float value)
    {
        string hex = DecToHex(Mathf.RoundToInt(value * 255f));
        return hex;
    }

    /// <summary>
    /// Method converting the string hex value to its normalized float counterpart
    /// </summary>
    /// <param name="hex">Hexadecimal value saved in string</param>
    /// <returns>Float number between 0 and 1</returns>
    public static float HexToFloatNormalized(string hex)
    {
        float floatNormalized = HexToDec(hex) / 255f;
        return floatNormalized;
    }

    /// <summary>
    /// Method converting the set of hexadecimal values to Color object
    /// </summary>
    /// <param name="hexString">Up to 4 pairs of numbers, representing the RGBA values</param>
    /// <returns>Converted color object</returns>
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

    /// <summary>
    /// Method converting given Color to its hexadecimal representation
    /// </summary>
    /// <param name="color">Color you want to convert</param>
    /// <param name="useAlpha">Does generated code is supposed to have alpha value?</param>
    /// <returns></returns>
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
    #endregion

    #region Time
    /// <summary>
    /// Method converting given timeSpan to its timer(string) representation, such as "13:42"
    /// </summary>
    /// <param name="timeSpan">TimeSpan you want to draw values from</param>
    /// <returns>Timer (string) representation of time span</returns>
    public static string GetTimeAsString(TimeSpan timeSpan)
    {
        string minutesLeft = timeSpan.Minutes < 10 ? "0" + timeSpan.Minutes.ToString() : timeSpan.Minutes.ToString();
        string secondsLeft = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds.ToString() : timeSpan.Seconds.ToString();

        return minutesLeft + ":" + secondsLeft;
    }
    #endregion

    #region Network
    /// <summary>
    /// Method being called when there's is need to close chosen network connections
    /// </summary>
    /// <param name="logOutOfAuthentication">Do you want to log out of Unity Authentication Service?</param>
    /// <param name="deleteNetworkManager">Do you want to Shutdown and destroy NetworkManager?</param>
    /// <param name="deleteLobbyManager">Do you want to destroy LobbyManager?</param>
    /// <param name="deleteMultiplayerGameManager">Do you want to destroy MultiplayerGameManager?</param>
    public static void DeleteNetworkConnections(bool logOutOfAuthentication, bool deleteNetworkManager, bool deleteLobbyManager, bool deleteMultiplayerGameManager)
    {
        if (logOutOfAuthentication)
        {
            AuthenticationService.Instance.SignOut();
        }

        if (NetworkManager.Singleton != null && deleteNetworkManager)
        {
            NetworkManager.Singleton.Shutdown();
            UnityEngine.GameObject.Destroy(NetworkManager.Singleton.gameObject);
        }
        if (LobbyManager.instance != null && deleteLobbyManager)
        {
            UnityEngine.GameObject.Destroy(LobbyManager.instance.gameObject);
        }
        if (MultiplayerGameManager.instance != null && deleteMultiplayerGameManager)
        {
            UnityEngine.GameObject.Destroy(MultiplayerGameManager.instance.gameObject);
        }
    }
    #endregion

    #region Data collections
    /// <summary>
    /// Generic method, which copy the elements of given List to new Array of chosen type
    /// </summary>
    /// <typeparam name="T">List type</typeparam>
    /// <param name="list">List object, which elements you want to copy</param>
    /// <returns>New array containing the elements of given list</returns>
    public static T[] ListToArray<T>(List<T> list)
    {
        T[] array = new T[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i];
        }

        return array;
    }

    /// <summary>
    /// Generic method, which copy the elements of given Array to new List of chosen type
    /// </summary>
    /// <typeparam name="T">Array type</typeparam>
    /// <param name="array">Array object, which elements you want to copy</param>
    /// <returns>New list containing the elements of given array</returns>
    public static List<T> ArrayToList<T>(T[] array)
    {
        List<T> list = new List<T>();

        foreach (T item in array)
        {
            list.Add(item);
        }

        return list;
    }

    /// <summary>
    /// Method copying items of given Network List (PlayerGameData type) to new List (PlayerGameData type)
    /// </summary>
    /// <param name="networkList">Network List, which items you want to copy</param>
    /// <returns>New list containing the elements of given collection</returns>
    public static List<PlayerGameData> NetworkListPGDToListPGD(NetworkList<PlayerGameData> networkList)
    {
        List<PlayerGameData> list = new List<PlayerGameData>();

        foreach (PlayerGameData item in networkList)
        {
            list.Add(item);
        }

        return list;
    }
    #endregion
}
