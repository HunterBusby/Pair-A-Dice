using System;
using UnityEngine;
using UnityEngine.Events;

public class MouseEventsBehaviour : MonoBehaviour
{
    public UnityEvent mouseEnter, mouseExit, mouseClick, mouseHold;

    private void OnMouseEnter()
    {
        mouseEnter.Invoke();
    }

    private void OnMouseExit()
    {
        mouseExit.Invoke();
    }

    private void OnMouseDown()
    {
        mouseClick.Invoke();
    }

    private void OnMouseDrag()
    {
        mouseHold.Invoke();
    }
}
