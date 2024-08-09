using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{
    // changes to scene with int sceneID (shown in file -> build settings)
    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    // quits game, will only work once the game is built
    public void QuitGame()
    {
        Application.Quit();
    }
}