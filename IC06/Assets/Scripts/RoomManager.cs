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
			if (enemy != null)
			{
				enemy.SetActive(_isActiveRoom);
			}
		}
	}

	protected virtual void Start()
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

	protected virtual void OnPlayerEnteredRoomHandler(PlayerController player, Teleporter teleporter)
	{
		SetActiveRoom(true);
	}

	protected virtual void OnPlayerLeftRoomHandler(PlayerController player, Teleporter teleporter)
	{
		SetActiveRoom(false);
	}
}
