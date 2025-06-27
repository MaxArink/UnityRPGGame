using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private IMovement _movement;
    // interface for Enemy move direction
    private IEnemyInput _input;

    public void Awake()
    {
        _movement = GetComponent<IMovement>();
        _input = GetComponent<IEnemyInput>();
    }

    void Update()
    {
        Vector2 movementInput = _input.GetMovementInput();

        _movement.Move(movementInput);
    }

}