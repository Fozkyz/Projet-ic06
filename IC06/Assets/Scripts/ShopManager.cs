using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
	[SerializeField] private WeaponStand weaponStand;

	public void InitShop()
	{
		List<WeaponSO> weapons = GameManager.Instance.GetWeaponsSO();

		weaponStand.SetWeapon(weapons[Random.Range(0, weapons.Count)]);
	}
}
