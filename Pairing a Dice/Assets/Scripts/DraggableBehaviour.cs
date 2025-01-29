using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DraggableBehaviour : MonoBehaviour
{
    private Vector3 offsetPosition;
    private Camera cam;
    private bool canDrag;

    public UnityEvent onDrag, onUp;
    public bool Draggable { get; set; } = true;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        if (!Draggable) return;

        onDrag.Invoke();

        // Raycasting to handle 3D object picking
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            offsetPosition = transform.position - hit.point;
        }
        else
        {
            offsetPosition = transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
        }

        StartCoroutine(Drag());
    }

    private IEnumerator Drag()
    {
        canDrag = true;

        while (canDrag)
        {
            yield return new WaitForFixedUpdate();

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.position = hit.point + offsetPosition;
            }
            else
            {
                transform.position = cam.ScreenToWorldPoint(Input.mousePosition) + offsetPosition;
            }
        }
    }

    private void OnMouseUp()
    {
        canDrag = false;
        if (Draggable)
        {
            onUp.Invoke();
        }
    }
}
