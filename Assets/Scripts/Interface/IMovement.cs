using UnityEngine;

public interface IMovement
{
    float MovementSpeed { get; }

    void Move(Vector2 pDirection);
}