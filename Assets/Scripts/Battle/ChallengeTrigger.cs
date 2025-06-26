using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    [SerializeField] private ChallengeData _challengeData;
    [SerializeField] private string _battleSceneName = "BattleScene"; // <-- vul je battle scene in

    private bool _hasTriggered = false;

    public void ResetTrigger()
    {
        _hasTriggered = false;
        gameObject.SetActive(true); // ook heractiveren, als je hem ooit hebt uitgezet
    }

    private void OnTriggerEnter2D(Collider2D pOther)
    {
        if (_hasTriggered) return;

        Player player = pOther.GetComponent<Player>();
        if (player != null)
        {
            _hasTriggered = true;
            GameManager.Instance.SetPendingBattleData(_challengeData, this);
            Debug.Log("Start Battle");
            SceneLoader.Instance.LoadSceneAdditive(_battleSceneName);
        }
    }

    public void OnBattleWon()
    {
        gameObject.SetActive(false);  // verberg de overworld enemy bij winst
    }

    public void OnBattleLost()
    {
        ResetTrigger(); // reset zodat je opnieuw kan triggeren
    }
}
