using UnityEngine;

public class FlamethrowerController : MonoBehaviour
{
    public ParticleSystem flameParticleSystem;
    public float cookTime = 3.0f; // Adjust as needed

    private float currentCookTime;
    private bool isCooking;
    private FoodScript currentFoodScript; // Added variable to keep track of the current food being cooked

    private void Update()
    {
        HandleFlamethrower();
    }

    private void HandleFlamethrower()
    {
        if (Input.GetMouseButton(0))
        {
            StartFlamethrower();
        }
        else
        {
            StopFlamethrower();
        }

        // Check if currently cooking and update the cook time
        if (isCooking)
        {
            currentCookTime += Time.deltaTime;

            if (currentCookTime >= cookTime)
            {
                isCooking = false;
                currentCookTime = 0f;

                // Set the cooked state for the current foodScript
                if (currentFoodScript != null)
                {
                    currentFoodScript.SetCookedState(true);
                    currentFoodScript = null; // Reset the variable after setting the state
                }
            }
        }
    }

    private void StartFlamethrower()
    {
        if (flameParticleSystem != null && !flameParticleSystem.isPlaying)
        {
            flameParticleSystem.Play();
        }

        // Raycast from the flamethrower to detect objects
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red, 1.0f);

            FoodScript foodScript = hit.collider.GetComponent<FoodScript>();
            if (foodScript != null && !foodScript.IsCooked() && !isCooking)
            {
                Debug.Log("flamethrower raycast is hitting food object with foodScript");
                // Start cooking the food only if it's not already cooking
                isCooking = true;
                currentFoodScript = foodScript; // Set the current foodScript being cooked
                foodScript.Cook();
            }
        }
    }

    private void StopFlamethrower()
    {
        if (flameParticleSystem != null && flameParticleSystem.isPlaying)
        {
            flameParticleSystem.Stop();
        }
    }
}
