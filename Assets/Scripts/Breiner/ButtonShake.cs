using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonShake : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        transform.DOShakePosition(0.2f, 5f, 10, 90, false, true);
    }
}