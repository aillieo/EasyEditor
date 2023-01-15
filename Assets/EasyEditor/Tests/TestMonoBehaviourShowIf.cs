using System.Collections.Generic;
using UnityEngine;
using AillieoUtils.EasyEditor;
public class TestMonoBehaviourShowIf : MonoBehaviour
{
    public bool showA;
    [ShowInInspector]
    public bool showB { get; set; }

    public bool ShowC() { return true; }

    [ShowIf("showA")]
    public string textA;
    [ShowIf("showB")]
    public string textB;
    [ShowIf("ShowC")]
    public string textC;

    public int showD = 0;
    [ShowIf("showD", 1)]
    public string textD;

    public PrimitiveType showE = default;
    [ShowIf("showE", PrimitiveType.Sphere)]
    public string textE;
}
