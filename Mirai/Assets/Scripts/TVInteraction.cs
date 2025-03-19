using UnityEngine;

public class TVInteraction : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TurnOn()
    {
        animator.SetBool("is_on", true);
    }

    public void TurnOff()
    {
        animator.SetBool("is_on", false);
    }
}
