using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedMaterial : MonoBehaviour 
{
	Material mat;
	int currentFrame = 0;
	public float framesPerSecond;
	public List<Texture> frames;

	public string sortingLayer;
	public int sortOrder;

	private bool coroutineRunning = false;

	float tilingFactor = 4f; //Damn it... I forgot what this does. I bet it's important though(?).

	public Color color;

	void Awake () 
	{
		mat = GetComponent<Renderer> ().material;
		mat.SetTexture ("_MainTex", frames [0]);

		GetComponent<Renderer> ().sortingLayerName = sortingLayer;
		GetComponent<Renderer> ().sortingOrder = sortOrder;
		//GetComponent<Renderer> ().material.color = color;
	}

	void Update () 
	{
		//Debug.Log (currentFrame);

		Vector2 texScale = new Vector2 (transform.localScale.x / tilingFactor, transform.localScale.y / tilingFactor);
		mat.SetTextureScale ("_MainTex", texScale);

		if (coroutineRunning == false) 
		{
			StartCoroutine (ChangeFrameAfterDelay (6f / framesPerSecond));
		}

		mat.SetTexture ("_MainTex", frames [currentFrame]);
	}

	private IEnumerator ChangeFrameAfterDelay(float delay)
	{
		coroutineRunning = true;
		yield return new WaitForSeconds (delay);
		mat.SetTexture ("_MainTex", frames [currentFrame]);
		if (currentFrame < frames.Count - 1)
			currentFrame++;
		else
			currentFrame = 0;
		coroutineRunning = false;
	}
}
