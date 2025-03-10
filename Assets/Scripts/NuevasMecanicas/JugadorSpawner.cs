using UnityEngine;
using UnityEngine.InputSystem;

public class JugadorSpawner : MonoBehaviour
{
    public GameObject prefabJugador1;
    public GameObject prefabJugador2;
    private int jugadoresConectados = 0;

    void Awake()
    {
        PlayerInputManager.instance.onPlayerJoined += AsignarPrefab;
    }

    void AsignarPrefab(PlayerInput nuevoJugador)
    {
        GameObject prefab = (jugadoresConectados == 0) ? prefabJugador1 : prefabJugador2;
        nuevoJugador.transform.SetParent(null);
        Instantiate(prefab, Vector3.zero, Quaternion.identity);
        jugadoresConectados++;
    }
}
