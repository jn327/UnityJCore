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
    private float _minVelocity = 0f;
    [SerializeField]
    private float _maxVelocity = 10.0f;

    [SerializeField]
    private float _minEmission = 0f;
    [SerializeField]
    private float _maxEmission = 10.0f;

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
        if (_rb.velocity.magnitude > _minVelocity)
        {
            float velocityN = Mathf.Min(Mathf.InverseLerp(_minVelocity, _maxVelocity, _rb.velocity.magnitude), 1.0f);
            float emission = Mathf.Lerp( _minEmission, _maxEmission, _emissionCurve.Evaluate(velocityN));

            _particleSystem.Emit((int)emission);
        }
    }
}
