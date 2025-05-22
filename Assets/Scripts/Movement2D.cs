using UnityEngine;

public class Movement2D : MonoBehaviour, IMovement
{
    [SerializeField] private float _movementSpeed = 1f;

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }

    public void Move(Vector2 pDirection)
    {
        transform.Translate(pDirection * (_movementSpeed * Time.deltaTime));
    } 
}