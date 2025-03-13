using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento1Jugaddor : MonoBehaviour
{
    public RectTransform apuntador; // 🎯 Cursor del Jugador
    public RectTransform canvasRectTransform; // 📏 Referencia al Canvas
    public InputActionReference moveAction; // 🎮 Movimiento del Jugador
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
        // Mover cursor sin cambiar su posición inicial
        MoverCursor();
    }

    void MoverCursor()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        apuntador.anchoredPosition += new Vector2(moveInput.x, moveInput.y) * pointerSpeed * Time.deltaTime;

        // Aplicar límites sin modificar la posición inicial
        apuntador.anchoredPosition = new Vector2(
            Mathf.Clamp(apuntador.anchoredPosition.x, canvasMin.x, canvasMax.x),
            Mathf.Clamp(apuntador.anchoredPosition.y, canvasMin.y, canvasMax.y)
        );
    }
}
