using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadScene;
    }

    private void LoadScene(Scene scene, LoadSceneMode mode)
    {
        if (SaveSystem.IsSaveData()) Events.LoadProgress();
    }
    

}
