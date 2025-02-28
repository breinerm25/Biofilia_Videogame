using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FlashEffect : MonoBehaviour
{
    public RectTransform highlight;      // Imagen del brillo
    public Transform flashOrigin;        // Posición de origen del flash
    public Image flashEffect;            // Imagen blanca de flash
    public AudioSource flashSound;       // Sonido de cámara

    [Header("Flash Configuración")]
    public float flashScale = 10f;
    public float flashDuration = 0.15f;
    public float fadeOutDuration = 0.3f;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(TriggerFlashEffect);
    }

    public void TriggerFlashEffect()
    {
        // Mueve el brillo al punto de origen del flash y lo hace crecer rápidamente
        highlight.position = flashOrigin.position;
        highlight.localScale = Vector3.one;
        highlight.DOScale(flashScale, flashDuration).SetEase(Ease.OutQuad);

        // Reproduce el sonido de la cámara
        flashSound.Play();

        // Activa el efecto de pantalla blanca y lo desvanece
        flashEffect.color = new Color(1, 1, 1, 1);
        flashEffect.DOFade(0, fadeOutDuration);
    }
}
