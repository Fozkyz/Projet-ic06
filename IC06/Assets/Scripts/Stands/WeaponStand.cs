using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponStand : InteractableStand
{
	[SerializeField] private TextMeshProUGUI weaponText;

	WeaponSO weaponSO;

	public void SetWeapon(WeaponSO newWeapon)
	{
		weaponSO = newWeapon;
		weaponText.text = weaponSO.name;
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		if (renderer != null)
		{
			renderer.sprite = weaponSO.GetWeaponSprite();
		}
		
	}

	public WeaponSO GetWeapon()
	{
		return weaponSO;
	}

	protected override void Interact(PlayerHealth player)
	{
		Weapon weapon = player.GetComponent<Weapon>();
		if (weapon != null)
		{
			WeaponSO temp = weapon.GetWeaponSO();
			player.InteractWithStand(this);
			SetWeapon(temp);
		}
	}
}
