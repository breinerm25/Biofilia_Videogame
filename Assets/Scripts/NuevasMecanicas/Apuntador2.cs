using UnityEngine;

public class Apuntador2 : MonoBehaviour
{
    public RectTransform pointerImage; // Imagen del apuntador en UI
    public float velocidad = 500f; // Velocidad de movimiento ajustable desde el Inspector

    private Vector2 screenBounds;

    void Start()
    {
        ActualizarLimitesPantalla();
    }

    void Update()
    {
        // Actualiza límites en caso de cambio de resolución
        ActualizarLimitesPantalla();

        // Movimiento con WASD
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 inputMovimiento = new Vector3(moveX, moveY, 0);
        Vector3 newPosition = pointerImage.position + (inputMovimiento * velocidad * Time.deltaTime);

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
