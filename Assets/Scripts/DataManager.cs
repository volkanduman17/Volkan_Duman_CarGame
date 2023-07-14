using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{

    private static string LevelKey = "LevelDataKey";
    private static int DefaultLevelValue = 0;

    public static int Level
    {
        get => PlayerPrefs.GetInt(LevelKey, DefaultLevelValue);
        set => PlayerPrefs.SetInt(LevelKey, value);
    }
}       