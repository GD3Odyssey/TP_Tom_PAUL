using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TriggerAttack1H();
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            TriggerAttack2H();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GetComponent<AbilityManager>().TryUseAbility(0);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            GetComponent<AbilityManager>().TryUseAbility(1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<AbilityManager>().TryUseAbility(2);
        }
    }

    public void TriggerAttack1H()
    {
        _animator.SetTrigger("Attack1H");
    }

    public void TriggerAttack2H()
    {
        _animator.SetTrigger("Attack2H");
    }

    // les 2 méthodes ci dessous sont temporaires pour montrer la méthode
    public class GameCompetence : ScriptableObject
    {
        // exemple scriptable objects
    }

    public void LoadCompencesDef()
    {
        GameCompetence[] competencesDef = Resources.LoadAll<GameCompetence>("Chemin du dossier / dans Resources");
    }
}