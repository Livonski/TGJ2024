using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<LevelController>().LoadNextLevel();
        }
    }
}