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
}
