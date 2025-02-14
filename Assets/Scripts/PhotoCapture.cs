using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    public Camera photoCamera;
    public float detectionRange = 20f;

    [Header("Detection Settings")]
    public LayerMask animalLayer;

    [Header("UI Elements")]
    public Image captureEffect;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI animalDescriptionText; // Texto que muestra la descripción del animal

    [Header("Audio")]
    public AudioSource shutterSound; // Sonido del obturador de la cámara

    private int totalScore = 0;
    private bool isCapturing = false;

    void Start()
    {
        if (captureEffect) captureEffect.gameObject.SetActive(false);
        if (animalDescriptionText) animalDescriptionText.text = "";
        UpdateScoreUI();
    }

    void Update()
    {
        // Evita tomar fotos si el juego está pausado
        if (Time.timeScale == 0f) return;

        if (photoCamera.enabled && !isCapturing && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TakePhoto());
        }
    }

    IEnumerator TakePhoto()
    {
        if (!photoCamera.gameObject.activeInHierarchy)
            yield break;

        isCapturing = true;
        StartCoroutine(CaptureEffect());

        // Reproducir sonido de la cámara
        if (shutterSound != null)
        {
            shutterSound.Play();
        }

        List<Animal> detectedAnimals = DetectAnimals();

        if (detectedAnimals.Count > 0)
        {
            int photoScore = CalculatePhotoScore(detectedAnimals);
            totalScore += photoScore;
            UpdateScoreUI();

            // Reproducir sonido del primer animal detectado (si tiene un AudioSource)
            PlayAnimalSound(detectedAnimals[0]);

            ShowAnimalDescription(detectedAnimals[0]); // Mostrar descripción del animal
            yield return StartCoroutine(CaptureScreenshot(1920, 1080));
        }
        else
        {
            Debug.Log("No hay animales en la foto, no se guardará la imagen.");
        }

        yield return new WaitForSeconds(0.5f);
        isCapturing = false;
    }

    void PlayAnimalSound(Animal animal)
    {
        if (animal.animalAudioSource != null && !animal.animalAudioSource.isPlaying)
        {
            animal.animalAudioSource.Play();
        }
    }

    List<Animal> DetectAnimals()
    {
        List<Animal> detectedAnimals = new List<Animal>();
        Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(photoCamera);
        Collider[] animalsInRange = Physics.OverlapSphere(photoCamera.transform.position, detectionRange, animalLayer);

        foreach (Collider animal in animalsInRange)
        {
            if (GeometryUtility.TestPlanesAABB(cameraFrustum, animal.bounds))
            {
                if (!IsBlocked(animal.transform))
                {
                    Animal animalScript = animal.GetComponent<Animal>();
                    if (animalScript != null)
                    {
                        detectedAnimals.Add(animalScript);
                    }
                }
            }
        }

        return detectedAnimals;
    }

    bool IsBlocked(Transform target)
    {
        Vector3 direction = target.position - photoCamera.transform.position;
        Ray ray = new Ray(photoCamera.transform.position, direction);
        RaycastHit hit;
        float distanceToTarget = Vector3.Distance(photoCamera.transform.position, target.position);

        if (Physics.Raycast(ray, out hit, distanceToTarget))
        {
            if (hit.transform != target)
            {
                return true;
            }
        }
        return false;
    }

    int CalculatePhotoScore(List<Animal> animals)
    {
        int score = 0;
        foreach (Animal animal in animals)
        {
            score += animal.scoreValue;
        }
        return score;
    }

    IEnumerator CaptureEffect()
    {
        if (captureEffect)
        {
            captureEffect.gameObject.SetActive(true);
            CanvasGroup canvasGroup = captureEffect.GetComponent<CanvasGroup>();
            if (canvasGroup == null) yield break;

            float duration = 0.2f;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, t / duration);
                yield return null;
            }
            canvasGroup.alpha = 1;
            yield return new WaitForSeconds(0.1f);

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, t / duration);
                yield return null;
            }
            canvasGroup.alpha = 0;

            captureEffect.gameObject.SetActive(false);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = "Score: " + totalScore;
    }

    void ShowAnimalDescription(Animal animal)
    {
        if (animalDescriptionText)
        {
            animalDescriptionText.text = animal.animalDescription;
            StartCoroutine(HideAnimalDescription());
        }
    }

    IEnumerator HideAnimalDescription()
    {
        yield return new WaitForSeconds(3f);
        if (animalDescriptionText) animalDescriptionText.text = "";
    }

    IEnumerator CaptureScreenshot(int width, int height)
    {
        yield return new WaitForEndOfFrame();
        string folderPath = Path.Combine(Application.dataPath, "Fotos");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"Foto_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");

        RenderTexture rt = new RenderTexture(width, height, 24);
        photoCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);

        photoCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        photoCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"📸 Captura guardada en: {filePath}");
    }
}
