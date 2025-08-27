using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    public void ShowPlatforms()
    {
        foreach (var c in transform.GetComponentsInChildren<GameObject>())
            c.SetActive(true);
    }
    public void HidePlatforms()
    {
        foreach (var c in transform.GetComponentsInChildren<GameObject>())
            c.SetActive(false);
    }
}
