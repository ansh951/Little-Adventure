using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    public Character playerCharacter;
    private bool gameIsOver;

    public GameUI_Manager gameUI_Manager;


    private void Awake()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void GameOver()
    {
        gameUI_Manager.ShowGameOverUI();
    }

    public void GameIsFinished()
    {
        gameUI_Manager.ShowGameFinishedUI();

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameUI_Manager.TogglePauseUI();
            Debug.Log("Toggle Button");
        }

        if (playerCharacter.currentState == Character.CharacterState.Dead)
        {
            gameIsOver = true;
            GameOver();
        }

    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
