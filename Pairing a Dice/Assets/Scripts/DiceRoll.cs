using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DiceRoll : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
    private float cameraZDistance;
    private Vector3 lastMousePosition;
    private Vector3 velocity;  // Mouse velocity for momentum

    private bool isBeingDragged = false;
    private bool isPickedUp = false;

    public float rotationSpeed = 10f;
    public float throwForce = 1f;  // Adjust to fine-tune the speed

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
    }

    void Update()
    {
        // If right-click is held, move the dice with the mouse without physics
        if (isPickedUp)
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraZDistance);
            Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            transform.position = newWorldPosition;
        }

        // Release right-click to drop the dice
        if (Input.GetMouseButtonUp(1) && isPickedUp)
        {
            isPickedUp = false;
            rb.isKinematic = false; // Re-enable physics
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(1)) // Right-click to pick up
        {
            isPickedUp = true;
            rb.isKinematic = true;  // Disable physics so it doesn't change value
        }
        else if (Input.GetMouseButton(0)) // Left-click to roll
        {
            isBeingDragged = true;
            lastMousePosition = Input.mousePosition;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnMouseDrag()
    {
        if (isBeingDragged)
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraZDistance);
            Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

            // Calculate velocity for momentum effect
            velocity = (Input.mousePosition - lastMousePosition) / Time.deltaTime;
            lastMousePosition = Input.mousePosition;

            // Move the dice with the cursor
            transform.position = newWorldPosition;

            // Apply rotational effect
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
            {
                Vector3 moveDirection = mainCamera.transform.right * mouseX + mainCamera.transform.up * mouseY;
                Vector3 torqueDirection = Vector3.Cross(Vector3.forward, moveDirection) * rotationSpeed;
                rb.AddTorque(torqueDirection, ForceMode.Acceleration);
            }
        }
    }

    private void OnMouseUp()
    {
        if (isBeingDragged)
        {
            isBeingDragged = false;

            // Convert mouse velocity to world space movement
            Vector3 worldVelocity = mainCamera.ScreenToWorldPoint(new Vector3(velocity.x, velocity.y, cameraZDistance)) 
                                    - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, cameraZDistance));

            rb.linearVelocity = worldVelocity * throwForce;  // Apply movement force

            // Ensure gravity is still applied
            rb.useGravity = true;
        }
    }

    public void Roll()
    {
        if (!isPickedUp)  // Prevent rolling if picked up
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 randomForce = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * throwForce;
            Vector3 randomTorque = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));

            rb.AddForce(randomForce, ForceMode.Impulse);
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }
}
