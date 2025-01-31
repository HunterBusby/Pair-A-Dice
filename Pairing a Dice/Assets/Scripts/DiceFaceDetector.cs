using UnityEngine;

public class DiceFaceDetector : MonoBehaviour
{
    private Transform[] faces;  // Will be automatically populated
    private int currentFaceValue = 1;

    void Start()
    {
        FindFacesAutomatically();
    }

    void Update()
    {
        DetectFaceUp();
    }

    private void FindFacesAutomatically()
    {
        faces = new Transform[6];  // Initialize the array for six faces
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        // Search for child objects named "Face_1" to "Face_6"
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

        // Debugging: Check if any face is missing
        for (int i = 0; i < 6; i++)
        {
            if (faces[i] == null)
            {
                Debug.LogError("DiceFaceDetector: Face_" + (i + 1) + " not found on " + gameObject.name + "! Make sure each face is properly named.");
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

        // Find the highest face
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
        }
    }

    public int GetFaceUpValue()
    {
        return currentFaceValue;
    }

    
}
