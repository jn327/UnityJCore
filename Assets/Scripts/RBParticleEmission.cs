using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows for emission from a particle system based on a Rigidbody2D's velocity.
public class RBParticleEmission : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private ParticleSystem _particleSystem;

    [SerializeField]
    private MinMaxFloat _velocityLimits = new MinMaxFloat( 0, 10.0f );

    [SerializeField]
    private MinMaxFloat _emissionRange = new MinMaxFloat( 0, 10.0f );

    [SerializeField]
    private AnimationCurve _emissionCurve = AnimationCurve.Linear(0, 0, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        if (!_rb)
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        if (!_particleSystem)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity.magnitude > _velocityLimits.min)
        {
            float velocityN = Mathf.Clamp( _velocityLimits.InverseLerp(_rb.velocity.magnitude), 0, 1 );
            float emission = _emissionRange.Lerp(velocityN);

            _particleSystem.Emit((int)emission);
        }
    }
}
