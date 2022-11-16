using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private PlayerController player;

	private Vector2 startPos;

	public void ResetPlayerPosition()
	{
		player.transform.position = startPos;
	}

	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	private void Start()
	{
		if (player != null)
		{
			startPos = player.transform.position;
		}
	}
}
