using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
	[SerializeField] private SpriteRenderer background;
	[SerializeField] private Teleporter leftTeleporter;
	[SerializeField] private Teleporter rightTeleporter;
	[SerializeField] private Teleporter topTeleporter;
	[SerializeField] private Teleporter bottomTeleporter;

	public UnityEvent<PlayerController, Teleporter> OnPlayerEnteredRoomEvent;
	public UnityEvent<PlayerController, Teleporter> OnPlayerLeftRoomEvent;

	public void LockAllTeleporters()
	{
		if (leftTeleporter != null)
		{
			leftTeleporter.IsTpLocked = true;
		}
		if (rightTeleporter != null)
		{
			rightTeleporter.IsTpLocked = true;
		}
		if (topTeleporter != null)
		{
			topTeleporter.IsTpLocked = true;
		}
		if (bottomTeleporter != null)
		{
			bottomTeleporter.IsTpLocked = true;
		}
	}

	public void UnlockAllTeleporters()
	{
		if (leftTeleporter != null)
		{
			leftTeleporter.IsTpLocked = false;
		}
		if (rightTeleporter != null)
		{
			rightTeleporter.IsTpLocked = false;
		}
		if (topTeleporter != null)
		{
			topTeleporter.IsTpLocked = false;
		}
		if (bottomTeleporter != null)
		{
			bottomTeleporter.IsTpLocked = false;
		}
	}

	public Teleporter GetLeftTeleporter()
	{
		return leftTeleporter;
	}

	public Teleporter GetRightTeleporter()
	{
		return rightTeleporter;
	}

	public Teleporter GetTopTeleporter()
	{
		return topTeleporter;
	}
	
	public Teleporter GetBotTeleporter()
	{
		return bottomTeleporter;
	}

	public void SetBackgroundImage(Sprite newSprite)
	{
		if (background != null)
		{
			Vector3 t = background.transform.position;
			t.z = 1;
			background.transform.position = t;
			background.sortingOrder = 0;
			background.sprite = newSprite;
		}
	}

	private void Start()
	{
		OnPlayerEnteredRoomEvent = new UnityEvent<PlayerController, Teleporter>();
		OnPlayerLeftRoomEvent = new UnityEvent<PlayerController, Teleporter>();
		if (leftTeleporter != null)
		{
			leftTeleporter.OnPlayerUsedTeleporterInEvent.AddListener(OnPlayerUsedTeleporterInHandler);
			leftTeleporter.OnPlayerUsedTeleporterOutEvent.AddListener(OnPlayerUsedTeleporterOutHandler);
		}
		if (rightTeleporter != null)
		{
			rightTeleporter.OnPlayerUsedTeleporterInEvent.AddListener(OnPlayerUsedTeleporterInHandler);
			rightTeleporter.OnPlayerUsedTeleporterOutEvent.AddListener(OnPlayerUsedTeleporterOutHandler);
		}
		if (topTeleporter != null)
		{
			topTeleporter.OnPlayerUsedTeleporterInEvent.AddListener(OnPlayerUsedTeleporterInHandler);
			topTeleporter.OnPlayerUsedTeleporterOutEvent.AddListener(OnPlayerUsedTeleporterOutHandler);
		}
		if (bottomTeleporter != null)
		{
			bottomTeleporter.OnPlayerUsedTeleporterInEvent.AddListener(OnPlayerUsedTeleporterInHandler);
			bottomTeleporter.OnPlayerUsedTeleporterOutEvent.AddListener(OnPlayerUsedTeleporterOutHandler);
		}
	}

	private void OnPlayerUsedTeleporterInHandler(PlayerController player, Teleporter teleporter)
	{
		OnPlayerEnteredRoomEvent.Invoke(player, teleporter);
	}

	private void OnPlayerUsedTeleporterOutHandler(PlayerController player, Teleporter teleporter)
	{
		OnPlayerLeftRoomEvent.Invoke(player, teleporter);
	}
}
