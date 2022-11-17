using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	private List<Enemy> _roomEnemies;

	private bool _isActiveRoom;

	public void SetActiveRoom(bool newIsActive)
	{
		_isActiveRoom = newIsActive;
		foreach (Enemy enemy in _roomEnemies)
		{
			enemy.SetActive(_isActiveRoom);
		}
	}

	private void Start()
	{
		TeleporterManager tpManager = GetComponent<TeleporterManager>();
		tpManager.OnPlayerEnteredRoomEvent.AddListener(OnPlayerEnteredRoomHandler);
		tpManager.OnPlayerLeftRoomEvent.AddListener(OnPlayerLeftRoomHandler);

		_roomEnemies = new List<Enemy>();
		foreach (Enemy enemy in transform.parent.GetComponentsInChildren<Enemy>())
		{
			_roomEnemies.Add(enemy);
		}

		//SetActiveRoom(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			SetActiveRoom(false);
		}
	}

	private void OnPlayerEnteredRoomHandler(PlayerController player, Teleporter teleporter)
	{
		SetActiveRoom(true);
	}

	private void OnPlayerLeftRoomHandler(PlayerController player, Teleporter teleporter)
	{
		SetActiveRoom(false);
	}
}
