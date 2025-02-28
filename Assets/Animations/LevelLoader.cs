using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator trans;

    public float transitionTime = 1f;

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //play animation
        trans.SetTrigger("start");

        //wait
        yield return new WaitForSeconds(transitionTime);
        
        //Load scene
        SceneManager.LoadScene(levelIndex);
    }
}
