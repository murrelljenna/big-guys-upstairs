using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class JustDie : MonoBehaviour
{
    [Tooltip("List of animation names, one of which will be play randomly")]
    public string[] animationNames; // Pass in a list of fucking names apparently
    
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        // Is this seriously the best API Unity could think of for this fucking component?
        // No method to get a list of all the animations to play one randomly. No. We are going
        // to make you literally pass a string naming the fucking animation... filename? 
        // Seriously who thought this was a good idea? There is literally NO METHOD that 
        // simply returns AnimationClip[]. I can get a single AnimationClip if I PASS
        // A FUCKING STRING representing SOMETHING that identifies the clip, apparently,
        // but NOTHING that will just give me ALL CLIPS.
        // So fucking stupid.
        string name = animationNames[Random.Range(0, animationNames.Length)];
        Debug.Log(name);
        anim.Play(name); // Fuck you
    }
}
