using UnityEngine;

public class Movement3D : MonoBehaviour, IMovement
{
    [SerializeField] private float _movementSpeed = 1f;

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }

    public void Move(Vector2 pDirection)
    {
        Vector3 moveDirection = new Vector3(pDirection.x, 0, pDirection.y);
        transform.Translate(moveDirection * (_movementSpeed * Time.deltaTime));
    }
}