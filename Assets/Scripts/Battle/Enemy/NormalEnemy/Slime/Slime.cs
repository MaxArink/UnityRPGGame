using UnityEngine;

public class Slime : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();

    public void BaseAction()
    {
        throw new System.NotImplementedException();
    }
}
