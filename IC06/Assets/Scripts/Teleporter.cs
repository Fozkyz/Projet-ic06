using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	[SerializeField] private float cooldownTime = 0f;

	private Teleporter linkedTeleporter;
	private float timeSinceUsed;

	public void SetLinkedTeleporter(Teleporter tp)
	{
		linkedTeleporter = tp;
	}

	public void TeleportPlayer(PlayerController player)
	{
		player.transform.position = transform.position;
		timeSinceUsed = 0f;
	}

	private void Update()
	{
		timeSinceUsed += Time.deltaTime;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (timeSinceUsed > cooldownTime)
		{
			PlayerController player = collision.GetComponent<PlayerController>();
			if (player != null)
			{
				linkedTeleporter.TeleportPlayer(player);
			}
		}
	}
}
