using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadButtonInfo(string pSceneName)
    {
        // Laad de game scene
        SceneLoader.Instance.LoadSceneSingle(pSceneName);
    }

    public void LoadExit()
    {
        // Sluit de applicatie
        Application.Quit();

        // Voor de editor, om het te testen
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
