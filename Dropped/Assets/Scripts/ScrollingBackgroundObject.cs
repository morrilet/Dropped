using UnityEngine;
using System.Collections;

public class ScrollingBackgroundObject : MonoBehaviour 
{
	public float scrollSpeed;

	Camera mainCamera;

	Vector3 cameraPosition;
	Vector3 cameraPositionPrev;

	void Start()
	{
		mainCamera = Camera.main.GetComponent<Camera> ();
	}

	void Update()
	{
		cameraPosition = mainCamera.transform.position;

		float targetPositionX = cameraPosition.x - cameraPositionPrev.x;

		Vector3 newPos = transform.position;
		newPos.x -= targetPositionX * scrollSpeed;
		transform.position = newPos;

		cameraPositionPrev = cameraPosition;
	}
}
