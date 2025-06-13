using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    [SerializeField] private ChallengeData _challengeData;

    public ChallengeData ChallengeData => _challengeData;

    // Hier kan je bv. detecteren of de speler collideert en BattleManager aanroepen:
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start het gevecht!
            //BattleManager.instance.StartBattle(_challengeData);
        }
    }
}