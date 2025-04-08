using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class TakePhotos : MonoBehaviour
{
    private static int nextID = 1;  // 🔹 Variable estática para asignar ID únicos

    public int jugadorID;  // 🔹 Ahora es de instancia, pero recibe un valor único
    public Camera photoCamera;
    public LayerMask animalLayer;
    public RectTransform apuntador;

    // 🔹 Variables que cambian por jugador
    public TextMeshProUGUI photosRemainingText;
    public Image previewImage;
    public CanvasGroup previewCanvasGroup;

    // 🔹 Variables compartidas
    public AudioSource cameraSound;
    public static int totalScore = 0;
    public TextMeshProUGUI scoreText;
    public Canvas uiCanvas;
    public TextMeshProUGUI playerNameText;  // 🔹 Referencia al texto que muestra "Player 1" o "Player 2"


    public int maxPhotos = 7;
    public float photoCooldown = 1.5f;
    public float rechargeTime = 1f;
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
    private void Awake()
    {
        jugadorID = nextID;
        nextID++;

        // 🔥 Buscar el texto automáticamente dentro del prefab
        playerNameText = transform.Find("Jugador1Texto").GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        Referencias.Instance.AsignarValores(this); // 🔹 Se asignan variables según el jugador

        photosRemaining = maxPhotos;
        UpdateUI();
        ActivarMira(MiraDefault);

        // 🚀 **Actualizar el nombre del jugador en pantalla**
        if (playerNameText != null)
        {
            playerNameText.text = "Player " + jugadorID;
        }
    }

    public static void ResetPlayer()
    {
        nextID = 1;
        totalScore = 0;
    }

    // 🚀 Método llamado desde PlayerInput
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && canTakePhoto)
        {
            if (photosRemaining > 0)
            {
                StartCoroutine(TakePhoto());
            }
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
            totalScore += animal.scoreValue;

            if (!animal.fotografiado)
            {
                animal.fotografiado = true;
                if (animal.animalAudioSource != null)
                {
                    animal.animalAudioSource.Play();
                }
            }

            ActivarMira(animal.epica ? MiraExcelente : MiraBuena);
        }

        cameraSound?.Play();
        yield return CaptureScreenshot();
        UpdateUI();

        // 🚀 **Verificar si se quedó sin fotos y recargar**
        if (photosRemaining <= 0)
        {
            StartCoroutine(RecargarFotos());  // 🔥 Iniciar recarga automáticamente
        }
        else
        {
            yield return new WaitForSeconds(photoCooldown);
            ActivarMira(MiraDefault);
            canTakePhoto = true;
        }
    }

    IEnumerator RecargarFotos()
    {
        isReloading = true;
        canTakePhoto = false;
        Debug.Log("Recargando fotos...");

        yield return new WaitForSecondsRealtime(rechargeTime);

        photosRemaining = maxPhotos;
        UpdateUI();
        Debug.Log("Recarga completa. Fotos restantes: " + photosRemaining);

        isReloading = false;
        canTakePhoto = true;  // 🔹 Solo aquí permitimos tomar fotos de nuevo
    }




    Animal DetectarAnimal()
    {
        Vector3 screenPos = apuntador.position;
        Ray ray = photoCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

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
    }

    IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();
        uiCanvas.enabled = false;
        yield return new WaitForEndOfFrame();

        Texture2D fullScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        fullScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        fullScreenshot.Apply();
        uiCanvas.enabled = true;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, apuntador.position);

        int x = Mathf.Clamp((int)screenPos.x - (fotoSize / 2), 0, Screen.width);
        int y = Mathf.Clamp((int)screenPos.y - (fotoSize / 2), 0, Screen.height);

        int safeWidth = Mathf.Min(fotoSize, Screen.width - x);
        int safeHeight = Mathf.Min(fotoSize, Screen.height - y);

        if (safeWidth <= 0 || safeHeight <= 0)
        {
            Destroy(fullScreenshot);
            yield break;
        }

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
