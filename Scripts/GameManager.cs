using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


    public Text countText;
    public Text winText;
    public GameObject pauseMenuUI;

    // Game States
    public bool isGameOver;
    public bool isGamePaused = false;
    
    

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
        }

        // Open menu "Esc"
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
            
        }
    }

    /** Game States **/
    public void StartGame()
    {
        countText.text = "";
        winText.text = "";
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }
    
    public void GameWon()
    {
       EndGame("You Won! =]");
    }

    public void GameOver()
    {
       EndGame("You Died!");
    }
    
    void EndGame(string endGameText)
    {
        winText.text = endGameText;
        countText.text = "Press 'space' to reset";
        isGameOver = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    /** END Game States **/
    
}
