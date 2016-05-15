using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EscapeBar : MonoBehaviour 
{
	public Image escapeBarLeft;
	public Image escapeBarRight;

	public Color startColor;
	public Color endColor;

	RectTransform rectTransform;
	Vector3 startingPosition;
	Quaternion startingRotation;

	Player player;

	bool isActive;

	void Start()
	{
		player = GameObject.Find ("Player").GetComponent<Player> ();

		rectTransform = GetComponent<RectTransform> ();
		startingPosition = rectTransform.position;
		startingRotation = rectTransform.rotation;

		isActive = false;
	}

	void Update()
	{
		if (isActive)
			FillBar ();
		else 
		{
			rectTransform.position = startingPosition;
			rectTransform.rotation = startingRotation;
		}
	}

	void FillBar()
	{
		float playerEscapePercentage = player.grappleEscapeAttempt / player.grappleStrength;
		playerEscapePercentage = Mathf.Clamp01 (playerEscapePercentage);
		//Debug.Log (playerEscapePercentage);

		escapeBarRight.fillAmount = Mathf.Lerp (0f, 1f, playerEscapePercentage);
		escapeBarRight.color = Color.Lerp (endColor, startColor, playerEscapePercentage);

		escapeBarLeft.fillAmount = Mathf.Lerp (0f, 1f, playerEscapePercentage);
		escapeBarLeft.color = Color.Lerp (endColor, startColor, playerEscapePercentage);
	}

	public void SetBarActive(bool active)
	{
		isActive = active;

		if (active)
			StartCoroutine ("ShakeBar");
		else
			StopCoroutine ("ShakeBar");

		for (int i = 0; i < transform.childCount; i++) 
		{
			rectTransform.GetChild (i).gameObject.SetActive (active);
		}
	}

	IEnumerator ShakeBar()
	{
		rectTransform.position = startingPosition;
		rectTransform.rotation = startingRotation;

		float playerEscapePercentage = player.grappleEscapeAttempt / player.grappleStrength;
		playerEscapePercentage = Mathf.Clamp01 (playerEscapePercentage);

		float posModifier = 10f;
		float rotModifier = 2.5f;

		Vector3 newPos = new Vector3 (Random.Range (-playerEscapePercentage * posModifier, playerEscapePercentage * posModifier),
			                 Random.Range (-playerEscapePercentage * posModifier, playerEscapePercentage * posModifier), 0f);
		Vector3 newRot = new Vector3 (0f, 0f, Random.Range (-playerEscapePercentage * rotModifier, playerEscapePercentage * rotModifier));

		rectTransform.position += newPos;
		rectTransform.rotation = Quaternion.Euler (newRot);
		//Debug.Log (rectTransform.position);

		yield return null;
	}
}
