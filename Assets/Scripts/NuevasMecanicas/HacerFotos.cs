using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using System.IO;

public class HacerFotos : MonoBehaviour
{
    public Camera photoCamera;
    public LayerMask animalLayer;
    public RectTransform apuntadorJugador1;
    public RectTransform apuntadorJugador2;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI photosRemainingText;
    public AudioSource cameraSound;
    public Image previewImage;
    public CanvasGroup previewCanvasGroup;
    public int maxPhotos = 7;
    public float photoCooldown = 1.5f;
    public InputActionReference shootActionPlayer1;
    public InputActionReference shootActionPlayer2;

    private int totalScore = 0;
    private int photosRemaining;
    private bool canTakePhoto = true;
    private Animal objetivoJugador1;
    private Animal objetivoJugador2;

    void Start()
    {
        photosRemaining = maxPhotos;
        UpdateUI();
    }

    void Update()
    {
        objetivoJugador1 = DetectarAnimal(apuntadorJugador1);
        objetivoJugador2 = DetectarAnimal(apuntadorJugador2);

        if (canTakePhoto && photosRemaining > 0)
        {
            if (shootActionPlayer1.action.WasPressedThisFrame()) // Jugador 1
            {
                StartCoroutine(TakePhoto(apuntadorJugador1, objetivoJugador1));
            }
            else if (shootActionPlayer2.action.WasPressedThisFrame()) // Jugador 2
            {
                StartCoroutine(TakePhoto(apuntadorJugador2, objetivoJugador2));
            }
        }
    }

    Animal DetectarAnimal(RectTransform apuntador)
    {
        Vector3 screenPos = apuntador.position;
        Ray ray = photoCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, animalLayer))
        {
            return hit.collider.GetComponent<Animal>();
        }
        return null;
    }

    IEnumerator TakePhoto(RectTransform apuntador, Animal animal)
    {
        canTakePhoto = false;
        photosRemaining--;

        if (animal != null)
        {
            int puntos = EvaluarPuntaje(animal, apuntador);
            totalScore += puntos;
            Debug.Log($"Foto tomada: {animal.name}, Puntaje: {puntos}");
            if (animal.animalAudioSource != null)
            {
                animal.animalAudioSource.Play();
            }
        }
        else
        {
            Debug.Log("Foto fallida: No había objetivo válido.");
        }

        if (cameraSound != null)
        {
            cameraSound.Play();
        }

        yield return CaptureScreenshot();
        UpdateUI();
        yield return new WaitForSeconds(photoCooldown);
        canTakePhoto = true;
    }

    int EvaluarPuntaje(Animal animal, RectTransform apuntador)
    {
        float distancia = Vector3.Distance(apuntador.position, Camera.main.WorldToScreenPoint(animal.transform.position));
        if (distancia < 20f) return animal.scoreValue * 2; // Excelente
        if (distancia < 50f) return animal.scoreValue; // Bueno
        return 0; // Fallo
    }

    void UpdateUI()
    {
        scoreText.text = $"Puntos: {totalScore}";
        photosRemainingText.text = $"Fotos restantes: {photosRemaining}";
    }

    IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        photoCamera.targetTexture = rt;
        photoCamera.Render();

        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        photoCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        MostrarFotoEnUI(screenShot, 2f);
    }

    void MostrarFotoEnUI(Texture2D foto, float duracion)
    {
        previewImage.sprite = Sprite.Create(foto, new Rect(0, 0, foto.width, foto.height), new Vector2(0.5f, 0.5f));
        StartCoroutine(FotoEnUI(duracion));
    }

    IEnumerator FotoEnUI(float duracion)
    {
        previewCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(duracion);
        previewCanvasGroup.alpha = 0;
    }
}