using UnityEngine;
using Cinemachine;

public class CambioDeCamara : MonoBehaviour
{
    private CinemachineVirtualCamera camaraAnterior; // ?? Cámara activa antes del cambio
    public CinemachineVirtualCamera nuevaCamara; // ?? Cámara a activar en el trigger

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ?? Solo cambia si el jugador entra
        {
            if (nuevaCamara != null)
            {
                camaraAnterior = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
                nuevaCamara.Priority = 20; // ?? Activa la nueva cámara con prioridad alta
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (camaraAnterior != null)
            {
                camaraAnterior.Priority = 20; // ?? Reactiva la cámara anterior
                nuevaCamara.Priority = 10; // ?? Baja la prioridad de la nueva cámara
            }
        }
    }
}
