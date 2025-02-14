using UnityEngine;

public class Ruta : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    [Header("Audio")]
    public AudioSource audioSource; // Un único AudioSource en el objeto
    public AudioClip startSound;  // Sonido al inicio
    public AudioClip endSound;    // Sonido al final

    void Start()
    {
        if (waypoints.Length == 0) return;

        // Posicionar el objeto en el primer waypoint
        transform.position = waypoints[0].position;

        // Reproducir sonido de inicio
        PlaySound(startSound);

        isMoving = true; // Iniciar el movimiento
    }

    void Update()
    {
        if (!isMoving || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                isMoving = false; // Se detiene al final

                // Reproducir sonido de finalización
                PlaySound(endSound);
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
