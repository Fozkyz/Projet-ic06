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

	public int OnHitDamageModifier { get; set; }
	public float OnHitDamageMultiplier { get; set; }
	public int OnHitExplosionDamageModifier { get; set; }
	public float OnHitExplosionDamageMultiplier { get; set; }
	public int OnHitExplosionRadiusModifier { get; set; }
	public float OnHitExplosionRadiusMultiplier { get; set; }
	public bool OnHitExplosionHitsPlayerOverride { get; set; }

	public void InitComponent()
	{
		OnHitDamageModifier = 0;
		OnHitDamageMultiplier = 1f;
		OnHitExplosionDamageModifier = 0;
		OnHitExplosionDamageMultiplier = 1f;
		OnHitExplosionRadiusModifier = 0;
		OnHitExplosionRadiusMultiplier = 1f;
		OnHitExplosionHitsPlayerOverride = false;
	}

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
		int damage = Mathf.FloorToInt((onHitDamage + OnHitDamageModifier) * OnHitDamageMultiplier);
		enemy.TakeDamage(damage, false);
		// Spawn particle effects
	}

	private void OnLastEnemyHitHandler(Projectile proj, Enemy enemy)
	{
		// OnEnemyHitEvent will always be fired before OnLastEnemyHitEvent,
		// so there's no need to calculate damage amount (unless we want additional damage...)
		if (!proj.DontDestroyOnHit)
			DestroyProjectile(proj);
	}

	private void OnWallHitHandler(Projectile proj, Transform wall, Vector2 hitPos)
	{
	}

	private void OnLastWallHitHandler(Projectile proj, Transform wall, Vector2 hitPos)
	{
		if (!proj.DontDestroyOnHit)
			DestroyProjectile(proj);
	}

	private void OnLifetimeElapsedHandler(Projectile proj)
	{
		DestroyProjectile(proj);
	}

	private void DestroyProjectile(Projectile proj)
	{
		if (explosionParticleSystem != null)
		{
			GameObject.Instantiate(explosionParticleSystem, proj.GetProjectileEndPoint(), Quaternion.identity);
		}
		GameObject.Destroy(proj.gameObject);

		int explosionRadius = Mathf.FloorToInt((onHitExplosionRadius + OnHitExplosionRadiusModifier) * OnHitExplosionRadiusMultiplier);
		if (explosionRadius > 0)
		{
			int explosionDamage = Mathf.FloorToInt((onHitExplosionDamage + OnHitExplosionDamageModifier) * OnHitExplosionDamageMultiplier);
			Collider2D[] cols = Physics2D.OverlapCircleAll(proj.GetProjectileEndPoint(), explosionRadius);
			foreach (Collider2D col in cols)
			{
				Enemy enemy = col.GetComponent<Enemy>();
				if (enemy != null)
				{
					enemy.TakeDamage(explosionDamage, true);
					continue;
				}
				if (onHitExplosionHitsPlayer || OnHitExplosionHitsPlayerOverride)
				{
					PlayerHealth player = col.GetComponent<PlayerHealth>();
					if (player != null)
					{
						player.TakeDamage();
					}
				}
			}
		}
	}
}
