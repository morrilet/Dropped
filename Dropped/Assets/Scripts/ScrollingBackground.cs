using UnityEngine;
using System.Collections;

public class ScrollingBackground : MonoBehaviour 
{
	public GameObject backgroundTile1;
	public GameObject backgroundTile2;
	public float scrollSpeed;

	Vector2 cameraExtents;  //Extents of the main camera.
	Vector3 backgroundSize; //Size of the background images.

	Vector3 left, right, center; //Left and right positions to scroll between, plus center to start at.

	Vector3 cameraPosition;
	Vector3 cameraPositionPrev;

	Camera mainCamera;

	public bool scrollOnAwake;

	bool tilesInStartingPlaces;
	float tilesInStartingPlacesTime;
	float tilesInStartingPlacesCount;

	void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		//Get camera extents.
		cameraExtents.x = mainCamera.orthographicSize * Screen.width / Screen.height;
		cameraExtents.y = mainCamera.orthographicSize;

		//Get background size.
		backgroundSize = backgroundTile1.GetComponent<SpriteRenderer>().bounds.size;

		//Get left and right positions. These will also need to be updated as camera changes position.
		//.125f is arbirtary; it allows us to remove any gaps by pushing the tiles together by .25f.
		left = new Vector3 (mainCamera.transform.position.x - backgroundSize.x * 1.5f + .125f, transform.position.y, transform.position.z);
		right = new Vector3 (mainCamera.transform.position.x + backgroundSize.x * 1.5f - .125f, transform.position.y, transform.position.z);
		center = new Vector3 (mainCamera.transform.position.x, transform.position.y, transform.position.z);

		Debug.Log (left + ", " + right);

		//Set the initial positions of the background tiles.
		backgroundTile1.transform.position = right;
		backgroundTile2.transform.position = center;

		//Set the height of the backgrounds to fit the camera.
		//In the future, adjust the entire ratio of the image perhaps?
		Vector3 newLocalScale = backgroundTile1.transform.localScale;
		newLocalScale.y = (cameraExtents.y * 2f) / backgroundSize.y;
		backgroundTile1.transform.localScale = newLocalScale;
		backgroundTile2.transform.localScale = newLocalScale;

		tilesInStartingPlaces = false;
		tilesInStartingPlacesTime = .5f;
	}

	void Update()
	{
		cameraPosition = mainCamera.transform.position;

		//Update the left and right positions as the camera changes position.
		left = new Vector3 (mainCamera.transform.position.x - backgroundSize.x * 1.5f + .125f, transform.position.y, transform.position.z);
		right = new Vector3 (mainCamera.transform.position.x + backgroundSize.x * 1.5f - .125f, transform.position.y, transform.position.z);
		center = new Vector3 (mainCamera.transform.position.x, transform.position.y, transform.position.z);

		//Scroll the backgrounds.
		ScrollBackground (backgroundTile1);
		ScrollBackground (backgroundTile2);

		if (!tilesInStartingPlaces) 
		{
			if(tilesInStartingPlacesCount >= tilesInStartingPlacesTime)
				tilesInStartingPlaces = true;

			backgroundTile1.transform.position = right;
			backgroundTile2.transform.position = center;
			tilesInStartingPlacesCount += Time.deltaTime;
		}

		cameraPositionPrev = cameraPosition;
	}

	void ScrollBackground(GameObject bg)
	{
		if (bg.transform.position.x - (backgroundSize.x / 2f) < left.x) 
		{
			//Debug.Log ("Left " + bg.name.ToString() + " " + bg.transform.position);
			Vector3 pos = bg.transform.position;
			pos.x = right.x - (backgroundSize.x / 2f);
			bg.transform.position = pos;
		}
		else if (bg.transform.position.x + (backgroundSize.x / 2f) > right.x) 
		{
			//Debug.Log ("Right " + bg.name.ToString() + " " + bg.transform.position);
			Vector3 pos = bg.transform.position;
			pos.x = left.x + (backgroundSize.x / 2f);
			bg.transform.position = pos;
		}

		Vector3 position = bg.transform.position;
		if (!scrollOnAwake)
			position.x -= (cameraPosition.x - cameraPositionPrev.x) * scrollSpeed;
		else
			position.x -= Time.deltaTime * scrollSpeed;
		position.z = transform.position.z;
		bg.transform.position = position;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (new Vector3(left.x, -5, 0), new Vector3(left.x, 5, 0));
		Gizmos.DrawLine (new Vector3(right.x, -5, 0), new Vector3(right.x, 5, 0));
	}
}
