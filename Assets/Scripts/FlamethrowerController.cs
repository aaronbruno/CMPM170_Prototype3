using UnityEngine;

public class FlamethrowerController : MonoBehaviour
{
    public ParticleSystem flameParticleSystem;
    public float cookTime = 3.0f; // Adjust as needed
    public float burnIntensity = 0.1f; // Adjust as needed
    public int rayCount = 5; // Number of rays in the cone
    public float coneAngle = 30f; // Angle of the cone

    private float currentCookTime;
    private bool isCooking;
    private FoodScript currentFoodScript;

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

        if (isCooking)
        {
            currentCookTime += Time.deltaTime;

            if (currentCookTime >= cookTime)
            {
                isCooking = false;
                currentCookTime = 0f;

                if (currentFoodScript != null)
                {
                    currentFoodScript.SetCookedState(true);
                    currentFoodScript = null;
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

        float angleIncrement = coneAngle / (float)(rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, -coneAngle / 2f + i * angleIncrement, 0);
            Vector3 direction = rotation * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red, 1.0f);

                FoodScript foodScript = hit.collider.GetComponent<FoodScript>();
                if (foodScript != null && !foodScript.IsCooked() && !isCooking)
                {
                    isCooking = true;
                    currentFoodScript = foodScript;
                    foodScript.Cook();
                }

                if (hit.collider.CompareTag("Partner"))
                {
                    Renderer partnerRenderer = hit.collider.GetComponent<Renderer>();
                    if (partnerRenderer != null)
                    {
                        Color originalColor = partnerRenderer.material.color;
                        float newR = Mathf.Clamp01(originalColor.r - burnIntensity * Time.deltaTime);
                        float newG = Mathf.Clamp01(originalColor.g - burnIntensity * Time.deltaTime);
                        float newB = Mathf.Clamp01(originalColor.b - burnIntensity * Time.deltaTime);
                        Color newColor = new Color(newR, newG, newB, originalColor.a);
                        partnerRenderer.material.color = newColor;

                        Debug.Log("Material color changed successfully.");
                        Debug.Log("Original Color: " + originalColor);
                        Debug.Log("New Color: " + newColor);
                    }
                    else
                    {
                        Debug.LogError("Renderer component not found on the partner (capsule) object.");
                    }
                }
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
