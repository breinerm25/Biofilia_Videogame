using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class SceneTransitioSimple : MonoBehaviour
{
    public string nextSceneName; 
    public float transitionDuration = 0.5f; 
    public CanvasGroup fadeCanvas; 
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(StartTransition);
    }

    public void StartTransition()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.DOFade(1, transitionDuration).OnComplete(LoadNextScene);
        }
        else
        {
            LoadNextScene();
        }
    }

   public void LoadNextScene()
    {
        DOTween.Clear(true);
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
