using UnityEngine;

public class Apuntador : MonoBehaviour
{
    public RectTransform pointerImage; // Imagen del apuntador en UI
    public float sensibilidad = 1.0f; // Sensibilidad ajustable desde el Inspector

    private Vector2 screenBounds;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined; // Evita que el cursor salga de la pantalla
        Cursor.visible = false; // Oculta el cursor
        ActualizarLimitesPantalla();
    }

    void Update()
    {
        // Actualiza límites en caso de cambio de resolución
        ActualizarLimitesPantalla();

        // Obtiene la posición del mouse en la pantalla
        Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        // Aplica la sensibilidad
        Vector3 newPosition = pointerImage.position + (mouseDelta * sensibilidad * 10f);

        // Limita el movimiento dentro de la pantalla
        newPosition.x = Mathf.Clamp(newPosition.x, 0, screenBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, screenBounds.y);

        // Asigna la nueva posición al apuntador
        pointerImage.position = newPosition;
    }

    void ActualizarLimitesPantalla()
    {
        screenBounds = new Vector2(Screen.width, Screen.height);
    }
}
