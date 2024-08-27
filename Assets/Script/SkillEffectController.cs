using UnityEngine;

public class SkillEffectController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator attached to this GameObject.
        animator = GetComponent<Animator>();
    }

    // Call this function to play the skill effect animation.
    public void PlayEffectAnimation()
    {
        // Set the trigger for the skill effect animation to play.
        animator.SetTrigger("PlayEffect");
    }
}
