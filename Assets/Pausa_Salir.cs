using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pausa_Salir : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;
    private InputAction pauseAction;

    void Awake()
    {
        // Cargar el InputAction desde el Input System directamente
        var inputActionAsset = new InputActionMap("UI");
        pauseAction = inputActionAsset.AddAction("Pause", binding: "<Gamepad>/start");

        // Suscribirse al evento de input
        pauseAction.performed += TogglePause;
        pauseAction.Enable();
    }

    void OnDestroy()
    {
        pauseAction.Disable();
        pauseAction.performed -= TogglePause;
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true; // Pausar todos los audios
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false; // Reanudar todos los audios
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}