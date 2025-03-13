using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpeedDollyCart : MonoBehaviour
{
    public CinemachineDollyCart dollyCart; // Asigna el Dolly Cart en el Inspector
    public float newSpeed = 5f; // Velocidad nueva cuando el player entra

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el Player tiene este tag
        {
            dollyCart.m_Speed = newSpeed;
        }
    }
}

