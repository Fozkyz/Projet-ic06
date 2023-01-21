using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStand : InteractableStand
{
	protected override void Interact(PlayerHealth player)
	{
		player.InteractWithStand(this);
		_canBuy = false;
		_priceText.enabled = false;
		_canPlayerPressRenderer.enabled = false;
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
		foreach (Renderer renderer in renderers)
		{
			Destroy(renderer);
		}
		Destroy(this);
	}
}
