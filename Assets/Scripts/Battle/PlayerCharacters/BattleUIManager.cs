using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    [Header("Skill UI")]
    [SerializeField] private Transform _skillButtonContainer;
    [SerializeField] private Button _skillButtonPrefab;

    [Header("Damage Counter UI")]
    [SerializeField] private TMP_Text _damageText;

    private Character _currentCharacter;
    private float _totalDamage = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -------------------
    // SKILL UI
    // -------------------

    public void ShowSkillOptions(Character pCharacter)
    {
        _currentCharacter = pCharacter;

        // Eerst alle oude knoppen opruimen
        foreach (Transform child in _skillButtonContainer)
            Destroy(child.gameObject);

        // Voor elke skill een knop maken
        foreach (Skill skill in pCharacter.Skills)
        {
            Button btn = Instantiate(_skillButtonPrefab, _skillButtonContainer);
            btn.GetComponentInChildren<TMP_Text>().text = skill.Name;

            btn.onClick.AddListener(() => OnSkillChosen(skill));
        }
    }

    private void OnSkillChosen(Skill chosenSkill)
    {
        Debug.Log($"Speler koos skill: {chosenSkill.Name}");

        // Targets bepalen (bijvoorbeeld eerst automatische enemy target, of via target UI)
        bool targetsAllies = chosenSkill.SkillType == SkillType.Heal || chosenSkill.SkillType == SkillType.Buff;
        TargetingType targetingType = BattleManager.Instance.TargetingUtils.ConvertToTargetingType(chosenSkill.TargetType);
        List<Entity> targets = BattleManager.Instance.TargetingService.GetTargets(targetingType, _currentCharacter, targetsAllies);

        // Skill uitvoeren
        _currentCharacter.PerformSkill(chosenSkill, targets);

        // Buffs / einde beurt
        _currentCharacter.TickBuffs();
        BattleManager.Instance.EndTurn();

        // UI verbergen
        HideSkillOptions();
    }

    // -------------------
    // DAMAGE COUNTER
    // -------------------

    private void HideSkillOptions()
    {
        foreach (Transform child in _skillButtonContainer)
            Destroy(child.gameObject);
    }


    public void AddDamage(float amount)
    {
        _totalDamage += amount;
        UpdateDamageText();
    }

    public void ResetDamageCounter()
    {
        _totalDamage = 0f;
        UpdateDamageText();
    }

    private void UpdateDamageText()
    {
        if (_damageText != null)
            _damageText.text = $"Damage: {_totalDamage}";
    }
}
