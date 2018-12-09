using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { set; get; }

    public Text countText;
    public Text winText;
    public GameObject pauseMenuUI;
    public string lastLevel;

    // Game States
    public bool isGameOver;
    public bool isGamePaused = false;

    private const string PLAYER = "Player";
    private static int nextSceneIndex;
    private static int currScene = 2; // Start of game
    private Respawm respawm;


    private void Start()
    {

        Instance = this;
        Load(PLAYER);
        Load(currScene);

        nextSceneIndex = currScene + 1;
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
        }

        // Open menu "Esc"
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (isGamePaused) ResumeGame();
            else PauseGame();
            
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
        // Reset the map
        Unload(currScene);
        Load(currScene);
        
        LoadPlayer();

        isGameOver = false;
        winText.text = "";
        countText.text = "";

       StartCoroutine(DisableEnterStage());
        ResumeGame();
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
    
    public void StageWon()
    {
        if (SceneManager.GetSceneByName(lastLevel).isLoaded)
        {
            GameWon();
            return;
        }

        // Load path for the next stage
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + nextSceneIndex, LoadSceneMode.Additive); // Don't like this part... to hacky
        FindObjectOfType<RemoveWall>().gameObject.GetComponent<Renderer>().enabled = false;
        FindObjectOfType<RemoveWall>().gameObject.GetComponent<BoxCollider>().isTrigger = true;

        nextSceneIndex += 2;

        currScene = nextSceneIndex - 1;
    }

    public void GameWon()
    {
        EndGame("You Won! =]", "Congratulation on winning & Thanks for playing !!");
    }

    public void GameOver()
    {
       EndGame("You Died!", "Press 'space' to reset");
    }
    
    void EndGame(string endGameTitle, string endGameText)
    {
        winText.text = endGameTitle;
        countText.text = endGameText;
        isGameOver = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    /** END Game States **/


    public void Load (string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    public void Load(int sceneIndex)
    {
        if (!SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
        {
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
        }
    }

    public void Unload (string sceneName)
    {
        
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneName).buildIndex);
        }
    }

    public void Unload(int sceneIndex)
    {

        if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(sceneIndex).buildIndex);
        }
    }

    private void LoadPlayer()
    {
        SetOnPlayerRespawm();
        
    }

    private void SetOnPlayerRespawm()
    {
        // Get the current position of the respawn point
        float x = FindObjectOfType<Respawm>().gameObject.transform.position.x;
        float y = FindObjectOfType<Respawm>().gameObject.transform.position.y;
        float z = FindObjectOfType<Respawm>().gameObject.transform.position.z;

        // Get the instance of the player
        GameObject player = null;
        GameObject[] gameObjects = SceneManager.GetSceneByName(PLAYER).GetRootGameObjects();
        foreach (GameObject gameObject in gameObjects)
        {
            // Found the object of player
            if(gameObject.CompareTag(PLAYER)) player = gameObject;
            
        }
       
        // No player exist, finish this method
        if (player == null) return;

        CleanPlayer(player);

        Vector3 respawmPos = new Vector3(x, y, z);
        // Set the player on the respawn position
        player.transform.position = respawmPos;
        
        // Stop the player on the spot
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void CleanPlayer(GameObject player)
    {
        if (player == null) return;

        player.SetActive(true);
        player.GetComponent<PlayerController>().CleanAfterDeath();

    }

    private IEnumerator DisableEnterStage()
    {
        yield return new WaitForSeconds(1);
        // Get the instance of the player
        GameObject enterStage = null;

        GameObject[] gameObjects = SceneManager.GetSceneByBuildIndex(currScene).GetRootGameObjects();
        foreach (GameObject gameObject in gameObjects)
        {
            // Make EnterStage untriggered
            if (gameObject.CompareTag("EnterStage"))
            {
                gameObject.GetComponent<BoxCollider>().isTrigger = false;
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }

        //FindObjectOfType<EnterStage>().gameObject.GetComponent<BoxCollider>().enabled = false;
    }

}
