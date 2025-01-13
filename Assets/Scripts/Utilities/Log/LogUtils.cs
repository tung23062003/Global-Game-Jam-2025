using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Runtime.CompilerServices;

public static class LogUtils
{
    public static void Log(object message)
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
        {
            return;
        }

        if (message as string != null && string.IsNullOrEmpty(message as string))
        {
            Debug.Log(message);
            return;
        }
#if UNITY_EDITOR
        Debug.Log(message);
#else
        Debug.Log("[" + DateTime.Now.ToLongTimeString() + "] " + message);
#endif
    }

    public static void LogColor(object message, bool isFullColor = false)
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
        {
            return;
        }

        if (message as string != null && string.IsNullOrEmpty(message as string))
        {
            Debug.Log(message);
            return;
        }
        string strTime = "";
#if UNITY_EDITOR

#else
        strTime = "[" + DateTime.Now.ToLongTimeString() + "] ";
#endif
        if (message as string != null)
        {
            if (isFullColor)
            {
                Debug.Log(strTime + "<color=orange>" + message + "</color>");
                return;
            }
            else
            {
                int index = FindCharacter(message as string, ':');
                if (index != -1)
                {
                    string[] splitStrings = (message as string).Split(':');
                    string firstString = splitStrings[0];
                    string str = "";
                    for (int i = 1; i < splitStrings.Length; i++)
                    {
                        str += splitStrings[i];
                    }
                    Debug.Log(strTime + "<color=orange>" + firstString + ":</color>" + str);
                    return;
                }
            }
        }
        Debug.Log(strTime + message);
    }

    public static void LogError(object message)
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
        {
            return;
        }

#if UNITY_EDITOR
        Debug.LogError(message);
#else
        Debug.LogError("[" + DateTime.Now.ToLongTimeString() + "] " + message);
#endif
    }

    public static void LogWarning(object message)
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
        {
            return;
        }

#if UNITY_EDITOR
        Debug.LogWarning(message);
#else
        Debug.LogWarning("[" + DateTime.Now.ToLongTimeString() + "] " + message);
#endif
    }

    public static void LogFormat(string message, params object[] args)
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
        {
            return;
        }

#if UNITY_EDITOR
        Debug.LogFormat(message, args);
#else
        Debug.LogFormat("[" + DateTime.Now.ToLongTimeString() + "] " + message, args);
#endif
    }

    public static void LogObject(object data)
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
        {
            return;
        }

        LogColor("Log data of " + data, true);
        foreach (var field in data.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var obj = field.GetValue(data);
            if (typeof(IEnumerable).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string))
            {
                if (obj is IDictionary)
                {
                    IDictionary dict = obj as IDictionary;
                    Log(field.Name + " : Dictionary Count " + dict.Count);
                    foreach (object key in dict.Keys)
                    {
                        Log("    " + key.ToString() + " : " + dict[key]);
                    }
                }
                else if (obj is IList)
                {
                    IList list = obj as IList;
                    Log(field.Name + " : List Count " + list.Count);
                    int index = 0;
                    foreach (object item in list)
                    {
                        Log("    " + index + " : " + item);
                        index++;
                    }
                }
            }
            else
            {
                Log(field.Name + " : " + obj);
            }
        }
    }

    private static int FindCharacter(string inputString, char character)
    {
        for (int i = 0; i < inputString.Length; i++)
        {
            if (inputString[i] == character)
            {
                return i;
            }
        }
        return -1;
    }

    private static readonly Dictionary<Type, DumpValue> DicMessagesIgnore = new()
    {
    };

    //public static void DumpToConsole(object obj, string str = "", bool isIgnore = true, bool isPretty = true)
    //{
    //    if (!Application.isEditor && !Debug.isDebugBuild)
    //    {
    //        return;
    //    }
    //    if (obj == null)
    //    {
    //        Log(str + " null");
    //        return;
    //    }
    //    if (!isIgnore || !DicMessagesIgnore.ContainsKey(obj.GetType()))
    //    {
    //        var strFormat = (isPretty && obj.GetType() == typeof(string)) ? JsonConvert.SerializeObject(JsonConvert.DeserializeObject((string)obj), Formatting.Indented) : Dumper.Dump(obj);
    //        string output = str + strFormat;
    //        Log(output);
    //    }
    //    else
    //    {
    //        var dumpValue = DicMessagesIgnore[obj.GetType()];
    //        if (dumpValue.totalSkip < 0)
    //        {
    //            return;
    //        }
    //        if (dumpValue.currentSkip >= dumpValue.totalSkip)
    //        {
    //            var strFormat = (isPretty && obj.GetType() == typeof(string)) ? JsonConvert.SerializeObject(JsonConvert.DeserializeObject((string)obj), Formatting.Indented) : Dumper.Dump(obj);
    //            string output = "(Skip " + dumpValue.totalSkip + " times) " + str + strFormat;
    //            Log(output);

    //            dumpValue.currentSkip = 0;
    //        }
    //        else
    //        {
    //            dumpValue.currentSkip++;
    //        }
    //    }
    //}

    public static void LogCaller([CallerLineNumber] int line = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
    {
        // Can replace UnityEngine.Debug.Log with any logging API you want
        Log($"{line} :: {memberName} :: {filePath}");
    }
}

class DumpValue
{
    public int currentSkip;
    public int totalSkip;
}
