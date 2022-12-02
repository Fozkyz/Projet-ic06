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
		Destroy(this);
	}
}
