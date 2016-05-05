using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EscapeBar : MonoBehaviour 
{
	public Image escapeBarLeft;
	public Image escapeBarRight;

	public Color startColor;
	public Color endColor;

	Player player;

	void Start()
	{
		player = GameObject.Find ("Player").GetComponent<Player> ();
	}

	void Update()
	{
		float playerEscapePercentage = player.grappleEscapeAttempt / player.grappleStrength;
		playerEscapePercentage = Mathf.Clamp01 (playerEscapePercentage);
		Debug.Log (playerEscapePercentage);

		escapeBarRight.fillAmount = Mathf.Lerp (0f, 1f, playerEscapePercentage);
		escapeBarRight.color = Color.Lerp (endColor, startColor, playerEscapePercentage);

		escapeBarLeft.fillAmount = Mathf.Lerp (0f, 1f, playerEscapePercentage);
		escapeBarLeft.color = Color.Lerp (endColor, startColor, playerEscapePercentage);
	}
}
