using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MouseClick : MonoBehaviour
{
    public UnityEvent mouseDownEvent, mouseUpEvent;
    public void OnMouseDown()
    {
        mouseDownEvent.Invoke();
    }

    private void OnMouseUp()
    {
        mouseUpEvent.Invoke();
    }
}
