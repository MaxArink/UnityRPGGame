using TMPro;
using UnityEngine;

public class InputSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _player; // GameObject met PlayerMove
    [SerializeField] private TextMeshProUGUI _inputStatusText;

    private bool _usingKeyboard = true;

    private void Start()
    {
        ToggleInput();
    }

    public void ToggleInput()
    {
        // Verwijder huidige inputcomponenten
        PlayerInputKeyboard keyboardInput = _player.GetComponent<PlayerInputKeyboard>();
        PlayerInputController controllerInput = _player.GetComponent<PlayerInputController>();

        if (keyboardInput != null)
            Destroy(keyboardInput);
        if (controllerInput != null)
            Destroy(controllerInput);

        // Voeg de gewenste inputcomponent toe
        if (_usingKeyboard == false)
        {
            _player.AddComponent<PlayerInputController>();
            UpdateStatusText("Controller");
            Debug.Log("Switched to keyboard input");
        }
        else
        {
            _player.AddComponent<PlayerInputKeyboard>();
            UpdateStatusText("Keyboard");
            Debug.Log("Switched to controller input");
        }

        _usingKeyboard = !_usingKeyboard;

        // Forceer opnieuw ophalen van input-component in PlayerMove
        PlayerMove move = _player.GetComponent<PlayerMove>();
        move.RefreshInputComponent();
    }

    private void UpdateStatusText(string pInputName)
    {
        if (_inputStatusText != null)
        {
            _inputStatusText.text = $"Active Input: <b>{pInputName}</b>";
        }
    }
}