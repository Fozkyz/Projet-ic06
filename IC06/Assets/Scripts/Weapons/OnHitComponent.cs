using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OnHitComponent
{
	[SerializeField] private int onHitDamage;
	[SerializeField] private ParticleSystem explosionParticleSystem;
	[SerializeField] private Sprite onHitExplosionSprite;
	[SerializeField] private float onHitExplosionRadius;
	[SerializeField] private int onHitExplosionDamage;
	[SerializeField] private bool onHitExplosionHitsPlayer;

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
		enemy.TakeDamage(onHitDamage);
		// Spawn particle effects
	}

	private void OnLastEnemyHitHandler(Projectile proj, Enemy enemy)
	{
		// OnEnemyHitEvent will always be fired before OnLastEnemyHitEvent,
		// so there's no need to calculate damage amount (unless we want additional damage...)
	}

	private void OnWallHitHandler(Projectile proj, Transform wall, Vector2 hitPos)
	{
	}

	private void OnLastWallHitHandler(Projectile proj, Transform wall, Vector2 hitPos)
	{
	}

	private void OnLifetimeElapsedHandler(Projectile proj)
	{
	}
}
