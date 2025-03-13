using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StartButtonAnim : MonoBehaviour
{
    [Header("Pop-Up Effect")]
    public float popUpDuration = 0.6f;
    public float popUpScaleX = 1.6f;
    public float popUpScaleY = 1.9f;

    [Header("Floating Effect")]
    public float floatDistance = 10f;
    public float floatDuration = 1.5f;

    private void Start()
    {
        transform.localScale = new Vector3(0, 0, 1); // Empieza invisible

        // 1. Escalar en X e Y por separado
        transform.DOScaleX(popUpScaleX, popUpDuration).SetEase(Ease.OutBack);
        transform.DOScaleY(popUpScaleY, popUpDuration).SetEase(Ease.OutBack)
            .OnComplete(StartFloatingEffect);
    }

    void StartFloatingEffect()
    {
        transform.DOLocalMoveY(transform.localPosition.y + floatDistance, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

}
