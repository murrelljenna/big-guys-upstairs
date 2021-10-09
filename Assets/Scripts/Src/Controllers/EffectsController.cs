using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public void PlayRandom(string name, int count)
    {
        ParticleSystem[] potentialParticleSystem = Get(name);
        ParticleSystem particles = potentialParticleSystem[Random.Range(0, potentialParticleSystem.Length)];
        particles.Emit(count);
    }

    private ParticleSystem[] Get(string name)
    {
        return transform.Find(name)?.gameObject?.GetComponents<ParticleSystem>();
    }
}
