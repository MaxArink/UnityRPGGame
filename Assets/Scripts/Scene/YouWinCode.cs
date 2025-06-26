using UnityEngine;

public class YouWinCode : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneLoader.Instance.LoadSceneSingle("OverworldScene");
        }
    }
}
