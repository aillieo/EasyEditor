// -----------------------------------------------------------------------
// <copyright file="TestMonoBehaviourErrorIf.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using AillieoUtils.EasyEditor;
using UnityEngine;

public class TestMonoBehaviourErrorIf : MonoBehaviour
{
    public bool showA;

    [ShowInInspector]
    public bool showB { get; set; }

    public bool ShowC()
    {
        return true;
    }

    [ErrorIf("errorA", "showA")]
    public string textA;
    [ErrorIf("errorB", "showB")]
    public string textB;
    [ErrorIf("errorC", "ShowC")]
    public string textC;
}
