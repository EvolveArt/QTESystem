using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMouseOver : MonoBehaviour {

    [HideInInspector]
    public bool mouseOver;

    public void mouseOvered(bool isOver)
    {
        mouseOver = isOver;
    }
}
