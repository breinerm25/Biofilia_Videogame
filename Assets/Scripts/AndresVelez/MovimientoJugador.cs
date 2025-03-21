using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    public RectTransform apuntador;
    public RectTransform canvasRectTransform;
    public float pointerSpeed = 300f;

    private Vector2 moveInput;
    private Vector2 canvasMin;
    private Vector2 canvasMax;

    void Start()
    {
        canvasRectTransform = Referencias.Instance.canva;
        transform.SetParent(canvasRectTransform);
        Vector2 canvasSize = canvasRectTransform.sizeDelta;
        canvasMin = -canvasSize / 2f;
        canvasMax = canvasSize / 2f;
    }

    private void Update()
    {
        MoverCursor();
    }

    void MoverCursor()
    {
        apuntador.anchoredPosition += moveInput * pointerSpeed * Time.deltaTime;
        apuntador.anchoredPosition = new Vector2(
            Mathf.Clamp(apuntador.anchoredPosition.x, canvasMin.x, canvasMax.x),
            Mathf.Clamp(apuntador.anchoredPosition.y, canvasMin.y, canvasMax.y)
        );
    }

    // 🚀 Este método es llamado automáticamente por PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
