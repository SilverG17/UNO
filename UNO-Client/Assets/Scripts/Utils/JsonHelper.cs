using System;
using UnityEngine;

public static class JsonHelper
{
    // ================= ARRAY SUPPORT =================

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    public static T[] FromJsonArray<T>(string json)
    {
        try
        {
            string wrapped = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
            return wrapper.array;
        }
        catch
        {
            Debug.LogError("FromJsonArray failed: " + json);
            return null;
        }
    }

    public static string ToJsonArray<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            array = array
        };

        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    // ================= LIST SUPPORT =================

    public static System.Collections.Generic.List<T> FromJsonList<T>(string json)
    {
        var array = FromJsonArray<T>(json);
        if (array == null) return null;

        return new System.Collections.Generic.List<T>(array);
    }

    public static string ToJsonList<T>(System.Collections.Generic.List<T> list, bool prettyPrint = false)
    {
        return ToJsonArray(list.ToArray(), prettyPrint);
    }
}