using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float destroyDelay = 0.5f; // Állítsd be az animációd hosszára

    void Start()
    {
        // Megadott idő után törli magát a jelenetből
        Destroy(gameObject, destroyDelay);
    }
}