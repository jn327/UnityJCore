using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct WindValue
{
	private Vector2 _angle;
	private float _strength;

    public WindValue(Vector2 angle, float strength)
    {
        _angle = angle;
        _strength = strength;
    }

	public Vector2 GetAngle()
	{
		return _angle;
	}

	public float GetStrength()
	{
		return _strength;
	}
}

[System.Serializable]
public struct ParticleLayer
{
	public ParticleLayer(ParticleSystem _particleSystem, float _strength = 2, float _dampen = 0.5f)
    {
        particleSystem = _particleSystem;
        strength = _strength;
		dampen = _dampen;
    }

	public ParticleSystem particleSystem;
	public float strength;
	[Range(0,1)]
	public float dampen;
}

public class WindController : MonoBehaviour 
{
	[SerializeField]
	private LayerMask _layersToAffect;

	[SerializeField]
	private float _rbStayForce = 5.0f;

	[SerializeField]
	private WindNoiseLayer[] _strengthNoise;
	private float _strTotalWeigthing = 0;
	private int _strTotalEnabled = 0;
	[SerializeField]
	private WindNoiseLayer[] _angleNoise;
	private float _angleTotalWeigthing = 0;
	
	[SerializeField]
	[Range(1,6)]
	//ratio between the noise value and the amount of radians the resulting vector will be rotated by,
	// this obfuscates the otherwise slightly obvious extremes that the wind can be rotated by.
	private float _angleRatio = 4;

	[SerializeField]
	private List<ParticleLayer> _particlesAffected;
	private ParticleSystem.Particle[] _particles;

	public void RegisterParticleSystem(ParticleSystem particleSystem, float strength = 2, float dampen = 0.5f)
	{
		_particlesAffected.Add( new ParticleLayer(particleSystem, strength, dampen) );
	}

	public void UnregisterParticleSystem(ParticleSystem particleSystem)
	{
		for (int i = 0; i < _particlesAffected.Count; i++)
		{
			if (_particlesAffected[i].particleSystem == particleSystem)
			{
				_particlesAffected.RemoveAt(i);
				i = _particlesAffected.Count;
			}
		}
	}

	void Start () 
	{
		_strTotalWeigthing = 0;
		_strTotalEnabled = 0;
		for (int i = 0; i < _strengthNoise.Length; i++)
		{
			_strengthNoise[i].Start();

			if (_strengthNoise[i].IsEnabled())
			{
				_strTotalEnabled ++;
				_strTotalWeigthing += _strengthNoise[i].GetWeighting();
			}
		}
		if (_strTotalWeigthing <= 0)
		{
			_strTotalWeigthing = 1;
		}

		_angleTotalWeigthing = 0;
		for (int j = 0; j < _angleNoise.Length; j++)
		{
			_angleNoise[j].Start();

			if (_angleNoise[j].IsEnabled())
			{
				_angleTotalWeigthing += _angleNoise[j].GetWeighting();
			}
		}
		if (_angleTotalWeigthing <= 0)
		{
			_angleTotalWeigthing = 1;
		}
	}
	
	void Update () 
	{
		int nParticles;
		WindValue windVal;
		Vector3 forceToAdd;
		for (int j = 0; j < _particlesAffected.Count; j++)
		{
			if (_particles == null || _particles.Length < _particlesAffected[j].particleSystem.main.maxParticles)
            	_particles = new ParticleSystem.Particle[_particlesAffected[j].particleSystem.main.maxParticles];

			// GetParticles is often allocation free because we reuse the _particles buffer between updates
       		nParticles = _particlesAffected[j].particleSystem.GetParticles(_particles);

			// Change only the particles that are alive
			for (int i = 0; i < nParticles; i++)
			{
				windVal = GetWindAtPos( _particles[i].position.x, _particles[i].position.y );
				forceToAdd = windVal.GetAngle() * _particlesAffected[j].strength * windVal.GetStrength();
				_particles[i].velocity *= (1 - _particlesAffected[j].dampen);
				_particles[i].velocity += forceToAdd;
			}

			// Apply the particle changes to the Particle System
        	_particlesAffected[j].particleSystem.SetParticles(_particles, nParticles);
		}

		// debug draw ray
		Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), GetWindAtPos( transform.position.x, transform.position.y ).GetAngle());
	}

	WindValue GetWindAtPos(float x, float y)
	{
		float str = _strengthNoise.Length > 0 && _strTotalEnabled > 0 ? 0 : 1;
		for (int i = 0; i < _strengthNoise.Length; i++)
		{
			if (_strengthNoise[i].IsEnabled())
			{
				str += _strengthNoise[i].GetValueAtPos( x, y ) * (_strengthNoise[i].GetWeighting() / _strTotalWeigthing);
			}
		}

		float angleRad = 0;
		for (int j = 0; j < _angleNoise.Length; j++)
		{
			if (_angleNoise[j].IsEnabled())
			{
				angleRad += _angleNoise[j].GetValueAtPos( x, y ) * (_angleNoise[j].GetWeighting() / _angleTotalWeigthing);
			}
		}

		//angle = 2pi * ratio so as to hit angles like 2pi and -2pi without them being obviously at the extremes, it sorta looks like a ping pong between those values otherwise
		angleRad *= 2 * Mathf.PI * _angleRatio;
		Vector2 angleVect = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

		return new WindValue(angleVect , str);
	}

	void OnTriggerStay2D(Collider2D other)
    {
        if ( other.attachedRigidbody != null &&
			(_layersToAffect == (_layersToAffect | (1 << other.gameObject.layer))) )
        {
			WindValue windVal = GetWindAtPos( other.transform.position.x, other.transform.position.y );
           	other.attachedRigidbody.AddForce( windVal.GetAngle() * _rbStayForce * windVal.GetStrength() );
        }
    }
}
