using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstThen : MonoBehaviour
{
    public AudioSource first;
    public AudioSource second;

    private bool firstIsFinished = false;
    private bool secondIsFinished = true;

    private bool aboutToPlay = false;

    private void Start()
    {
        first.Play();
    }

    private void Update()
    {
        if (aboutToPlay)
        {
            return;
        }
        if (secondIsFinished && !firstIsFinished && !first.isPlaying)
        {
            aboutToPlay = true;
            Invoke("playSecond", 5f);
        }

        if (firstIsFinished && !secondIsFinished && !second.isPlaying)
        {
            aboutToPlay = true;
            Invoke("playFirst", 5f);
        }
    }

    private void playFirst()
    {
        firstIsFinished = false;
        secondIsFinished = true;
        first.Play();
        aboutToPlay = false;
    }

    private void playSecond()
    {
        firstIsFinished = true;
        secondIsFinished = false;
        second.Play();
        aboutToPlay = false;
    }
}
