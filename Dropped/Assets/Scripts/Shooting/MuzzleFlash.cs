using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour 
{
	public float framesOfFlash;
	float frameCounter;

	void Start()
	{
		frameCounter = 0;
	}

	void Update ()
	{
		if (frameCounter >= framesOfFlash)
			Destroy (gameObject);

		frameCounter += Time.deltaTime;
	}
}
