using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    public GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerPrefs.SetInt("LastLevelIndex", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        ManagePlayerActivation(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            PlayerPrefs.SetInt("LastLevelIndex", nextSceneIndex);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("No more levels!");
            SceneManager.LoadScene(0);
        }
    }

    public void reload()
    {
        ContinueLastLevel();
    }

    public static void ContinueLastLevel()
    {
        int lastLevelIndex = PlayerPrefs.GetInt("LastLevelIndex", 1);
        lastLevelIndex = lastLevelIndex == 0 ? 1 : lastLevelIndex;

        if (lastLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(lastLevelIndex);
        }
        else
        {
            Debug.Log("Invalid level index stored");
            SceneManager.LoadScene(1);
        }
    }

    public void ReloadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(0);
        SceneManager.LoadScene(currentSceneIndex);
        PlayerPrefs.SetInt("LastLevelIndex", currentSceneIndex);
        PlayerPrefs.Save();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetPlayerPosition();
        ManagePlayerActivation(scene.buildIndex);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ResetPlayerPosition()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("resseting player position");
        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    private void ManagePlayerActivation(int sceneIndex)
    {
        if (player != null)
        {
            player.SetActive(sceneIndex != 0);
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }
}