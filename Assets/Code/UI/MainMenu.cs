using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exit requested");

#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}