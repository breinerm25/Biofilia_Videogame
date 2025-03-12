using UnityEngine;
using UnityEngine.InputSystem;

public class CursorMovimiento : MonoBehaviour
{
    public RectTransform apuntadorJugador1; // 🎯 Cursor del Jugador 1
    public RectTransform apuntadorJugador2; // 🎯 Cursor del Jugador 2
    public RectTransform canvasRectTransform; // 📏 Referencia al Canvas
    public InputActionReference moveActionJugador1; // 🎮 Movimiento del Jugador 1
    public InputActionReference moveActionJugador2; // 🎮 Movimiento del Jugador 2
    public float pointerSpeed = 300f; // Velocidad del cursor

    private Vector2 canvasMin;
    private Vector2 canvasMax;

    void Start()
    {
        // Definir los límites del Canvas
        Vector2 canvasSize = canvasRectTransform.sizeDelta;
        canvasMin = -canvasSize / 2f; // Esquina inferior izquierda
        canvasMax = canvasSize / 2f;  // Esquina superior derecha
    }

    private void Update()
    {
        // Mover cursores sin cambiar su posición inicial
        MoverCursor(apuntadorJugador1, moveActionJugador1);
        MoverCursor(apuntadorJugador2, moveActionJugador2);
    }

    void MoverCursor(RectTransform cursor, InputActionReference moveAction)
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        cursor.anchoredPosition += new Vector2(moveInput.x, moveInput.y) * pointerSpeed * Time.deltaTime;

        // Aplicar límites sin modificar la posición inicial
        cursor.anchoredPosition = new Vector2(
            Mathf.Clamp(cursor.anchoredPosition.x, canvasMin.x, canvasMax.x),
            Mathf.Clamp(cursor.anchoredPosition.y, canvasMin.y, canvasMax.y)
        );
    }
}
