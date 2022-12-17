using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOtherFuckingAudioListeners : MonoBehaviour
{
    private AudioListener theOnlyFuckingListenerThatShouldBeInTheGame;
    public void Start()
    {
        theOnlyFuckingListenerThatShouldBeInTheGame = GetComponent<AudioListener>();
        int i = 0;
        foreach (Object obj in FindObjectsOfType<AudioListener>())
        {
            i++;
            AudioListener stopFuckingSpammingMe = (AudioListener)obj;

            if (stopFuckingSpammingMe != theOnlyFuckingListenerThatShouldBeInTheGame) {
                stopFuckingSpammingMe.enabled = false;
            }
        }
    }
}
