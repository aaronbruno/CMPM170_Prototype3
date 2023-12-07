using System.Collections;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    public bool isCooked = false;
    public GameObject cookedFoodGameObject;

    private void Start()
    {
        // Initialize based on the starting state (raw or cooked)
        SetCookedState(isCooked);
    }

    public void Cook()
    {
        if (!isCooked)
        {
            Debug.Log("Cooking the food!");
            // Play cooking animation or particle effect

            // Optionally, perform other actions like updating a score or triggering an event
        }
    }

    public void SetCookedState(bool cooked)
    {
        isCooked = cooked;

        if (cookedFoodGameObject != null)
        {
            Debug.Log("Setting cooked state");
            cookedFoodGameObject.SetActive(cooked);
        }

        gameObject.SetActive(!cooked); // Disable the raw version
    }

    public bool IsCooked()
    {
        return isCooked;
    }
}
