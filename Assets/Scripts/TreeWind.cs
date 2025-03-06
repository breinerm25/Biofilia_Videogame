using UnityEngine;

public class TreeWind : MonoBehaviour
{
    public float minSwayAmount = 0.5f; // Mínimo ángulo de inclinación
    public float maxSwayAmount = 3f;   // Máximo ángulo de inclinación
    public float swaySpeed = 0.5f;     // Velocidad del viento

    private float swayAmount;  // Ángulo máximo de inclinación (aleatorio)
    private float randomOffset; // Variación única para cada árbol

    void Start()
    {
        swayAmount = Random.Range(minSwayAmount, maxSwayAmount); // Rango editable
        randomOffset = Random.Range(0f, 100f); // Variación por árbol
    }

    void Update()
    {
        float sway = (Mathf.PerlinNoise(Time.time * swaySpeed, randomOffset) - 0.5f) * 2f * swayAmount;
        transform.rotation = Quaternion.Euler(sway, 0, 0);
    }
}
