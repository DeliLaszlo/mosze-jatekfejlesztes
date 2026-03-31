using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public GameObject circlePrefab;
    public float repeatRate = 0.5f;
    public Vector3 fireScale = new Vector3(2f, 2f, 1f);

    // --- ÚJ VÁLTOZÓ AZ ELTOLÁSHOZ ---
    [Header("Pozíció Beállítások")]
    public Vector2 offset = new Vector2(0f, -1f); // Alapból 1 egységgel lejjebb (-Y)
    // --------------------------------

    void Start()
    {
        InvokeRepeating("SpawnEffect", 0f, repeatRate);
    }

    void SpawnEffect()
    {
        if (circlePrefab != null)
        {
            // Kiszámoljuk az új pozíciót: Karakter helye + az eltolás
            Vector3 spawnPosition = new Vector3(
                transform.position.x + offset.x, 
                transform.position.y + offset.y, 
                transform.position.z
            );

            GameObject instantiatedFire = Instantiate(circlePrefab, spawnPosition, Quaternion.identity);
            instantiatedFire.transform.localScale = fireScale;
        }
    }
}