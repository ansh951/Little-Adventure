using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI_Manager : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("Overview");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
