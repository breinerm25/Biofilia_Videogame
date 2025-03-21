using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneTransitioSimple : MonoBehaviour
{
    public string nextSceneName;
    public float transitionDuration = 0.5f;
    public CanvasGroup fadeCanvas;
    private InputAction selectAction; // Acción para detectar el botón A

    private void Awake()
    {
        // Crear la acción para el botón A del Gamepad (buttonSouth)
        var inputActionAsset = new InputActionMap("UI");
        selectAction = inputActionAsset.AddAction("Select", binding: "<Gamepad>/buttonSouth"); // A en Xbox, X en PlayStation

        selectAction.performed += ctx => StartTransition();
        selectAction.Enable();
    }

    private void OnDestroy()
    {
        selectAction.Disable();
        selectAction.performed -= ctx => StartTransition();
    }

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(StartTransition);

        // Seleccionar el botón al inicio para que responda al Gamepad
        button.Select();
    }

    public void StartTransition()
    {
        Time.timeScale = 1f; // Restaurar el tiempo antes de la transición
        AudioListener.pause = false; // Asegurar que el sonido sigue

        if (fadeCanvas != null)
        {
            fadeCanvas.DOFade(1, transitionDuration).OnComplete(LoadNextScene);
        }
        else
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        DOTween.Clear(true);
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
