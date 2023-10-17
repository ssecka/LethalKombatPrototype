using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    private Animator animator;
    private const bool enabled = false;

    private void Start()
    {
        // Get the Animator component from your character.
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!enabled) return;
        // Check if your bool condition is true, and if so, trigger the animation.
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetTrigger("Kick");
        }
    }
}
