using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public string nombreEscena;
    public GameObject pausePanel; // Arrastra el Panel aquí en el Inspector

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // Pausa el juego
            Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
            Cursor.visible = true; // Hace visible el cursor
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el juego
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor al centro
            Cursor.visible = false; // Oculta el cursor
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(nombreEscena);
        Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad antes de salir
        
    }
}
