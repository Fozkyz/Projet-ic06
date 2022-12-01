using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{
	[SerializeField] private Vector2 size;
	[SerializeField] GameObject roomPrefab;
	[SerializeField] RoomDirection roomDirection;

	public Vector2 GetSize()
	{
		return size;
	}

	public RoomDirection GetRoomDirection()
	{
		return roomDirection;
	}

	public TeleporterManager Spawn(Vector2Int position, GameObject go)
	{
		Vector2 pos = position;
		pos.x *= size.x;
		pos.y *= size.y;
		GameObject roomGO = Instantiate(roomPrefab, pos, Quaternion.identity, go.transform);
		return roomGO.transform.GetComponentInChildren<TeleporterManager>();
	}
}
