using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ParticleSystem))]
public class ParticleSelfDestruct : MonoBehaviour 
{
	ParticleSystem particles;

	void Start () 
	{
		particles = GetComponent<ParticleSystem> ();
	}

	void Update () 
	{
		if (!particles.IsAlive ())
			Destroy (this.gameObject);
	}
}
