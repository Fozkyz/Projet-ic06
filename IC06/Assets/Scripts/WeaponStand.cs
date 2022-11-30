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

	protected override void Interact()
	{
		WeaponSO temp = playerWeapon.GetWeaponSO();
		playerWeapon.SetWeapon(weaponSO);
		SetWeapon(temp);
	}
}
