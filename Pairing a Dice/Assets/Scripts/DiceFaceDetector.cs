using UnityEngine;

public class DiceFaceDetector : MonoBehaviour
{
    private Transform[] faces;
    private int currentFaceValue = 1;
    private Rigidbody rb;
    private bool isRolling = false;
    public bool hasStoppedRolling = false; // ✅ Flag to track rolling state

    [Header("Machine Connection")]
    public BrandonMachine brandonMachine; // 🔥 New: Connect this in Inspector

    private bool notifiedMachine = false; // 🔥 New: Prevent double notifications

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        FindFacesAutomatically();
    }

    void Update()
    {
        if (!isRolling && (rb.linearVelocity.magnitude > 0.1f || rb.angularVelocity.magnitude > 0.1f))
        {
            isRolling = true;
            hasStoppedRolling = false;
            notifiedMachine = false; // 🔥 Reset notification when dice starts rolling again
        }
        else if (isRolling && rb.linearVelocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
        {
            isRolling = false;
            hasStoppedRolling = true;
            DetectFaceUp(); // Detect face only once dice stop

            if (!notifiedMachine && brandonMachine != null)
            {
                brandonMachine.OnSingleDiceStopped(this);
                notifiedMachine = true; // 🔥 Mark as notified
            }
        }
    }

    private void FindFacesAutomatically()
    {
        faces = new Transform[6];
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        for (int i = 1; i <= 6; i++)
        {
            foreach (Transform child in allChildren)
            {
                if (child.name == "Face_" + i)
                {
                    faces[i - 1] = child;
                    break;
                }
            }
        }
    }

    void DetectFaceUp()
    {
        if (faces == null || faces.Length < 6)
        {
            Debug.LogError("DiceFaceDetector: Faces array is not properly set up.");
            return;
        }

        float highestY = float.MinValue;
        int bestFaceIndex = -1;

        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i] != null && faces[i].position.y > highestY)
            {
                highestY = faces[i].position.y;
                bestFaceIndex = i;
            }
        }

        if (bestFaceIndex != -1)
        {
            currentFaceValue = bestFaceIndex + 1;
            Debug.Log(gameObject.name + " final face-up value: " + currentFaceValue);
        }
    }

    public int GetFaceUpValue()
    {
        return currentFaceValue;
    }
}
