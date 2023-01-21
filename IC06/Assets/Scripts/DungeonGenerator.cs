using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[SerializeField] private GameObject grid;
	[SerializeField] private Vector2Int dungeonSize;
	[SerializeField] private int roomCount;
	[SerializeField] private int shopRoomCount;

	[SerializeField] List<Room> startingRoomPool;
	[SerializeField] List<Room> roomPool;
	[SerializeField] List<Room> shopRoomPool;
	[SerializeField] List<Room> bossRoomPool;
	[SerializeField] List<Sprite> backgroundImages;

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

	public void GenerateSpecialRooms()
	{
		List<Vector2Int> possibleRooms = GetTerminalRooms();
		if (possibleRooms.Count > 0)
		{
			Vector2Int bossRoomPos = possibleRooms[Random.Range(0, possibleRooms.Count)];
			if (dungeon[bossRoomPos.x + 1, bossRoomPos.y] > RoomDirection.ND)
			{
				dungeon[bossRoomPos.x, bossRoomPos.y] = RoomDirection.BOSSR;
				dungeon[bossRoomPos.x + 1, bossRoomPos.y] += 4;
			}
			else
			{
				dungeon[bossRoomPos.x, bossRoomPos.y] = RoomDirection.BOSSL;
				dungeon[bossRoomPos.x - 1, bossRoomPos.y] += 8;
			}
			possibleRooms.Remove(bossRoomPos);
		}
		for (int shop = 0; shop < shopRoomCount; shop++)
		{
			if (possibleRooms.Count > 0)
			{
				Vector2Int shopRoomPos = possibleRooms[Random.Range(0, possibleRooms.Count)];
				if (dungeon[shopRoomPos.x + 1, shopRoomPos.y] > RoomDirection.ND)
				{
					dungeon[shopRoomPos.x, shopRoomPos.y] = RoomDirection.SHOPR;
					dungeon[shopRoomPos.x + 1, shopRoomPos.y] += 4;
				}
				else
				{
					dungeon[shopRoomPos.x, shopRoomPos.y] = RoomDirection.SHOPL;
					dungeon[shopRoomPos.x - 1, shopRoomPos.y] += 8;
				}
				possibleRooms.Remove(shopRoomPos);
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
					if (i == dungeonSize.x / 2 && j == dungeonSize.y / 2)
					{
						Vector2Int t = new Vector2Int(i, j);
						List<Room> possibleRooms = GetRoomsByDirection(startingRoomPool, room);
						if (possibleRooms.Count > 0)
						{
							//TeleporterManager tpManager = startingRoomPool[(int)room - 1].Spawn(new Vector2Int(i - dungeonSize.x / 2, j - dungeonSize.y / 2), grid);
							TeleporterManager tpManager = possibleRooms[Random.Range(0, possibleRooms.Count)].Spawn(new Vector2Int(i - dungeonSize.x / 2, j - dungeonSize.y / 2), grid);
							teleporterManagers[t] = tpManager;
						}
					}
					else
					{
						Vector2Int t = new Vector2Int(i, j);
						if (room == RoomDirection.BOSSL || room == RoomDirection.BOSSR)
						{
							// Spawn boss room
							List<Room> possibleRooms = GetRoomsByDirection(bossRoomPool, room);
							if (possibleRooms.Count > 0)
							{
								TeleporterManager tpManager = possibleRooms[Random.Range(0, possibleRooms.Count)].Spawn(new Vector2Int(i - dungeonSize.x / 2, j - dungeonSize.y / 2), grid);
								teleporterManagers[t] = tpManager;
							}
						}
						else if (room == RoomDirection.SHOPL || room == RoomDirection.SHOPR)
						{
							// Spawn shop room
							List<Room> possibleRooms = GetRoomsByDirection(shopRoomPool, room);
							if (possibleRooms.Count > 0)
							{							
								TeleporterManager tpManager = possibleRooms[Random.Range(0, possibleRooms.Count)].Spawn(new Vector2Int(i - dungeonSize.x / 2, j - dungeonSize.y / 2), grid);
								teleporterManagers[t] = tpManager;
								ShopManager shopManager = tpManager.GetComponent<ShopManager>();
								if (shopManager != null)
								{
									shopManager.InitShop();
								}
							}
						}
						else
						{
							List<Room> possibleRooms = GetRoomsByDirection(roomPool, room);
							if (possibleRooms.Count > 0)
							{
								//TeleporterManager tpManager = roomPool[(int)room - 1].Spawn(new Vector2Int(i - dungeonSize.x / 2, j - dungeonSize.y / 2), grid);
								TeleporterManager tpManager = possibleRooms[Random.Range(0, possibleRooms.Count)].Spawn(new Vector2Int(i - dungeonSize.x / 2, j - dungeonSize.y / 2), grid);
								teleporterManagers[t] = tpManager;
							}
						}
					}
				}
			}
		}
	}

	private List<Vector2Int> GetTerminalRooms()
	{
		List<Vector2Int> terminalRooms = new List<Vector2Int>();
		for (int i = 0; i < dungeonSize.x; i++)
		{
			for (int j = 0; j < dungeonSize.y; j++)
			{
				RoomDirection room = dungeon[i, j];
				if (room == RoomDirection.NULL)
				{
					if (i < dungeonSize.x - 1 && IsRoomTerminal(i, j, 1) && dungeon[i+1, j] > RoomDirection.ND || i > 0 && IsRoomTerminal(i, j, -1) && dungeon[i - 1, j] > RoomDirection.ND)
					{
						terminalRooms.Add(new Vector2Int(i, j));
					}
				}
			}
		}

		return terminalRooms;
	}

	private bool IsRoomTerminal(int i, int j, int side)
	{
		int iMin = Mathf.Max(i - 1, 0);
		int iMax = Mathf.Min(i + 1, dungeonSize.x - 1);
		int jMin = Mathf.Max(j - 1, 0);
		int jMax = Mathf.Min(j + 1, dungeonSize.y - 1);

		for (int ii = iMin; ii < iMax; ii++)
		{
			for (int jj = jMin; jj < jMax; jj++)
			{
				if (ii != i + side && dungeon[ii, jj] > RoomDirection.ND)
				{
					return false;
				}
			}
		}
		return true;
	}

	private List<Room> GetRoomsByDirection(List<Room> possibleRooms, RoomDirection direction)
	{
		List<Room> rooms = new List<Room>();
		foreach (Room room in possibleRooms)
		{
			if (room.GetRoomDirection() == direction)
			{
				rooms.Add(room);
			}
		}
		return rooms;
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

	public void AddBackgroundImages()
	{
		for (int i = 0; i < dungeonSize.x; i++)
		{
			for (int j = 0; j < dungeonSize.y; j++)
			{
				RoomDirection room = dungeon[i, j];
				if (room != RoomDirection.NULL && room > RoomDirection.ND)
				{
					TeleporterManager tpManager = teleporterManagers[new Vector2Int(i, j)];
					tpManager.SetBackgroundImage(backgroundImages[Random.Range(0, backgroundImages.Count)]);
				}
			}
		}
	}

	private void Start()
	{
		GameObject go = GameObject.Find("Dungeon");
		if (go != null)
		{
			Destroy(go);
		}

		GenerateDungeon(roomCount);
		GenerateSpecialRooms();
		BuildDungeon();
		LinkPortals();
		AddBackgroundImages();

		GetComponent<MinimapManager>().GenerateMinimap(dungeon);

		Invoke(nameof(SubscribeMinimapManagerToEvents), .5f);
	}

	private void SetFirstRoomActive()
	{
		RoomManager rm = teleporterManagers[new Vector2Int(dungeonSize.x / 2, dungeonSize.y / 2)].GetComponent<RoomManager>();
		rm.SetActiveRoom(true);
	}

	private void SubscribeMinimapManagerToEvents()
	{
		MinimapManager minimapManager = GetComponent<MinimapManager>();
		if (minimapManager != null)
		{
			foreach (KeyValuePair<Vector2Int, TeleporterManager> tpManager in teleporterManagers)
			{
				RoomManager rm = tpManager.Value.GetComponent<RoomManager>();
				if (rm != null)
				{
					rm.OnPlayerEnteredRoomEvent = new UnityEvent<RoomManager>();
					rm.Position = tpManager.Key;
					rm.OnPlayerEnteredRoomEvent.AddListener(minimapManager.OnPlayerEnteredRoomHandler);
				}
			}
		}
		SetFirstRoomActive();
	}

	public static bool IsTop(RoomDirection room)
	{
		return (room == RoomDirection.T || room == RoomDirection.TB || room == RoomDirection.TL || 
			room == RoomDirection.TBL || room == RoomDirection.TR || room == RoomDirection.TBR || 
			room == RoomDirection.TLR || room == RoomDirection.TBLR);
	}

	public static bool IsBot(RoomDirection room)
	{
		return (room == RoomDirection.B || room == RoomDirection.TB || room == RoomDirection.BL || 
			room == RoomDirection.TBL || room == RoomDirection.BR || room == RoomDirection.TBR || 
			room == RoomDirection.BLR || room == RoomDirection.TBLR);
	}

	public static bool IsLeft(RoomDirection room)
	{
		return (room == RoomDirection.L || room == RoomDirection.TL || room == RoomDirection.BL || 
			room == RoomDirection.TBL || room == RoomDirection.LR || room == RoomDirection.TLR || 
			room == RoomDirection.BLR || room == RoomDirection.TBLR || room == RoomDirection.BOSSL ||
			room == RoomDirection.SHOPL);
	}

	public static bool IsRight(RoomDirection room)
	{
		return (room == RoomDirection.R || room == RoomDirection.TR || room == RoomDirection.BR ||
			room == RoomDirection.TBR || room == RoomDirection.LR || room == RoomDirection.TLR || 
			room == RoomDirection.BLR || room == RoomDirection.TBLR || room == RoomDirection.BOSSR ||
			room == RoomDirection.SHOPR);
	}

	public static bool IsNormalRoom(RoomDirection room)
	{
		return (room >= RoomDirection.T && room <= RoomDirection.TBLR);
	}

	public static bool IsBossRoom(RoomDirection room)
	{
		return (room == RoomDirection.BOSSL || room == RoomDirection.BOSSR);
	}

	public static bool IsShopRoom(RoomDirection room)
	{
		return (room == RoomDirection.SHOPL || room == RoomDirection.SHOPR);
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
	BOSSL = 16,
	BOSSR = 17,
	SHOPL = 18,
	SHOPR = 19
}