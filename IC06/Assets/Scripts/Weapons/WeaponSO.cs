using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
	[SerializeField] ShootComponent shootComponent;
	[SerializeField] ProjectileComponent projectileComponent;
	[SerializeField] OnHitComponent onHitComponent;
	[SerializeField] SoundComponent soundComponent;

	public ShootComponent GetShootComponent()
	{
		return shootComponent;
	}

	public ProjectileComponent GetProjectileComponent()
	{
		return projectileComponent;
	}

	public OnHitComponent GetOnHitComponent()
	{
		return onHitComponent;
	}

	public SoundComponent GetSoundComponent()
	{
		return soundComponent;
	}
}
