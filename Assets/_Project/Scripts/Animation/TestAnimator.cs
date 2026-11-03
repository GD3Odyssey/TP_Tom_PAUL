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
    }

    public void TriggerAttack1H()
    {
        _animator.SetTrigger("Attack1H");
    }

    public void TriggerAttack2H()
    {
        _animator.SetTrigger("Attack2H");
    }
}