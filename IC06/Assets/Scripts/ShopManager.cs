using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
	[SerializeField] private WeaponStand weaponStand;
	[SerializeField] private AugmentStand augmentStand;

	public void InitShop()
	{
		List<WeaponSO> weapons = GameManager.Instance.GetWeaponsSO();
		List<Augment> augments = GameManager.Instance.GetAugmentsSO();

		weaponStand.SetWeapon(weapons[Random.Range(0, weapons.Count)]);
		augmentStand.SetAugment(augments[Random.Range(0, augments.Count)]);
	}
}
