// -----------------------------------------------------------------------
// <copyright file="TestMonoBehaviour.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using AillieoUtils.EasyEditor;
using UnityEngine;

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
        get
        {
            return this.backValue;
        }

        set
        {
            if (value != this.backValue)
            {
                this.backValue = value;
                Debug.Log($"value changed to {value}");
            }
        }
    }

    [AssetPath]
    public string prefab;

    [FoldableGroup("Int")]
    public int inta;
    [FoldableGroup("String")]
    public string stra;
    [FoldableGroup("Int")]
    public int intb;
    [FoldableGroup("String")]
    public string strb;
    [FoldableGroup("Int")]
    public int intc;
    [FoldableGroup("String")]
    public string strc;
    [FoldableGroup("Int2")]
    public int intd;
    [FoldableGroup("String")]
    public string strd;
}
