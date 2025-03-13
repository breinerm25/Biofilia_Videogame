using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproducirSonido : MonoBehaviour
{
    public AudioSource sonido; // 🔊 Arrastra aquí el AudioSource en el Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (sonido != null && !sonido.isPlaying)
        {
            sonido.Play();
        }
    }
}
