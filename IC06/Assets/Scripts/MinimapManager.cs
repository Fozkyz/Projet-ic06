using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
	[SerializeField] private RectTransform roomsHolder;
	[SerializeField] private RectTransform corridorsHolder;
	[SerializeField] private Sprite tileSprite;
	[SerializeField] private Sprite bossTileSprite;
	[SerializeField] private Sprite shopTileSprite;
	[SerializeField] private Sprite playerSprite;

	[SerializeField] private Color visitedColor;
	[SerializeField] private Color unvisitedColor;
	[SerializeField] private Color unkownColor;

	[SerializeField] private float tileScale;

	private RoomDirection[,] _dungeon;
	private Image[,] roomImages;

	private int leftMost, rightMost, topMost, botMost;
	private float size;

    public void GenerateMinimap(RoomDirection[,] dungeon)
	{
		_dungeon = dungeon;
		roomImages = new Image[dungeon.GetLength(0), dungeon.GetLength(1)];

		leftMost = FindLeftMost(dungeon);
		rightMost = FindRightMost(dungeon);
		topMost = FindTopMost(dungeon);
		botMost = FindBotMost(dungeon);

		size = Mathf.Min(roomsHolder.rect.height / (botMost - topMost) * tileScale / 2, roomsHolder.rect.width / (rightMost - leftMost) * tileScale / 2);

		for (int i = leftMost; i <= rightMost; i++)
		{
			for (int j = topMost; j <= botMost; j++)
			{
				if (dungeon[i, j] < RoomDirection.T)
				{
					continue;
				}

				GameObject tile = new GameObject();
				Image tileImage = tile.AddComponent<Image>();

				if (DungeonGenerator.IsNormalRoom(dungeon[i, j]))
				{
					tileImage.sprite = tileSprite;
				}
				else if (DungeonGenerator.IsBossRoom(dungeon[i, j]))
				{
					tileImage.sprite = bossTileSprite;
				}
				else if (DungeonGenerator.IsShopRoom(dungeon[i, j]))
				{
					tileImage.sprite = shopTileSprite;
				}
				tileImage.color = unkownColor;
				roomImages[i, j] = tileImage;

				RectTransform rect = tile.GetComponent<RectTransform>();
				rect.SetParent(roomsHolder);
				rect.localPosition = new Vector2((i - leftMost - (rightMost - leftMost) / 2) * size * 1.2f, (j - topMost - (botMost - topMost) / 2) * size * 1.2f);
				rect.sizeDelta = new Vector2(size, size);
				
				tile.SetActive(true);
			}
		}
	}

	public void OnPlayerEnteredRoomHandler(RoomManager rm)
	{
		Debug.Log("Player entered room");
		Vector2Int pos = rm.Position;
		int i = pos.x;
		int j = pos.y;
		Debug.Log("Test");
		if (roomImages[i, j].color == unvisitedColor || roomImages[i, j].color == unkownColor)
		{
			Debug.Log("techte");
			roomImages[i, j].color = visitedColor;

			if (DungeonGenerator.IsRight(_dungeon[i, j]) && roomImages[i + 1, j].color == unkownColor)
			{
				roomImages[i + 1, j].color = unvisitedColor;
				GenerateCorridor(((i - leftMost - (rightMost - leftMost) / 2) * size * 1.2f + (i + 1 - leftMost - (rightMost - leftMost) / 2) * size * 1.2f) / 2, (j - topMost - (botMost - topMost) / 2) * size * 1.2f);
			}
			if (DungeonGenerator.IsLeft(_dungeon[i, j]) && roomImages[i - 1, j].color == unkownColor)
			{
				roomImages[i - 1, j].color = unvisitedColor;
				GenerateCorridor(((i - leftMost - (rightMost - leftMost) / 2) * size * 1.2f + (i - 1 - leftMost - (rightMost - leftMost) / 2) * size * 1.2f) / 2, (j - topMost - (botMost - topMost) / 2) * size * 1.2f);
			}
			if (DungeonGenerator.IsTop(_dungeon[i, j]) && roomImages[i, j + 1].color == unkownColor)
			{
				roomImages[i, j + 1].color = unvisitedColor;
				GenerateCorridor((i - leftMost - (rightMost - leftMost) / 2) * size * 1.2f, ((j - topMost - (botMost - topMost) / 2) * size * 1.2f + (j + 1 - topMost - (botMost - topMost) / 2) * size * 1.2f) / 2);
			}
			if (DungeonGenerator.IsBot(_dungeon[i, j]) && roomImages[i, j - 1].color == unkownColor)
			{
				roomImages[i, j - 1].color = unvisitedColor;
				GenerateCorridor((i - leftMost - (rightMost - leftMost) / 2) * size * 1.2f, ((j - topMost - (botMost - topMost) / 2) * size * 1.2f + (j - 1 - topMost - (botMost - topMost) / 2) * size * 1.2f) / 2);
			}
		}
		rm.OnPlayerEnteredRoomEvent.RemoveListener(OnPlayerEnteredRoomHandler);
	}

	private void GenerateCorridor(float x, float y)
	{
		GameObject corridor = new GameObject();
		Image corridorImage = corridor.AddComponent<Image>();
		corridorImage.color = Color.gray;
		RectTransform corridorRect = corridor.GetComponent<RectTransform>();
		corridorRect.SetParent(corridorsHolder);
		corridorRect.localPosition = new Vector2(x, y);
		corridorRect.sizeDelta = new Vector2(size, size) * .7f;

		corridor.SetActive(true);
	}

	private int FindLeftMost(RoomDirection[,] dungeon)
	{
		for (int i = 0; i < dungeon.GetLength(0); i++)
		{
			for (int j = 0; j < dungeon.GetLength(1); j++)
			{
				if (dungeon[i, j] >= RoomDirection.T)
				{
					return i;
				}
			}
		}
		return 0;
	}

	private int FindRightMost(RoomDirection[,] dungeon)
	{
		for (int i = dungeon.GetLength(0) - 1; i >= 0; i--)
		{
			for (int j = 0; j < dungeon.GetLength(1); j++)
			{
				if (dungeon[i, j] >= RoomDirection.T)
				{
					return i;
				}
			}
		}
		return dungeon.GetLength(0) - 1;
	}

	private int FindTopMost(RoomDirection[,] dungeon)
	{
		for (int j = 0; j < dungeon.GetLength(1); j++)
		{
			for (int i = 0; i < dungeon.GetLength(0); i++)
			{
				if (dungeon[i, j] >= RoomDirection.T)
				{
					return j;
				}
			}
		}
		return 0;
	}

	private int FindBotMost(RoomDirection[,] dungeon)
	{
		for (int j = dungeon.GetLength(1) - 1; j >= 0; j--)
		{
			for (int i = 0; i < dungeon.GetLength(0); i++)
			{
				if (dungeon[i, j] >= RoomDirection.T)
				{
					return j;
				}
			}
		}
		return dungeon.GetLength(1) - 1;
	}
}
