using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OnHitComponent
{
	[SerializeField] private int damage;

    public void OnProjectileFiredHandler(Projectile proj)
	{
		proj.OnEnemyHitEvent.AddListener(OnEnemyHitHandler);
		proj.OnLastEnemyHitEvent.AddListener(OnLastEnemyHitHandler);
		proj.OnWallHitEvent.AddListener(OnWallHitHandler);
		proj.OnLastWallHitEvent.AddListener(OnLastWallHitHandler);
		proj.OnLifetimeElapsedEvent.AddListener(OnLifetimeElapsedHandler);
	}

	private void OnEnemyHitHandler(Projectile proj, Enemy enemy)
	{
		enemy.TakeDamage(damage);
		// Spawn particle effects
	}

	private void OnLastEnemyHitHandler(Projectile proj, Enemy enemy)
	{
		// OnEnemyHitEvent will always be fired before OnLastEnemyHitEvent,
		// so there's no need to calculate damage amount (unless we want additional damage...)
		proj.DestroyProjectile();
	}

	private void OnWallHitHandler(Projectile proj, Collision2D collision)
	{
		proj.CalculateProjectileRotation();
	}

	private void OnLastWallHitHandler(Projectile proj, Collision2D collision)
	{
		proj.DestroyProjectile();
	}

	private void OnLifetimeElapsedHandler(Projectile proj)
	{
		proj.DestroyProjectile();
	}
}
