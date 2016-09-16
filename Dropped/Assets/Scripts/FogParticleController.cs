using UnityEngine;
using System.Collections;

/// <summary>
/// This controls the new fog objects particles. It removes them when they go outside the bounds 
/// of the fog, as well and determines how many particles to emit based on size of the bounds.
/// </summary>
public class FogParticleController : MonoBehaviour 
{
	public Collider2D collider;

	ParticleSystem particleSystem;
	ParticleSystem.Particle[] particles;

	void Start()
	{
		particleSystem = GetComponent<ParticleSystem> ();
		if(collider == null)
			collider = GetComponent<Collider2D> ();
	}

	void Update()
	{
		SyncEmissionShape ();
		SetEmissionRate ();
		RemoveParticles ();
	}
		
	/// <summary>
	/// Sets the rate of emmision for the particle system based on size of the are we're trying to render
	/// it to. This keeps it looking consistent no matter how large or small we make it in animations.
	/// We also set max particles here.
	/// </summary>
	void SetEmissionRate()
	{
		float emissionAreaSize = 0f;
		emissionAreaSize = particleSystem.shape.box.x * particleSystem.shape.box.y;

		ParticleSystem.EmissionModule emission = particleSystem.emission;
		ParticleSystem.MinMaxCurve rate = emission.rate;
		rate.constantMax = emissionAreaSize * 15f;
		rate.constantMin = emissionAreaSize * 15f;
		emission.rate = rate;

		//Replace constant float value here with a fog density float, stored in game manager.
		particleSystem.maxParticles = (int)(emission.rate.constantMax * 1f);
	}

	/// <summary>
	/// Syncs the emission shape to the box collider.
	/// </summary>
	void SyncEmissionShape()
	{
		ParticleSystem.ShapeModule shape = particleSystem.shape;
		shape.box = new Vector3 (collider.bounds.size.x * .85f, collider.bounds.size.y * .85f, collider.bounds.size.z * .85f);

		//ParticleSystemRenderer renderer = (ParticleSystemRenderer)particleSystem.GetComponent<Renderer> ();
		//renderer.pivot = (Vector3)collider.offset;
	}

	/// <summary>
	/// Removes out of bounds particles.
	/// </summary>
	/// TODO: Do this based on the particles billboard, not its center. This way there's no half-particle overflow.
	void RemoveParticles()
	{
		particles = new ParticleSystem.Particle[particleSystem.particleCount];
		int livingParticlesCount = particleSystem.GetParticles (particles);

		for (int i = 0; i < livingParticlesCount; i++)
		{
			if (particles [i].GetCurrentColor(particleSystem).a != 0f) 
			{
				float particleSize = particles [i].GetCurrentSize (particleSystem);
				//if (particles[i].position.x < -collider.bounds.extents.x || particles[i].position.x > collider.bounds.extents.x
					//|| particles[i].position.y < - collider.bounds.extents.y || particles[i].position.y > collider.bounds.extents.y)
				if(particles[i].position.x > collider.bounds.extents.x || particles[i].position.x < -collider.bounds.extents.x
					|| particles[i].position.y > collider.bounds.extents.y || particles[i].position.y < -collider.bounds.extents.y)
				{
					//Debug.Log (particles [i].position);
					Color newColor = particles [i].GetCurrentColor (particleSystem);
					newColor.a = 0f;
					particles [i].color = newColor;
				}
			}
		}

		particleSystem.SetParticles (particles, livingParticlesCount);
	}

	void OnDrawGizmos()
	{
		collider = GetComponent<Collider2D> ();
		particleSystem = GetComponent<ParticleSystem> ();

		SyncEmissionShape ();
		SetEmissionRate ();
		RemoveParticles ();
	}
}
