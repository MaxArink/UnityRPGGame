using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    [SerializeField] private ChallengeData _challengeData;
    [SerializeField] private string _battleSceneName = "BattleScene"; // <-- vul je battle scene in

    private bool _hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D pOther)
    {
        if (_hasTriggered) return;

        Player player = pOther.GetComponent<Player>();
        if (player != null)
        {
            _hasTriggered = true;
            GameManager.Instance.SetPendingBattleData(_challengeData);
            Debug.Log("Start Battle");
            SceneLoader.Instance.LoadSceneAdditive(_battleSceneName);
        }
    }
}
