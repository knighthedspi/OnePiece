using UnityEngine;
using System.Collections;

public class ParticleUtility : MonoBehaviour {

    public static bool IsFinishedEmitParticles(ParticleSystem particle) {
        if(particle == null) return true;
        if(particle.particleCount > 0) return false;
        foreach(Transform t in particle.transform) {
            if(!IsFinishedEmitParticles(t.particleSystem)) return false;
        }
        return true;
    }
}
