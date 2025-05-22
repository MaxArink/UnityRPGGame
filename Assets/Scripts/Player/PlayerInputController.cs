using UnityEngine;

public class PlayerInputController : MonoBehaviour, IPlayerInput
{
    public Vector2 GetMovementInput()
    {
        Vector2 currentInput = Vector2.zero;

        currentInput.x = Input.GetAxis("Horizontal");
        currentInput.y = Input.GetAxis("Vertical");

        return currentInput == Vector2.zero ? currentInput : currentInput.normalized;
    }
}
