using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Camera playerCamera;  // C�mara del jugador
    public Camera photoCamera;   // C�mara fotogr�fica
    public float mouseSensitivity = 100f; // Sensibilidad del mouse
    public Image hudCamera;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f; // Velocidad del zoom
    public float minZoom = 20f;  // Zoom m�nimo
    public float maxZoom = 60f;  // Zoom m�ximo
    private float targetZoom;    // Valor objetivo del zoom

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup; // Referencia al CanvasGroup de la pantalla negra
    public float fadeDuration = 0.5f;   // Duraci�n del fade

    private bool isPhotoMode = false;
    private bool isTransitioning = false; // Para evitar spam de cambio de c�mara
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquear cursor
        Cursor.visible = false; // Ocultar cursor al iniciar el juego
        fadeCanvasGroup.alpha = 0; // Asegurar que el fade empieza invisible
        targetZoom = photoCamera.fieldOfView; // Iniciar con el zoom normal
        hudCamera.gameObject.SetActive(false); // HUD oculto al inicio

        // Activamos solo la c�mara del jugador al inicio
        playerCamera.gameObject.SetActive(true);
        photoCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        RotateView(); // Manejo de la c�mara con el mouse
        HandleZoom(); // Manejo del zoom

        if (Input.GetMouseButtonDown(1) && !isTransitioning) // Evitar spam de cambio de c�mara
        {
            StartCoroutine(FadeAndSwitchCamera());
        }
    }

    void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar rotaci�n vertical

        transform.Rotate(Vector3.up * mouseX); // Rotar la c�psula (jugador)
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        photoCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void HandleZoom()
    {
        if (isPhotoMode) // Solo permitir zoom en la c�mara fotogr�fica
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            photoCamera.fieldOfView = Mathf.Lerp(photoCamera.fieldOfView, targetZoom, Time.deltaTime * 10f);
          
        }
    }

    IEnumerator FadeAndSwitchCamera()
    {
        isTransitioning = true; // Bloquea el cambio de c�mara

        // Hacer Fade In (pantalla negra aparece)
        yield return StartCoroutine(Fade(1));

        // **Cambiar de c�mara activando/desactivando GameObjects**
        isPhotoMode = !isPhotoMode;
        playerCamera.gameObject.SetActive(!isPhotoMode);
        photoCamera.gameObject.SetActive(isPhotoMode);

        // Activar el HUD de la c�mara solo en modo foto
        hudCamera.gameObject.SetActive(isPhotoMode);

        // Hacer Fade Out (pantalla negra desaparece)
        yield return StartCoroutine(Fade(0));

        isTransitioning = false; // Permite cambiar de c�mara nuevamente
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
