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
    public InputActionReference shootAction;
    public AudioSource cameraSound;
    public static int totalScore = 0;
    public TextMeshProUGUI scoreText;
    public Image previewImage;
    public CanvasGroup previewCanvasGroup;
    public Canvas uiCanvas;
    public int maxPhotos = 7;
    public float photoCooldown = 1.5f;
    public float rechargeTime = 1f; // Tiempo de recarga reducido
    public float previewDuration = 2f;
    public int fotoSize = 200;
    public float rangoRaycast = 10f;

    public GameObject MiraDefault;
    public GameObject MiraMala;
    public GameObject MiraBuena;
    public GameObject MiraExcelente;

    private int photosRemaining;
    private bool canTakePhoto = true;
    private bool isReloading = false;

    void Start()
    {
        photosRemaining = maxPhotos;
        UpdateUI();
        ActivarMira(MiraDefault);
    }

    void Update()
    {
        if (shootAction.action.WasPressedThisFrame() && canTakePhoto)
        {
            if (photosRemaining > 0)
            {
                StartCoroutine(TakePhoto());
            }
        }

        // 🚀 Si se acaban las fotos, iniciar la recarga automáticamente
        if (photosRemaining == 0 && !isReloading)
        {
            StartCoroutine(RecargarFotos());
        }
    }

    IEnumerator TakePhoto()
    {
        canTakePhoto = false;
        photosRemaining--;
        ActivarMira(MiraMala);

        Animal animal = DetectarAnimal();
        if (animal != null)
        {
            // 🔥 Sumar puntos siempre, sin importar si ya fue fotografiado antes
            totalScore += animal.scoreValue;

            // 🔊 Reproducir audio solo la primera vez que se le toma foto
            if (!animal.fotografiado)
            {
                animal.fotografiado = true;
                if (animal.animalAudioSource != null)
                {
                    animal.animalAudioSource.Play();
                }
            }

            // 🔥 Cambiar la mira cada vez que se le toma foto
            ActivarMira(animal.epica ? MiraExcelente : MiraBuena);
        }

        cameraSound?.Play();
        yield return CaptureScreenshot();
        UpdateUI();
        yield return new WaitForSeconds(photoCooldown);
        ActivarMira(MiraDefault);
        canTakePhoto = true;
    }


    IEnumerator RecargarFotos()
    {
        isReloading = true;
        canTakePhoto = false;
        Debug.Log("⚡ Recargando fotos...");

        yield return new WaitForSecondsRealtime(rechargeTime);

        photosRemaining = maxPhotos; // 🔥 AHORA SE ACTUALIZA BIEN
        UpdateUI();

        Debug.Log($"✅ Recarga completa. Fotos disponibles: {photosRemaining}");

        isReloading = false;
        canTakePhoto = true;
    }

    Animal DetectarAnimal()
    {
        Vector3 screenPos = apuntador.position;
        Ray ray = photoCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        // 🔥 Usar el rango configurable en el Inspector
        if (Physics.Raycast(ray, out hit, rangoRaycast, animalLayer))
        {
            return hit.collider.GetComponent<Animal>();
        }
        return null;
    }

    void UpdateUI()
    {
        photosRemainingText.text = $"{photosRemaining}";
        scoreText.text = $"{totalScore}";
        Debug.Log($"📸 UI Actualizada - Fotos restantes: {photosRemaining}");
    }

    IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();
        uiCanvas.enabled = false;
        yield return new WaitForEndOfFrame();

        // 📸 Tomar captura de pantalla completa
        Texture2D fullScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        fullScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        fullScreenshot.Apply();
        uiCanvas.enabled = true;

        // 🖼 Obtener la posición de la mira en pantalla
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, apuntador.position);

        // 📏 Limitar valores para evitar errores de acceso a memoria
        int x = Mathf.Clamp((int)screenPos.x - (fotoSize / 2), 0, Screen.width);
        int y = Mathf.Clamp((int)screenPos.y - (fotoSize / 2), 0, Screen.height);

        // 📌 Ajustar tamaño de recorte si estamos cerca de los bordes
        int safeWidth = Mathf.Min(fotoSize, Screen.width - x);
        int safeHeight = Mathf.Min(fotoSize, Screen.height - y);

        // 🛠 Verificar que el tamaño de recorte es válido
        if (safeWidth <= 0 || safeHeight <= 0)
        {
            Debug.LogWarning("No se pudo capturar la imagen: fuera de los límites de la pantalla.");
            Destroy(fullScreenshot);
            yield break;
        }

        // ✂️ Recortar la imagen sin errores
        Texture2D croppedScreenshot = new Texture2D(safeWidth, safeHeight);
        croppedScreenshot.SetPixels(fullScreenshot.GetPixels(x, y, safeWidth, safeHeight));
        croppedScreenshot.Apply();

        Destroy(fullScreenshot);
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

    void ActivarMira(GameObject mira)
    {
        MiraDefault.SetActive(false);
        MiraMala.SetActive(false);
        MiraBuena.SetActive(false);
        MiraExcelente.SetActive(false);
        mira.SetActive(true);
    }
}
