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
    public Canvas uiCanvas; // Referencia al Canvas de la UI
    public int maxPhotos = 7;
    public float photoCooldown = 1.5f;
    public float rechargeTime = 5f;
    public float previewDuration = 2f;

    // Miras
    public GameObject MiraDefault;
    public GameObject MiraMala;
    public GameObject MiraBuena;
    public GameObject MiraExcelente;

    private int photosRemaining;
    private bool canTakePhoto = true;
    private bool isReloading = false;
    private int fotoSize = 200;

    void Start()
    {
        photosRemaining = maxPhotos;
        UpdateUI();
        ActivarMira(MiraDefault);
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
        ActivarMira(MiraMala);

        Animal animal = DetectarAnimal();
        if (animal != null)
        {
            totalScore += animal.scoreValue;
            animal.fotografiado = true;
            animal.animalAudioSource?.Play();
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

    void UpdateUI()
    {
        photosRemainingText.text = $"Jugador {jugadorID} - Fotos: {photosRemaining}";
        scoreText.text = $"Puntos: {totalScore}";
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
        int x = Mathf.Clamp((int)screenPos.x - (fotoSize / 2), 0, Screen.width - fotoSize);
        int y = Mathf.Clamp((int)screenPos.y - (fotoSize / 2), 0, Screen.height - fotoSize);

        Texture2D croppedScreenshot = new Texture2D(fotoSize, fotoSize);
        croppedScreenshot.SetPixels(fullScreenshot.GetPixels(x, y, fotoSize, fotoSize));
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
