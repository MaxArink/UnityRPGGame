using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    [SerializeField] private ChallengeData _challengeData;
    [SerializeField] private string _battleSceneName = "BattleScene"; // Vul hier je battle scene naam in

    private bool _hasTriggered = false;

    /// <summary>
    /// Reset de trigger zodat het opnieuw kan worden geactiveerd
    /// </summary>
    public void ResetTrigger()
    {
        _hasTriggered = false;
        gameObject.SetActive(true); // Heractiveer het object, als die ooit is uitgezet
    }

    private void OnTriggerEnter2D(Collider2D pOther)
    {
        if (_hasTriggered) return; // Niet opnieuw triggeren

        Player player = pOther.GetComponent<Player>();
        if (player != null)
        {
            _hasTriggered = true;
            // Stel de battle data in in GameManager, geef deze trigger door als referentie
            GameManager.Instance.SetPendingBattleData(_challengeData, this);

            Debug.Log("Start Battle");
            // Laad de battle scene additive (bovenop de huidige)
            SceneLoader.Instance.LoadSceneAdditive(_battleSceneName);
        }
    }

    /// <summary>
    /// Wordt aangeroepen als de battle gewonnen is
    /// </summary>
    public void OnBattleWon()
    {
        // Verberg de trigger (bv enemy) in de overworld zodat je niet opnieuw tegen dezelfde vecht
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Wordt aangeroepen als de battle verloren is
    /// </summary>
    public void OnBattleLost()
    {
        // Reset de trigger zodat de speler het opnieuw kan proberen
        ResetTrigger();
    }
}
