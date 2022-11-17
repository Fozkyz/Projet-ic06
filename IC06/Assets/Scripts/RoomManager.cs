using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	[SerializeField] private List<Enemy> _roomEnemies;

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

		SetActiveRoom(false);
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
