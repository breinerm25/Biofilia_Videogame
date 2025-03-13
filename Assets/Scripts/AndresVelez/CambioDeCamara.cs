using UnityEngine;
using Cinemachine;

public class CambioDeCamara : MonoBehaviour
{
    public CinemachineVirtualCamera cameraA;
    public CinemachineVirtualCamera cameraB;
    public int highPriority = 10;
    public int lowPriority = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si cameraA está activa, cambia a cameraB
            if (cameraA.Priority == highPriority)
            {
                cameraA.Priority = lowPriority;
                cameraB.Priority = highPriority;
            }
            else
            {
                cameraA.Priority = highPriority;
                cameraB.Priority = lowPriority;
            }
        }
    }
}
