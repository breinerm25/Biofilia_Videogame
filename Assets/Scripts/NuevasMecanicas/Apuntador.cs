using UnityEngine;

public class Apuntador : MonoBehaviour
{
    public RectTransform pointerImage; // 🎯 Imagen del apuntador en UI
    public float sensibilidad = 500f; // Sensibilidad ajustable desde el Inspector
    public int playerIndex = 0; // 0 para Jugador 1, 1 para Jugador 2

    private Vector2 screenBounds;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        ActualizarLimitesPantalla();
    }

    void Update()
    {

        ActualizarLimitesPantalla();

        // 🎮 Movimiento con el joystick **izquierdo**
        float moveX = Input.GetAxis("Joystick" + playerIndex + "LeftX");
        float moveY = Input.GetAxis("Joystick" + playerIndex + "LeftY");

        Vector3 inputMovimiento = new Vector3(moveX, -moveY, 0);
        Vector3 newPosition = pointerImage.position + (inputMovimiento * sensibilidad * Time.deltaTime);

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
