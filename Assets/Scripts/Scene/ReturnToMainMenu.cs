using UnityEngine;

public class ReturnToMainMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.EnemiesDefeated = 0; // Reset the defeated enemies count
            SceneLoader.Instance.LoadSceneSingle("MainMenu");
        }
    }
}
