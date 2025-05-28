using UnityEngine;

public class Character : MonoBehaviour
{
    private ICharacter _character;

    private void Awake()
    {
        _character = GetComponent<ICharacter>();
    }

    private void Start()
    {
        if ( _character != null)
            _character.CharacterInfo();
    }
}
