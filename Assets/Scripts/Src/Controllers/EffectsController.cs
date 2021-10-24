using game.assets.ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    private const int DEFAULT_COUNT = 30;
    public void PlayDestroyEffects(Health health)
    {
        ParticleSystem[] potentialParticleSystem = Get("DestroyEffects");
        if (potentialParticleSystem == null)
        {
            Debug.LogError("Trying to run particleSystem that is null");
            return;
        }
        ParticleSystem particles = potentialParticleSystem[Random.Range(0, potentialParticleSystem.Length)];
        particles.Emit(DEFAULT_COUNT);
    }

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
