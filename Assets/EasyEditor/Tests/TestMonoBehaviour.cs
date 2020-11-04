using System.Collections.Generic;
using UnityEngine;
using EasyEditor;
public class TestMonoBehaviour : MonoBehaviour
{
    [Button]
    void TestButton()
    {
        Debug.Log("TestButton");
    }

    [ReorderableList]
    public int[] intArray;

    [ReorderableList]
    public List<Color> colorList;

    private int backValue = 1;

    [ShowInInspector]
    public int propValue
    {
        set
        {
            if (value != backValue)
            {
                backValue = value;
                Debug.Log($"value changed to {value}");
            }
        }
        get { return backValue; }
    }
}
