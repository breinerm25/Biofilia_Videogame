using UnityEngine;
using UnityEngine.InputSystem;

public class CursorMovimiento : MonoBehaviour
{
    public RectTransform apuntadorJugador1; // 🎯 Cursor del Jugador 1
    public RectTransform apuntadorJugador2; // 🎯 Cursor del Jugador 2
    public InputActionReference moveActionJugador1; // 🎮 Movimiento del Jugador 1
    public InputActionReference moveActionJugador2; // 🎮 Movimiento del Jugador 2
    public float pointerSpeed = 300f; // Velocidad del cursor

    private void Update()
    {
        Vector2 moveInput1 = moveActionJugador1.action.ReadValue<Vector2>();
        apuntadorJugador1.anchoredPosition += new Vector2(moveInput1.x, moveInput1.y) * pointerSpeed * Time.deltaTime;

        Vector2 moveInput2 = moveActionJugador2.action.ReadValue<Vector2>();
        apuntadorJugador2.anchoredPosition += new Vector2(moveInput2.x, moveInput2.y) * pointerSpeed * Time.deltaTime;
    }
}