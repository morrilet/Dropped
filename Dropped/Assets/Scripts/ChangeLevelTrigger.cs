using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeLevelTrigger : MonoBehaviour 
{
	public string levelToChangeTo;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			GameManager.Instance.ChangeLevel (levelToChangeTo);
		}
	}
}
