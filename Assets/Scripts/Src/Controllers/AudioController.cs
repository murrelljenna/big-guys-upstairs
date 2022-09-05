using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.audio {
    public class AudioController : MonoBehaviour
    {
        public AudioSource[] Get(string name)
        {
            Transform potentialAudio = transform.Find(name);

            if (potentialAudio != null)
            {
                AudioSource[] audioSources = potentialAudio.gameObject.GetComponents<AudioSource>();
                return audioSources;
            }
            else
            {
                return null;
            }
        }

        public void PlayRandom(string name)
        {
            AudioSource[] potentialAudio = Get(name);
            AudioSource audio = potentialAudio[Random.Range(0, potentialAudio.Length)];
            AudioSource.PlayClipAtPoint(audio.clip, transform.position);
        }
    }
}
