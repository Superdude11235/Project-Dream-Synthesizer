using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{   
    [SerializeField] private const int MAIN_SCENE_ID = 3;
    // changes to scene with int sceneID (shown in file -> build settings)
    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    // quits game, will only work once the game is built
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void NewGame()
    {
        SaveSystem.DeleteData();
        LoadGame();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(MAIN_SCENE_ID);
    }
}