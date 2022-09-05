using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using game.assets.audio;

namespace Tests
{
    public class TestAudioController
    {
        private AudioController audioController;
        private AudioSource[] audios;
        [SetUp]
        public void SetUp()
        {
            GameObject gameObject = new GameObject("TestAudio");
            GameObject[] gosWithAudio = new GameObject[] {
                new GameObject("AudioSource"),
                new GameObject("AudioSource2")
            };

            audios = new AudioSource[gosWithAudio.Length];

            for (int i = 0; i < gosWithAudio.Length; i++)
            {
                audios[i] = gosWithAudio[i].AddComponent(typeof(AudioSource)) as AudioSource;
                gosWithAudio[i].transform.parent = gameObject.transform;
            }

            audioController = gameObject.AddComponent(typeof(AudioController)) as AudioController;
        }

        [Test]
        public void TestGetClips()
        {
            AudioSource[] audioWeGot = audioController.Get("AudioSource");
            Assert.AreEqual(audioWeGot[0], audios[0]);
        }
    }
}
