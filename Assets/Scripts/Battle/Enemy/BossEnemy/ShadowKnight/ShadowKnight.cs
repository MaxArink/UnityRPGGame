using UnityEngine;

public class ShadowKnight : MonoBehaviour, IEnemy
{
    public Enemy Enemy => GetComponent<Enemy>();

    public void BaseAction()
    {
        throw new System.NotImplementedException();
    }
}
