using UnityEngine;
using UnityEngine.UI;

public class WeaponPickup : MonoBehaviour
{
    public Transform[] inventorySlots; // Inventory slot transforms
    private GameObject[] currentWeapons; // Array to store equipped weapons
    public Image[] inventoryUI; // Array for UI images corresponding to inventory slots
    public Sprite emptySlotSprite; // Sprite for empty slots
    public Sprite defaultWeaponSprite; // Default sprite for weapons

    public float interactionDistance = 3f;
    private int selectedSlot = 0; // Currently selected inventory slot

    public Transform itemHoldPoint; // Assign the ItemHoldPoint in the Inspector

    void Start()
    {
        currentWeapons = new GameObject[inventorySlots.Length];
        UpdateInventoryUI(); // Initialize the UI
        UpdateSelectedWeapon(); // Initialize active weapon
    }

    void Update()
    {
        HandleInteraction();
        HandleSlotSwitch();
        HandleItemDrop(); // Handle dropping items
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Interact key
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                if (hit.collider.CompareTag("Weapon"))
                {
                    PickupWeapon(hit.collider.gameObject);
                }
            }
        }
    }

    private void PickupWeapon(GameObject weapon)
    {
        // Check if the currently selected slot is empty
        if (currentWeapons[selectedSlot] == null)
        {
            EquipWeapon(weapon, selectedSlot);
        }
        else
        {
            // Find the next available slot
            int nextAvailableSlot = -1;
            for (int i = 0; i < currentWeapons.Length; i++)
            {
                if (currentWeapons[i] == null)
                {
                    nextAvailableSlot = i;
                    break;
                }
            }

            if (nextAvailableSlot != -1)
            {
                EquipWeapon(weapon, nextAvailableSlot);
                Debug.Log($"Picked up {weapon.name} and placed it in slot {nextAvailableSlot + 1}.");
            }
            else
            {
                // Replace the item in the currently selected slot
                DropWeapon(selectedSlot);
                EquipWeapon(weapon, selectedSlot);
                Debug.Log($"Replaced the weapon in slot {selectedSlot + 1} with {weapon.name}.");
            }
        }

        UpdateInventoryUI();
    }



    private void EquipWeapon(GameObject weapon, int slotIndex)
    {
        // Add the weapon to the current slot
        currentWeapons[slotIndex] = weapon;

        // Reparent the weapon to the itemHoldPoint
        weapon.transform.SetParent(itemHoldPoint);

        // Reset position and rotation relative to the itemHoldPoint
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        // Disable the Rigidbody to prevent physics glitches
        Rigidbody weaponRigidbody = weapon.GetComponent<Rigidbody>();
        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = true;
        }

        // Activate the weapon
        weapon.SetActive(true);

        Debug.Log($"Equipped {weapon.name} in slot {slotIndex + 1}.");
    }



    private void HandleSlotSwitch()
    {
        int previousSlot = selectedSlot;

        // Use number keys to switch slots (1, 2, 3, etc.)
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedSlot = i;
                break;
            }
        }

        // Scroll wheel support for switching slots
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedSlot = (selectedSlot + 1) % inventorySlots.Length;
        }
        else if (scroll < 0f)
        {
            selectedSlot = (selectedSlot - 1 + inventorySlots.Length) % inventorySlots.Length;
        }

        if (previousSlot != selectedSlot)
        {
            UpdateSelectedWeapon();
            UpdateInventoryUI(); // Ensure the UI reflects the selected slot
        }
    }

    private void UpdateSelectedWeapon()
    {
        for (int i = 0; i < currentWeapons.Length; i++)
        {
            if (currentWeapons[i] != null)
            {
                currentWeapons[i].SetActive(i == selectedSlot);
            }
        }

        Debug.Log($"Switched to slot {selectedSlot + 1}");
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventoryUI.Length; i++)
        {
            if (currentWeapons[i] != null)
            {
                Weapon weaponScript = currentWeapons[i].GetComponent<Weapon>();
                inventoryUI[i].sprite = weaponScript != null ? weaponScript.icon : defaultWeaponSprite;
            }
            else
            {
                inventoryUI[i].sprite = emptySlotSprite;
            }

            // Highlight the selected slot
            inventoryUI[i].color = i == selectedSlot ? Color.yellow : Color.white;
        }
    }

    private void HandleItemDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Drop key
        {
            DropWeapon(selectedSlot);
        }
    }

    private void DropWeapon(int slotIndex)
    {
        if (currentWeapons[slotIndex] != null)
        {
            GameObject weaponToDrop = currentWeapons[slotIndex];

            // Remove the weapon from the inventory
            currentWeapons[slotIndex] = null;

            // Detach the weapon
            weaponToDrop.transform.SetParent(null);

            // Enable Rigidbody for physics
            Rigidbody weaponRigidbody = weaponToDrop.GetComponent<Rigidbody>();
            if (weaponRigidbody != null)
            {
                weaponRigidbody.isKinematic = false;
            }

            // Drop in front of the player
            Camera playerCamera = Camera.main;
            weaponToDrop.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 1.5f;

            // Apply push force
            Vector3 pushDirection = playerCamera.transform.forward.normalized;
            float pushForce = 300f;
            if (weaponRigidbody != null)
            {
                weaponRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }

            // Update inventory UI
            UpdateInventoryUI();
            UpdateSelectedWeapon();

            Debug.Log($"Dropped {weaponToDrop.name} from slot {slotIndex + 1}.");
        }
        else
        {
            Debug.Log("No weapon to drop in the selected slot.");
        }
    }

}



