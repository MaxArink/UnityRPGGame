using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2DRB : MonoBehaviour, IMovement
{
    [SerializeField] private float _movementSpeed = 1f;

    private Rigidbody2D _rb => GetComponent<Rigidbody2D>();

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }

    public void Move(Vector2 pDirection)
    {
        if (_rb == null)
            return;
        _rb.linearVelocity = pDirection * (_movementSpeed * Time.deltaTime);
    }
}