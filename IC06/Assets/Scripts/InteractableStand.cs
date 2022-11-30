using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableStand : MonoBehaviour
{
	[SerializeField] SpriteRenderer canPlayerPressRenderer;
	[SerializeField] KeyCode keyCode;
	[SerializeField] int price;

	private bool canPlayerPress;
	protected Weapon playerWeapon;
	
	private void Start()
	{
		canPlayerPress = false;
		canPlayerPressRenderer.enabled = false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(keyCode) && canPlayerPress && playerWeapon.GetCurrentMoney() >= price)
		{
			playerWeapon.PayMoney(price);
			Interact();
		}
	}

	protected abstract void Interact();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Weapon weapon = collision.GetComponent<Weapon>();
		if (weapon != null)
		{
			canPlayerPressRenderer.enabled = true;
			playerWeapon = weapon;
			canPlayerPress = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Weapon weapon = collision.GetComponent<Weapon>();
		if (weapon != null)
		{
			canPlayerPressRenderer.enabled = false;
			canPlayerPress = false;
		}
	}
}
