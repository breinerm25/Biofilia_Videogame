using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionwEffec : MonoBehaviour
{
    public RectTransform focusImage;     // Imagen que simula el foco
    public Image focusImageComponent;    // Componente de la imagen para manejar el alpha
    public Transform flashOrigin;        // Punto de origen del foco
    public AudioSource flashSound;       // Sonido del flash
    public string nextSceneName;         // Nombre de la escena a cargar
    public MonoBehaviour focusMovementScript; // Script que controla el movimiento del foco

    [Header("Animación de Enfoque")]
    public float focusScale = 10f;
    public float focusDuration = 0.5f;

    [Header("Efecto de Flash")]
    public float flashExpansionDuration = 0.15f;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(StartTransition);
    }

    public void StartTransition()
    {
        // Desactiva el script de movimiento del foco si está asignado
        if (focusMovementScript != null)
        {
            focusMovementScript.enabled = false;
        }

        // Posiciona la imagen del foco en el origen y la activa
        focusImage.position = flashOrigin.position;
        focusImage.localScale = Vector3.one * 0.5f;
        focusImage.gameObject.SetActive(true);

        // Expande la imagen del foco
        focusImage.DOScale(focusScale, focusDuration).SetEase(Ease.OutQuad)
            .OnComplete(() => TriggerFlash());
    }

    private void TriggerFlash()
    {
        // Reproduce el sonido del flash
        flashSound.Play();

        // Aumenta el alpha de la imagen a 1 de forma suave pero rápida
        if (focusImageComponent != null)
        {
            focusImageComponent.DOFade(1, flashExpansionDuration);
        }

        // Simula el flash haciendo que el foco crezca aún más
        focusImage.DOScale(focusScale * 1.5f, flashExpansionDuration).SetEase(Ease.OutQuad)
            .OnComplete(() => LoadNextScene());
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
