using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI_Manager : MonoBehaviour
{
    public GameManager GM;
    public TMPro.TextMeshProUGUI Cointext;
    public Slider HealthSlider;

    public GameObject UI_Pause;
    public GameObject UI_GameOver;

    public GameObject UI_GameIsFinished;

    private enum GameUI_Stage
    {
        GamePlay, Pause, GameOver, GameIsFinished
    }
    GameUI_Stage currentState;


    // Start is called before the first frame update
    void Start()
    {
        SwithUIState(GameUI_Stage.GamePlay);
    }

    // Update is called once per frame
    void Update()
    {
        HealthSlider.value = GM.playerCharacter.GetComponent<Health>().CurrentHealthPercentage;
        Cointext.text = GM.playerCharacter.Coin.ToString();
    }




    private void SwithUIState(GameUI_Stage state)
    {
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);
        Time.timeScale = 1;
        switch (state)
        {
            case GameUI_Stage.GamePlay:
                break;
            case GameUI_Stage.Pause:
                Time.timeScale = 0;
                UI_Pause.SetActive(true);
                break;
            case GameUI_Stage.GameOver:
                UI_GameOver.SetActive(true);
                break;
            case GameUI_Stage.GameIsFinished:
                UI_GameIsFinished.SetActive(true);
                break; 
        }

        currentState = state;
    }


    public void TogglePauseUI()
    {
        if (currentState == GameUI_Stage.GamePlay)
        {
            SwithUIState(GameUI_Stage.Pause);
            Debug.Log("Pause");
        }
        else if (currentState == GameUI_Stage.Pause)
        {
            SwithUIState(GameUI_Stage.GamePlay);
            Debug.Log("Game play");
        }
    }

    public void Button_MainManu()
    {
        GM.ReturnToTheMainMenu();
    }
    public void Buton_Restart()
    {
        GM.Restart();
    }

    public void ShowGameOverUI()
    {
        SwithUIState(GameUI_Stage.GameOver);
    }

    public void ShowGameFinishedUI()
    {
        SwithUIState(GameUI_Stage.GameIsFinished);
    }

}
