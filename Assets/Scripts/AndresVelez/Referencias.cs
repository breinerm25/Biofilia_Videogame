using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Referencias : MonoBehaviour
{
    public RectTransform canva;

    // 🔹 Variables que cambian por jugador
    public TextMeshProUGUI photosRemainingText1, photosRemainingText2;
    public Image previewImage1, previewImage2;
    public CanvasGroup previewCanvasGroup1, previewCanvasGroup2;

    // 🔹 Variables compartidas
    public AudioSource cameraSound;
    public int totalScore = 0;
    public TextMeshProUGUI scoreText;
    public Canvas uiCanvas;
    public Camera photoCamera;

    private static Referencias _instance;
    public static Referencias Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        TakePhotos.ResetPlayer();
    }

    public void AsignarValores(TakePhotos jugador)
    {
        // 🔹 Variables que cambian por jugador
        if (jugador.jugadorID == 1)
        {
            jugador.photosRemainingText = photosRemainingText1;
            jugador.previewImage = previewImage1;
            jugador.previewCanvasGroup = previewCanvasGroup1;
        }
        else if (jugador.jugadorID == 2)
        {
            jugador.photosRemainingText = photosRemainingText2;
            jugador.previewImage = previewImage2;
            jugador.previewCanvasGroup = previewCanvasGroup2;
        }

        // 🔹 Variables compartidas
        jugador.cameraSound = cameraSound;
        TakePhotos.totalScore = totalScore;
        jugador.scoreText = scoreText;
        jugador.uiCanvas = uiCanvas;
        jugador.photoCamera = photoCamera;
    }
}
