using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Referentie naar input component die spelerbewegingen leest
    private IPlayerInput _input;

    // Referentie naar movement component die de beweging uitvoert
    private IMovement _movement;

    void Awake()
    {
        // Haal de input en movement componenten op bij initialisatie
        _input = GetComponent<IPlayerInput>();
        _movement = GetComponent<IMovement>();
    }

    void Update()
    {
        // Check of beide componenten aanwezig zijn voordat we doorgaan
        if (_input == null || _movement == null)
            return;

        // Lees de huidige bewegingsinput van de speler
        Vector2 movementInput = _input.GetMovementInput();

        // Voer de beweging uit op basis van de input
        _movement.Move(movementInput);
    }

    // Methode om de input component opnieuw te laden (bijv. na wijzigingen)
    public void RefreshInputComponent()
    {
        _input = GetComponent<IPlayerInput>();
    }
}
