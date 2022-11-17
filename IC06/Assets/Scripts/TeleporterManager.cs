using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
	[SerializeField] private Teleporter leftTeleporter;
	[SerializeField] private Teleporter rightTeleporter;
	[SerializeField] private Teleporter topTeleporter;
	[SerializeField] private Teleporter bottomTeleporter;

	public UnityEvent<PlayerController, Teleporter> OnPlayerEnteredRoomEvent;
	public UnityEvent<PlayerController, Teleporter> OnPlayerLeftRoomEvent;

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
