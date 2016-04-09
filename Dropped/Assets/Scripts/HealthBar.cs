using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour 
{
	public Image healthBar;
	public Color startColor;
	public Color endColor;

	Player player;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}

	void Update()
	{
		float playerHealthPercentage = player.health / player.maxHealth;

		healthBar.fillAmount = Mathf.Lerp (0f, 1f, playerHealthPercentage);
		healthBar.color = Color.Lerp (endColor, startColor, playerHealthPercentage);
	}
}
