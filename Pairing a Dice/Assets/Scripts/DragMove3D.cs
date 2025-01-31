using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DragMove3D : MonoBehaviour
{
    private Camera mainCamera;
    private float CameraZDistance;
    private Rigidbody _rigidbody;

    public float rotationSpeed = 10;
    public float throwForce = 5f; // Adjust to control how far the dice moves after release

    private Vector3 lastMousePosition;
    private Vector3 velocity; // Stores velocity of drag movement

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        CameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
    }

    private void OnMouseDown()
    {
        lastMousePosition = Input.mousePosition;
        _rigidbody.linearVelocity = Vector3.zero;  // Reset movement
        _rigidbody.angularVelocity = Vector3.zero;  // Reset spin
    }

    private void OnMouseDrag()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraZDistance);
        Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        
        // Calculate movement velocity (for momentum)
        velocity = (Input.mousePosition - lastMousePosition) / Time.deltaTime;
        lastMousePosition = Input.mousePosition;

        // Move the dice
        transform.position = newWorldPosition;

        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f) // Prevent unnecessary torque
        {
            // Convert mouse movement to world space
            Vector3 moveDirection = mainCamera.transform.right * mouseX + mainCamera.transform.up * mouseY;

            // Apply torque in the direction of movement
            Vector3 torqueDirection = Vector3.Cross(Vector3.forward, moveDirection) * rotationSpeed;
            _rigidbody.AddTorque(torqueDirection, ForceMode.Acceleration);
        }
    }

    private void OnMouseUp()
    {
        // Apply momentum when released
        Vector3 worldVelocity = mainCamera.ScreenToWorldPoint(velocity) - mainCamera.ScreenToWorldPoint(Vector3.zero);
        _rigidbody.linearVelocity = worldVelocity * throwForce;  // Apply movement force

        // Keep existing angular velocity for spinning effect
    }
}
