using UnityEngine;

public class EnemyInput : MonoBehaviour, IEnemyInput
{
    private EnemyStateManager _iiii;

    public Vector2 GetMovementInput()
    {
        Vector2 currentInput = Vector2.zero;

        // logica voor movement

        //Temp
        if (Input.GetMouseButton(0))
            currentInput.y++;
        if (Input.GetMouseButton(1))
            currentInput.y--;

        return currentInput == Vector2.zero ? currentInput : currentInput.normalized;
    }
}

public class EnemyStateManager
{
    private IEnemyState _enemyState;


}

public enum EnemyState
{
    Idle,
    Patrol,
    Chase
}

public interface IEnemyState
{
    void EnterState(EnemyStateManager pEnemy);
    void UpdateState(EnemyStateManager pEnemy);
    void ExitState(EnemyStateManager pEnemy);
}

//public class IdleState : IEnemyState
//{
//    public void EnterState(EnemyStateManager pEnemy) => Debug.Log("Entering Idle State");

//    public void UpdateState(EnemyStateManager pEnemy)
//    {
//        // Check voor overgang naar andere staat
//        if (/* bijvoorbeeld player in zicht */ false)
//        {
//            pEnemy.SwitchState(new ChaseState());
//        }
//    }

//    public void ExitState(EnemyStateManager pEnemy) => Debug.Log("Exiting Idle State");
//}

//public class PatrolState : IEnemyState
//{
//    public void EnterState(EnemyStateManager pEnemy) => Debug.Log("Entering Patrol State");

//    public void UpdateState(EnemyStateManager pEnemy)
//    {
//        if (/* bijvoorbeeld player in zicht */ false)
//        {
//            pEnemy.SwitchState(new ChaseState());
//        }

//        // Logica om te patrouilleren

//    }

//    public void ExitState(EnemyStateManager pEnemy) => Debug.Log("Exiting Patrol State");
//}

//public class ChaseState : IEnemyState
//{
//    public void EnterState(EnemyStateManager pEnemy) => Debug.Log("Entering Chase State");

//    public void UpdateState(EnemyStateManager pEnemy)
//    {
//        // Achtervolg speler
//    }

//    public void ExitState(EnemyStateManager pEnemy) => Debug.Log("Exiting Chase State");
//}

//public class EnemyStateManager : MonoBehaviour
//{
//    private IEnemyState _currentState;

//    private void Start()
//    {
//        // Start in Idle
//        SwitchState(new IdleState());
//    }

//    private void Update()
//    {
//        _currentState?.UpdateState(this);
//    }

//    public void SwitchState(IEnemyState newState)
//    {
//        _currentState?.ExitState(this);
//        _currentState = newState;
//        _currentState?.EnterState(this);
//    }
//}

