//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class TeamSelectUI : MonoBehaviour
//{
//    [SerializeField] private Transform availableListParent;
//    [SerializeField] private Transform selectedListParent;
//    [SerializeField] private GameObject characterButtonPrefab;
//    [SerializeField] private Button startBattleButton;

//    private List<EntityData> selectedTeam = new();

//    private void Start()
//    {
//        InitializeCharacterSelection();
//        startBattleButton.onClick.AddListener(OnStartBattle);
//    }

//    private void InitializeCharacterSelection()
//    {
//        foreach (var character in GameManager.Instance.AllAvailableCharacters)
//        {
//            GameObject buttonGO = Instantiate(characterButtonPrefab, availableListParent);
//            TextMeshProUGUI nameText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
//            nameText.text = character.Name;

//            buttonGO.GetComponent<Button>().onClick.AddListener(() =>
//            {
//                if (selectedTeam.Count < 3 && !selectedTeam.Contains(character))
//                {
//                    selectedTeam.Add(character);
//                    UpdateSelectedList();
//                    UpdateStartButtonState();
//                }
//            });
//        }
//    }

//    private void UpdateSelectedList()
//    {
//        foreach (Transform child in selectedListParent)
//            Destroy(child.gameObject);

//        foreach (var character in selectedTeam)
//        {
//            GameObject buttonGO = Instantiate(characterButtonPrefab, selectedListParent);
//            buttonGO.GetComponentInChildren<Text>().text = character.Name;

//            // Maak verwijderen mogelijk
//            buttonGO.GetComponent<Button>().onClick.AddListener(() =>
//            {
//                selectedTeam.Remove(character);
//                UpdateSelectedList();
//            });
//        }
//    }