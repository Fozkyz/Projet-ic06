using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AugmentStand : InteractableStand
{
	[SerializeField] private TextMeshProUGUI augmentText;

	[SerializeField] private Augment augment;

	public void SetAugment(Augment newAugment)
	{
		augment = newAugment;
		augmentText.text = augment.Name;
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		if (renderer != null)
		{
			renderer.sprite = augment.Sprite;
		}
	}

	public Augment GetAugment()
	{
		return augment;
	}

	protected override void Interact(PlayerHealth player)
	{
		player.InteractWithStand(this);
		_canBuy = false;
		augmentText.enabled = false;
		_canPlayerPressRenderer.enabled = false;
		_priceText.enabled = false;
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		if (renderer != null)
		{
			Destroy(renderer);
		}
		Destroy(this);
	}
}
