using UnityEngine;

public class Animal : MonoBehaviour
{
    [TextArea(2, 4)]
    public string animalDescription; // Descripci�n del animal
    public int scoreValue = 100; // Puntos otorgados por la foto
}

