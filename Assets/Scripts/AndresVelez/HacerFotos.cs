using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class HacerFotos : MonoBehaviour
{
    public int jugadorID = 1;
    public Camera photoCamera;
    public LayerMask animalLayer;
    public RectTransform apuntador;
    public TextMeshProUGUI photosRemainingText;
    public TextMeshProUGUI animalNameText; // Texto para mostrar el nombre del animal
    public InputActionReference shootAction;
    public AudioSource cameraSound;
    public static int totalScore = 0;
    public TextMeshProUGUI scoreText;
    public Image previewImage;
    public CanvasGroup previewCanvasGroup;
    public Canvas uiCanvas; // Referencia al Canvas de la UI
    public int maxPhotos = 7;
    public float photoCooldown = 1.5f;
    public float rechargeTime = 5f;
    public float previewDuration = 2f;

    private int photosRemaining;
    private bool canTakePhoto = true;
    private bool isReloading = false;
    private int fotoSize = 200;

    void Start()
    {
        photosRemaining = maxPhotos;
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
        photosRemaining--;

        Animal animal = DetectarAnimal();
        if (animal != null)
        {
            totalScore += EvaluarPuntaje(animal);

            if (!animal.fotografiado) // Solo reproduce el audio la primera vez
            {
                animal.fotografiado = true;
                animal.animalAudioSource?.Play();
            }

            MostrarNombreAnimal(animal.nombreAnimal);
        }

        cameraSound?.Play();
        yield return CaptureScreenshot();
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

        if (Physics.Raycast(ray, out hit, 20f, animalLayer))
        {
            return hit.collider.GetComponent<Animal>();
        }
        return null;
    }

    int EvaluarPuntaje(Animal animal)
    {
        return animal.scoreValue;
    }

    void UpdateUI()
    {
        photosRemainingText.text = $"Jugador {jugadorID} - Fotos: {photosRemaining}";
        scoreText.text = $"Puntos: {totalScore}";
    }

    IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        // Desactivar la UI antes de la captura
        uiCanvas.enabled = false;

        yield return new WaitForEndOfFrame(); // Esperar un frame para asegurar que la UI desaparezca

        // Capturar la pantalla sin la UI
        Texture2D fullScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        fullScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        fullScreenshot.Apply();

        // Reactivar la UI después de la captura
        uiCanvas.enabled = true;

        // Recortar el área de la mira
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, apuntador.position);
        int x = Mathf.Clamp((int)screenPos.x - (fotoSize / 2), 0, Screen.width - fotoSize);
        int y = Mathf.Clamp((int)screenPos.y - (fotoSize / 2), 0, Screen.height - fotoSize);

        Texture2D croppedScreenshot = new Texture2D(fotoSize, fotoSize);
        croppedScreenshot.SetPixels(fullScreenshot.GetPixels(x, y, fotoSize, fotoSize));
        croppedScreenshot.Apply();

        Destroy(fullScreenshot); // Liberar memoria
        MostrarFotoEnUI(croppedScreenshot);
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

    void MostrarNombreAnimal(string nombre)
    {
        animalNameText.text = nombre;
        StartCoroutine(OcultarNombreAnimal());
    }

    IEnumerator OcultarNombreAnimal()
    {
        yield return new WaitForSeconds(2f);
        animalNameText.text = "";
    }
}
