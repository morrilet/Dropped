using UnityEngine;
using System.Collections;

/// <summary>
/// This script is meant to control the density of a particle system using its emission rate and max particles,
/// based on the area that the system is trying to fill. This script does not require the particle system to
/// have an attached collider, instead relying on the shape of the system to determine its area.
/// 
/// ParticleDensityController requires a particle system on the gameobject to function.
/// 
/// ==================================================NOTE==================================================
/// This currently only works with the particle systems emitting from a box shape. Will update if needed.
/// ========================================================================================================
/// </summary>
[RequireComponent (typeof (ParticleSystem))]
public class ParticleDensityController : MonoBehaviour 
{
	/// <summary>
	/// A 0-1 value for modifying the density of fog 
	/// within its given area. 0 = min, 1 = max.
	/// </summary>
	public float density;

	ParticleSystem particleSystem;
	ParticleSystem.Particle[] particles;

	void Start () 
	{
		particleSystem = GetComponent<ParticleSystem> ();
	}

	void Update () 
	{
		Mathf.Clamp01 (density);

		SetEmissionRate ();
	}

	//Something to consider could be making a fog density option in the game manager. Less pertinent now 
	//that we're using a hybrid fog system, but still an option to consider.
	void SetEmissionRate()
	{
		float areaToFill = particleSystem.shape.box.x * particleSystem.shape.box.y;

		ParticleSystem.EmissionModule emission = particleSystem.emission;
		ParticleSystem.MinMaxCurve rate = emission.rate;

		//25 is an arbitrary number meant to modify the 0-1 value of density.
		rate.constantMax = areaToFill * 25f * density;
		rate.constantMin = areaToFill * 25f * density;

		emission.rate = rate;

		particleSystem.maxParticles = (int)(emission.rate.constantMax * density);
	}
}
