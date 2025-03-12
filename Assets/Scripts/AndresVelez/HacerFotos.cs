using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class HacerFotos : MonoBehaviour
{
    public int jugadorID = 1; // Se configura en el Inspector (Jugador 1 o Jugador 2)
    public Camera photoCamera;
    public LayerMask animalLayer;
    public RectTransform apuntador;
    public TextMeshProUGUI photosRemainingText;
    public InputActionReference shootAction;
    public AudioSource cameraSound;
    public static int totalScore = 0; // Puntaje compartido para ambos jugadores
    public TextMeshProUGUI scoreText;
    public Image previewImage; // Imagen para mostrar la foto tomada
    public CanvasGroup previewCanvasGroup;
    public int maxPhotos = 7;
    public float photoCooldown = 1.5f;
    public float rechargeTime = 5f;
    public float previewDuration = 2f; // Duración de la imagen en pantalla

    private int photosRemaining; // Independiente para cada jugador
    private bool canTakePhoto = true; // Independiente para cada jugador
    private bool isReloading = false; // Independiente para cada jugador

    void Start()
    {
        photosRemaining = maxPhotos; // Cada jugador empieza con su cantidad de fotos
        UpdateUI();
    }

    void Update()
    {
        if (shootAction.action.WasPressedThisFrame() && canTakePhoto && !isReloading)
        {
            if (photosRemaining > 0)
            {
                StartCoroutine(TakePhoto());
            }
            else
            {
                StartCoroutine(RecargarFotos());
            }
        }
    }

    IEnumerator TakePhoto()
    {
        canTakePhoto = false;
        photosRemaining--; // Solo se reduce para este jugador

        Animal animal = DetectarAnimal();
        if (animal != null)
        {
            totalScore += EvaluarPuntaje(animal);
        }

        cameraSound?.Play();
        yield return CaptureScreenshot(); // Captura la foto
        UpdateUI();
        yield return new WaitForSeconds(photoCooldown);
        canTakePhoto = true;
    }

    IEnumerator RecargarFotos()
    {
        if (isReloading) yield break;
        isReloading = true;
        canTakePhoto = false;

        yield return new WaitForSeconds(rechargeTime);

        photosRemaining = maxPhotos;
        UpdateUI();
        canTakePhoto = true;
        isReloading = false;
    }

    Animal DetectarAnimal()
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

    int EvaluarPuntaje(Animal animal)
    {
        float distancia = Vector3.Distance(apuntador.position, Camera.main.WorldToScreenPoint(animal.transform.position));
        if (distancia < 20f) return animal.scoreValue * 2;
        if (distancia < 50f) return animal.scoreValue;
        return 0;
    }

    void UpdateUI()
    {
        photosRemainingText.text = $"Jugador {jugadorID} - Fotos: {photosRemaining}";
        scoreText.text = $"Puntos: {totalScore}";
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

        MostrarFotoEnUI(screenShot);
    }

    void MostrarFotoEnUI(Texture2D foto)
    {
        previewImage.sprite = Sprite.Create(foto, new Rect(0, 0, foto.width, foto.height), new Vector2(0.5f, 0.5f));
        StartCoroutine(FotoEnUI());
    }

    IEnumerator FotoEnUI()
    {
        previewCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(previewDuration);
        previewCanvasGroup.alpha = 0;
    }
}
