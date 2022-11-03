using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private PlayerController player;

	private Vector2 startPos;

	public void ResetPlayerPosition()
	{
		player.transform.position = startPos;
	}

	private void Start()
	{
		startPos = player.transform.position;
	}
}
