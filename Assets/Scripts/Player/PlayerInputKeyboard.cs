using UnityEngine;

public class PlayerInputKeyboard : MonoBehaviour, IPlayerInput
{
    [SerializeField] private KeyCode _upKey = KeyCode.W;
    [SerializeField] private KeyCode _downKey = KeyCode.S;
    [SerializeField] private KeyCode _leftKey = KeyCode.A;
    [SerializeField] private KeyCode _rightKey = KeyCode.D;

    public Vector2 GetMovementInput()
    {
        Vector2 currentInput = Vector2.zero;

        if (Input.GetKey(_upKey))
            currentInput.y++;

        if (Input.GetKey(_downKey))
            currentInput.y--;

        if (Input.GetKey(_leftKey))
            currentInput.x--;

        if (Input.GetKey(_rightKey))
            currentInput.x++;

        return currentInput == Vector2.zero ? currentInput : currentInput.normalized;
    }
}
