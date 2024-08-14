using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenuUI;

    public void HideGameOverMenu()
    {
        gameOverMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
