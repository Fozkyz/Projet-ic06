using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	[SerializeField] private List<KeyCode> teleportKeys;
	[SerializeField] private float cooldownTime = 0f;

	public UnityEvent<PlayerController, Teleporter> OnPlayerUsedTeleporterInEvent;
	public UnityEvent<PlayerController, Teleporter> OnPlayerUsedTeleporterOutEvent;

	public bool IsTpLocked { get; set; }

	private Teleporter linkedTeleporter;

	private PlayerController _player;
	private float timeSinceUsed;
	private bool isPlayerOnTP;

	public void SetLinkedTeleporter(Teleporter tp)
	{
		linkedTeleporter = tp;
	}

	public void TeleportPlayer(PlayerController player)
	{
		player.transform.position = transform.position;
		isPlayerOnTP = true;
		_player = player;
		OnPlayerUsedTeleporterInEvent.Invoke(player, this);
	}

	private void Awake()
	{
		OnPlayerUsedTeleporterInEvent = new UnityEvent<PlayerController, Teleporter>();
		OnPlayerUsedTeleporterOutEvent = new UnityEvent<PlayerController, Teleporter>();
	}

	private void Start()
	{
		IsTpLocked = false;
	}

	private void Update()
	{
		timeSinceUsed += Time.deltaTime;
		if (isPlayerOnTP && !IsTpLocked && timeSinceUsed > cooldownTime)
		{
			if (Utils.IsAnyKeyHeld(teleportKeys))
			{
				timeSinceUsed = 0f;
				linkedTeleporter.TeleportPlayer(_player);
				OnPlayerUsedTeleporterOutEvent.Invoke(_player, this);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerController player = collision.GetComponent<PlayerController>();
		if (player != null)
		{
			isPlayerOnTP = true;
			_player = player;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		PlayerController player = collision.GetComponent<PlayerController>();
		if (player != null)
		{
			isPlayerOnTP = false;
			_player = null;
		}
	}
}
