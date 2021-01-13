using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    void Start()
    {
        Explode();
    }

    void Explode()
    {
        ParticleSystem exp = GetComponentInChildren<ParticleSystem>();
        exp.Play();
        Destroy(gameObject, exp.main.duration);
    }
}
