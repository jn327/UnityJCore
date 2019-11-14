using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour 
{
	public struct WindValue
	{
		private Vector2 _direction;
		public Vector2 direction {get { return _direction; } }
		private float _strength;
		public float strength {get { return _strength; } }

		public WindValue(Vector2 direction, float strength)
		{
			_direction = direction;
			_strength = strength;
		}
	}

	[System.Serializable]
	protected struct ParticleLayer
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

	[SerializeField]
	private LayerMask _layersToAffect = default;

	[SerializeField]
	private float _rbStayForce = 5.0f;
	
	[SerializeField] [EditableHeader(new string[] {"_type", "_scale", "_changeSpeed", "_bEnabled"})]
	private WindNoiseLayer[] _strengthNoise = default;
	private float _strVal;
	[SerializeField] [EditableHeader(new string[] {"_type", "_scale", "_changeSpeed", "_bEnabled"})]
	private WindNoiseLayer[] _angleNoise = default;
	
	private float _angleVal;
	[SerializeField]
	private float _curlStep = 0.5f;

	[SerializeField]
	[Range(1,6)]
	//ratio between the noise value and the amount of radians the resulting vector will be rotated by,
	// this obfuscates the otherwise slightly obvious extremes that the wind can be rotated by.
	private float _angleRatio = 4;

	[SerializeField] [EditableHeader(new string[] {"particleSystem"})]
	private List<ParticleLayer> _particlesAffected = default;
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
				forceToAdd = windVal.direction * _particlesAffected[j].strength * windVal.strength;
				_particles[i].velocity *= (1 - _particlesAffected[j].dampen);
				_particles[i].velocity += forceToAdd;
			}

			// Apply the particle changes to the Particle System
        	_particlesAffected[j].particleSystem.SetParticles(_particles, nParticles);
		}

		// debug draw ray
		//Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), GetWindAtPos( transform.position.x, transform.position.y ).GetDirection());
	}

	WindValue GetWindAtPos(float x, float y)
	{
		_strVal = _strengthNoise.Length > 0 ? 0 : 1;
		for (int i = 0; i < _strengthNoise.Length; i++)
		{
			if (_strengthNoise[i].IsEnabled())
			{
				_strVal = _strengthNoise[i].GetValueAtPos( _strVal, x, y );
			}
		}
		
		Vector2 angleVector;
		if (_curlStep > 0)
		{
			angleVector = CurlNoiseUtil.CurlNoise(getAngleAtPos, x, y, _curlStep);
		}
		else
		{
			angleVector = getAngleAtPos(x, y);
		}

		return new WindValue(angleVector , _strVal);
	}

	private Vector2 getAngleAtPos(float x, float y)
	{
		_angleVal = 0;
		for (int j = 0; j < _angleNoise.Length; j++)
		{
			if (_angleNoise[j].IsEnabled())
			{
				_angleVal = _angleNoise[j].GetValueAtPos( _angleVal, x, y );
			}
		}

		//angle = 2pi * ratio so as to hit angles like 2pi and -2pi without them being obviously at the extremes, it sorta looks like a ping pong between those values otherwise
		_angleVal *= 2 * Mathf.PI * _angleRatio;
		return new Vector2(Mathf.Cos(_angleVal), Mathf.Sin(_angleVal));
	}

	void OnTriggerStay2D(Collider2D other)
    {
		IWindAffected windAffected = other.GetComponent<IWindAffected>();
		if (windAffected != null)
		{
			windAffected.onWindUpdate( GetWindAtPos( other.transform.position.x, other.transform.position.y ) );
		}
		else
		{
			if ( other.attachedRigidbody != null &&
				(_layersToAffect == (_layersToAffect | (1 << other.gameObject.layer))) )
			{
				WindValue windVal = GetWindAtPos( other.transform.position.x, other.transform.position.y );
				other.attachedRigidbody.AddForce( windVal.direction * _rbStayForce * windVal.strength );
			}
		}
    }
}
