using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private IPlayerInput _input;
    private IMovement _movement;

    void Awake()
    {
        _input = GetComponent<IPlayerInput>();
        _movement = GetComponent<IMovement>(); 
    }

    void Update()
    {
        if (_input == null || _movement == null)
            return;

        Vector2 movementInput = _input.GetMovementInput();

        _movement.Move(movementInput);
    }

    public void RefreshInputComponent()
    {
        _input = GetComponent<IPlayerInput>();
    }
}
