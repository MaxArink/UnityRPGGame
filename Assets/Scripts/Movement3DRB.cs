using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement3DRB : MonoBehaviour, IMovement
{
    [SerializeField] private float _movementSpeed = 1f;

    private Rigidbody _rb => GetComponent<Rigidbody>();

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }

    public void Move(Vector2 pDirection)
    {
        Vector3 moveDirection = new Vector3(pDirection.x, 0, pDirection.y);
        _rb.angularVelocity = moveDirection * (_movementSpeed * Time.deltaTime);
        //stransform.Translate(moveDirection * (_movementSpeed * Time.deltaTime));
    }
}
