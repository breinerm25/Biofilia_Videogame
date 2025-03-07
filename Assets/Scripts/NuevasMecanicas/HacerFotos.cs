using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;

public class HacerFotos : MonoBehaviour
{
    public Camera photoCamera;
    public LayerMask animalLayer;
    public RectTransform apuntadorJugador1; // 🎯 Puntero del Jugador 1
    public RectTransform apuntadorJugador2; // 🎯 Puntero del Jugador 2
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI photosRemainingText; // 📷 UI para mostrar fotos restantes
    public AudioSource cameraSound;
    public Image previewImage; // 📸 Imagen en la UI (parte inferior izquierda)
    public CanvasGroup previewCanvasGroup; // 🎭 Para ocultar/mostrar la foto en UI
    public int photoSize = 300; // 📏 Tamaño del recorte de la foto
    public int maxPhotos = 7; // 📷 Cantidad inicial de fotos
    public float photoCooldown = 1.5f; // ⏳ Tiempo de espera entre fotos

    private int totalScore = 0;
    private int photosRemaining;
    private bool canTakePhoto = true;

    void Start()
    {
        photosRemaining = maxPhotos; // 📷 Inicializa con 7 fotos disponibles
        UpdateUI();
    }

    void Update()
    {
        if (canTakePhoto && photosRemaining > 0)
        {
            if (Input.GetMouseButtonDown(0)) // Jugador 1 (Click Izquierdo)
            {
                StartCoroutine(TakePhoto(apuntadorJugador1));
            }
            else if (Input.GetKeyDown(KeyCode.Space)) // Jugador 2 (Espacio)
            {
                StartCoroutine(TakePhoto(apuntadorJugador2));
            }
        }
    }

    IEnumerator TakePhoto(RectTransform apuntador)
    {
        canTakePhoto = false; // 🛑 Evita que se spameen fotos

        Vector3 screenPos = apuntador.position; // 📍 Posición del puntero en la pantalla
        Ray ray = photoCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, animalLayer))
        {
            Animal animal = hit.collider.GetComponent<Animal>();

            if (animal != null)
            {
                totalScore += animal.scoreValue;
                photosRemaining--; // 📸 Reducir el contador de fotos

                UpdateUI(); // 🔄 Actualizar puntaje y fotos restantes en UI

                if (animal.animalAudioSource != null)
                {
                    animal.animalAudioSource.Play();
                }

                if (cameraSound != null)
                {
                    cameraSound.Play();
                }

                StartCoroutine(CaptureScreenshot(animal, apuntador));
            }
        }

        yield return new WaitForSeconds(photoCooldown);
        canTakePhoto = true; // ✅ Permitir tomar otra foto después del cooldown
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntos: {totalScore}";
        }

        if (photosRemainingText != null)
        {
            photosRemainingText.text = $"Fotos restantes: {photosRemaining}";
        }
    }

    IEnumerator CaptureScreenshot(Animal animal, RectTransform apuntador)
    {
        yield return new WaitForEndOfFrame();

        // 🖼 Crear RenderTexture para capturar
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        photoCamera.targetTexture = rt;
        photoCamera.Render();

        // 🎯 Posición del puntero en coordenadas de pantalla
        Vector3 pointerPos = apuntador.position;
        int x = Mathf.Clamp((int)(pointerPos.x - (photoSize / 2)), 0, Screen.width - photoSize);
        int y = Mathf.Clamp((int)(pointerPos.y - (photoSize / 2)), 0, Screen.height - photoSize);

        // 📷 Capturar solo el área alrededor del apuntador
        Texture2D screenShot = new Texture2D(photoSize, photoSize, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(x, y, photoSize, photoSize), 0, 0);
        screenShot.Apply();

        // 📜 Guardar la imagen en la carpeta "Fotos" dentro de Assets
        string folderPath = Path.Combine(Application.dataPath, "Fotos");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"Foto_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
        File.WriteAllBytes(filePath, screenShot.EncodeToPNG());
        Debug.Log($"📸 Captura guardada en: {filePath}");

        // 🖼 Mostrar la foto en la UI
        MostrarFotoEnUI(screenShot, animal.duracion);

        // 🧹 Limpiar memoria
        photoCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
    }

    void MostrarFotoEnUI(Texture2D foto, float duracion)
    {
        previewImage.sprite = Sprite.Create(foto, new Rect(0, 0, foto.width, foto.height), new Vector2(0.5f, 0.5f));
        StartCoroutine(FotoEnUI(duracion));
    }

    IEnumerator FotoEnUI(float duracion)
    {
        previewCanvasGroup.alpha = 1; // 📸 Mostrar la foto
        yield return new WaitForSeconds(duracion);
        previewCanvasGroup.alpha = 0; // ❌ Ocultar después del tiempo
    }
}
