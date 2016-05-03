using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeLevelTrigger : MonoBehaviour 
{
	GameObject gameManager;
	public string levelToChangeTo;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			GameManager.instance.ChangeLevel (levelToChangeTo);
		}
	}
}
