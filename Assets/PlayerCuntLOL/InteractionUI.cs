using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public Text interactPrompt; // Reference to the UI Text
    public string interactMessage = "Press E to pick up"; // Prompt message
    public float rayDistance = 3f; // Max distance for interaction

    private Camera playerCamera; // Reference to the player's camera

    void Start()
    {
        playerCamera = Camera.main; // Get the main camera
        interactPrompt.gameObject.SetActive(false); // Hide prompt initially
    }

    void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Weapon")) // Check if the object has the "Weapon" tag
            {
                interactPrompt.text = interactMessage;
                interactPrompt.gameObject.SetActive(true);

                // Optional: Check for interaction input
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Interacted with " + hit.collider.name);
                    // Add your weapon pickup logic here
                }
            }
            else
            {
                interactPrompt.gameObject.SetActive(false); // Hide prompt if not pointing at a weapon
            }
        }
        else
        {
            interactPrompt.gameObject.SetActive(false); // Hide prompt if no object is hit
        }
    }
}
