using UnityEngine;

public class EnergyBeam : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    [Header("Célpontok")]
    public Transform targetBoss; // Ide kell behúzni a sárkányt

    void Start()
    {
        // Megkeressük a Line Renderert ugyanazon az objektumon
        lineRenderer = GetComponent<LineRenderer>();
        
        // A kezdőpontot rögzítjük a kristály helyzetéhez
        lineRenderer.SetPosition(0, transform.position);
    }

    void Update()
    {
        if (targetBoss != null)
        {
            // A végpontot (1-es index) folyamatosan a sárkány pozíciójára állítjuk
            lineRenderer.SetPosition(1, targetBoss.position);
        }
    }
}
