using UnityEngine;

public class TokenSpawner : MonoBehaviour
{
    public GameObject tokenPrefab; // Assign the Token Prefab in the Inspector
    public Color[] tokenColors; // Array of colors for different sums
    public LayerMask diceLayerMask;  // Assign the "Dice" layer

    private DiceRoll[] dice;  // Reference to all dice in the scene

    void Start()
    {
        // Find all dice in the scene without sorting (faster)
        dice = FindObjectsByType<DiceRoll>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click to spawn a token
        {
            TrySpawnToken();
        }
    }

    void TrySpawnToken()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if right-click was on a dice
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, diceLayerMask))
        {
            SpawnTokenAtMousePosition();
        }
    }

    void SpawnTokenAtMousePosition()
    {
        // Calculate the sum of both dice
        int totalSum = 0;
        foreach (DiceRoll die in dice)
        {
            totalSum += die.GetComponent<DiceFaceDetector>().GetFaceUpValue();
        }

        // Spawn the token
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5f; // Adjust depth for world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        GameObject token = Instantiate(tokenPrefab, worldPosition, Quaternion.identity);

        // Assign color based on sum
        Renderer renderer = token.GetComponent<Renderer>();
        if (renderer != null && totalSum - 2 < tokenColors.Length) // Sum ranges from 2 to 12
        {
            renderer.material.color = tokenColors[totalSum - 2]; // Offset by 2 since sum starts at 2
        }

        // Assign an ID corresponding to the sum
        IDContainerBehaviour idContainer = token.GetComponent<IDContainerBehaviour>();
        if (idContainer != null)
        {
            idContainer.idObj = Resources.Load<ID>("CardNumberID/ID_" + totalSum); // Load matching ID from Resources folder
        }
    }
}
