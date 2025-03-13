using UnityEngine;
using DG.Tweening;

public class StartButtonAnim : MonoBehaviour
{
    [Header("Pop-Up Effect")]
    public float popUpDuration = 0.6f;
    public float initialScale = 0.3f; // Escala inicial perceptible
    public float finalScaleX = 1.6f;
    public float finalScaleY = 1.9f;

    [Header("Floating Effect")]
    public float floatDistance = 10f;
    public float floatDuration = 1.5f;

    private Vector3 startPosition;

    private void OnEnable()
    {
        ResetAnimation();
        AnimateButton();
    }

    void ResetAnimation()
    {
        transform.DOKill(true); // Mata cualquier animación anterior para evitar conflictos
        startPosition = transform.localPosition; // Guarda la posición inicial
        transform.localScale = Vector3.one * initialScale; // Reinicia la escala
    }

    void AnimateButton()
    {
        // Animación de crecimiento
        transform.DOScale(new Vector3(finalScaleX, finalScaleY, 1f), popUpDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(StartFloatingEffect);
    }

    void StartFloatingEffect()
    {
        transform.DOLocalMoveY(startPosition.y + floatDistance, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
