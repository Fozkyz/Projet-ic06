using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[SerializeField] private GameObject grid;
	[SerializeField] private Vector2Int dungeonSize;
	[SerializeField] private int roomCount;

	[SerializeField] List<Room> roomPool;

	private RoomDirection[,] dungeon;
	private Dictionary<Vector2Int, TeleporterManager> teleporterManagers;

	public void GenerateDungeon(int n)
	{
		dungeon = new RoomDirection[dungeonSize.x, dungeonSize.y];
		for (int i = 0; i < dungeonSize.x; i++)
		{
			for (int j = 0; j < dungeonSize.y; j++)
			{
				dungeon[i, j] = RoomDirection.NULL;
			}
		}

		List<Vector2Int> neighbourRooms = new List<Vector2Int>();

		Vector2Int position = new Vector2Int(dungeonSize.x / 2, dungeonSize.y / 2);
		dungeon[position.x, position.y] = RoomDirection.ND;
		neighbourRooms.Add(position + new Vector2Int(-1, 0));
		neighbourRooms.Add(position + new Vector2Int(1, 0));
		neighbourRooms.Add(position + new Vector2Int(0, -1));
		neighbourRooms.Add(position + new Vector2Int(0, 1));

		int cnt = 0;
		while (cnt < n)
		{
			cnt++;
			Vector2Int newRoomPos = neighbourRooms[Random.Range(0, neighbourRooms.Count)];
			neighbourRooms.RemoveAll(room => room == newRoomPos);
			dungeon[newRoomPos.x, newRoomPos.y] = RoomDirection.ND;
			if (newRoomPos.x > 0)
			{
				if (dungeon[newRoomPos.x - 1, newRoomPos.y] == RoomDirection.NULL)
				{
					neighbourRooms.Add(newRoomPos + new Vector2Int(-1, 0));
				}
				else
				{
					dungeon[newRoomPos.x - 1, newRoomPos.y] += 8;
					dungeon[newRoomPos.x, newRoomPos.y] += 4;
				}
			}
			if (newRoomPos.x < dungeonSize.x - 1)
			{
				if (dungeon[newRoomPos.x + 1, newRoomPos.y] == RoomDirection.NULL)
				{
					neighbourRooms.Add(newRoomPos + new Vector2Int(1, 0));
				}
				else
				{
					dungeon[newRoomPos.x + 1, newRoomPos.y] += 4;
					dungeon[newRoomPos.x, newRoomPos.y] += 8;
				}
			}
			if (newRoomPos.y > 0)
			{
				if (dungeon[newRoomPos.x, newRoomPos.y - 1] == RoomDirection.NULL)
				{
					neighbourRooms.Add(newRoomPos + new Vector2Int(0, -1));
				}
				else
				{
					dungeon[newRoomPos.x, newRoomPos.y - 1] += 1;
					dungeon[newRoomPos.x, newRoomPos.y] += 2;
				}
			}
			if (newRoomPos.y < dungeonSize.y)
			{
				if (dungeon[newRoomPos.x, newRoomPos.y + 1] == RoomDirection.NULL)
				{
					neighbourRooms.Add(newRoomPos + new Vector2Int(0, 1));
				}
				else
				{
					dungeon[newRoomPos.x, newRoomPos.y + 1] += 2;
					dungeon[newRoomPos.x, newRoomPos.y] += 1;
				}
			}
		}
	}

	public void BuildDungeon()
	{
		teleporterManagers = new Dictionary<Vector2Int, TeleporterManager>();
		GameObject holder = new GameObject();
		holder.name = "Dungeon";
		for (int i = 0; i < dungeonSize.x; i++)
		{
			for (int j = 0; j < dungeonSize.y; j++)
			{
				RoomDirection room = dungeon[i, j];
				if(room != RoomDirection.NULL && room > RoomDirection.ND)
				{
					Vector2Int t = new Vector2Int(i, j);
					TeleporterManager tpManager = roomPool[(int)room - 1].Spawn(new Vector2Int(i - dungeonSize.x / 2, j - dungeonSize.y / 2), grid);
					teleporterManagers[t] = tpManager;
				}
			}
		}
	}

	public void LinkPortals()
	{
		for (int i = 0; i < dungeonSize.x; i++)
		{
			for (int j = 0; j < dungeonSize.y; j++)
			{
				RoomDirection room = dungeon[i, j];
				if (room != RoomDirection.NULL && room > RoomDirection.ND)
				{
					TeleporterManager tpManager = teleporterManagers[new Vector2Int(i, j)];
					if (IsTop(room))
					{
						tpManager.GetTopTeleporter().SetLinkedTeleporter(teleporterManagers[new Vector2Int(i, j + 1)].GetBotTeleporter());
						teleporterManagers[new Vector2Int(i, j + 1)].GetBotTeleporter().SetLinkedTeleporter(tpManager.GetTopTeleporter());
					}
					if (IsBot(room))
					{
						tpManager.GetBotTeleporter().SetLinkedTeleporter(teleporterManagers[new Vector2Int(i, j - 1)].GetTopTeleporter());
						teleporterManagers[new Vector2Int(i, j - 1)].GetTopTeleporter().SetLinkedTeleporter(tpManager.GetBotTeleporter());
					}
					if (IsLeft(room))
					{
						tpManager.GetLeftTeleporter().SetLinkedTeleporter(teleporterManagers[new Vector2Int(i - 1, j)].GetRightTeleporter());
						teleporterManagers[new Vector2Int(i - 1, j)].GetRightTeleporter().SetLinkedTeleporter(tpManager.GetLeftTeleporter());
					}
					if (IsRight(room))
					{
						tpManager.GetRightTeleporter().SetLinkedTeleporter(teleporterManagers[new Vector2Int(i + 1, j)].GetLeftTeleporter());
						teleporterManagers[new Vector2Int(i + 1, j)].GetLeftTeleporter().SetLinkedTeleporter(tpManager.GetRightTeleporter());
					}
				}
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			GameObject go = GameObject.Find("Dungeon");
			if (go != null)
			{
				Destroy(go);
			}

			GenerateDungeon(roomCount);
			BuildDungeon();
			LinkPortals();
		}
	}

	private bool IsTop(RoomDirection room)
	{
		return (room == RoomDirection.T || room == RoomDirection.TB || room == RoomDirection.TL || room == RoomDirection.TBL || room == RoomDirection.TR || room == RoomDirection.TBR || room == RoomDirection.TLR || room == RoomDirection.TBLR);
	}

	private bool IsBot(RoomDirection room)
	{
		return (room == RoomDirection.B || room == RoomDirection.TB || room == RoomDirection.BL || room == RoomDirection.TBL || room == RoomDirection.BR || room == RoomDirection.TBR || room == RoomDirection.BLR || room == RoomDirection.TBLR);
	}

	private bool IsLeft(RoomDirection room)
	{
		return (room == RoomDirection.L || room == RoomDirection.TL || room == RoomDirection.BL || room == RoomDirection.TBL || room == RoomDirection.LR || room == RoomDirection.TLR || room == RoomDirection.BLR || room == RoomDirection.TBLR);
	}

	private bool IsRight(RoomDirection room)
	{
		return (room == RoomDirection.R || room == RoomDirection.TR || room == RoomDirection.BR || room == RoomDirection.TBR || room == RoomDirection.LR || room == RoomDirection.TLR || room == RoomDirection.BLR || room == RoomDirection.TBLR);
	}


}

public enum RoomDirection
{
	NULL = -1,
	ND = 0,
	T = 1,
	B = 2,
	TB = 3,
	L = 4,
	TL = 5,
	BL = 6,
	TBL = 7,
	R = 8,
	TR = 9,
	BR = 10,
	TBR = 11,
	LR = 12,
	TLR = 13,
	BLR = 14,
	TBLR = 15,
}