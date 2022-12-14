using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public enum ProjectileType { PROJECTILE, HITSCAN};
[System.Serializable]
public class ProjectileComponent
{
	public UnityEvent<Projectile> OnProjectileFiredEvent;
	public UnityEvent<Projectile> OnHitscanFiredEvent;


	[SerializeField] private GameObject projectileGO; // Can be a projectile or a hitscan
	[SerializeField] private Sprite projectileSprite; // Only used if projectileGO is null
	[SerializeField] private ProjectileType projectileType;
	[SerializeField, Tooltip("Whether or not the projectile attributes should take their default values from the projectile gameobject")]
	private bool overrideProjectileAttributes;
	//[SerializeField] private Sprite projectileSprite;
	[SerializeField] private float projectileSpeed;
	[SerializeField] private float projectileSize;
	[SerializeField] private float projectileLifetime;
	[SerializeField] private float projectileRange;
	[SerializeField] private int projectileEnemiesToGoThrough;
	[SerializeField] private int projectileNumberOfBounces;
	[SerializeField] private bool isProjectileAffectedByGravity;
	[SerializeField] private bool dontDestroyProjectileOnHit;
	[SerializeField] private bool folowsMouse;

	public float ProjectileSpeedModifier { get; set; }
	public float ProjectileSpeedMultiplier { get; set; }
	public float ProjectileRangeModifier { get; set; }
	public float ProjectileRangeMultiplier { get; set; }
	public int ProjectileEnemiesToGoThroughModifier { get; set; }
	public float ProjectileEnemiesToGoThroughMultiplier { get; set; }
	public int ProjectileNumberOfBouncesModifier { get; set; }
	public float ProjectileNumberOfBouncesMultiplier { get; set; }

	private GameObject projectileInstance;

	public void InitComponent()
	{
		ProjectileSpeedModifier = 0f;
		ProjectileSpeedMultiplier = 1f;
		ProjectileRangeModifier = 0f;
		ProjectileRangeMultiplier = 1f;
		ProjectileEnemiesToGoThroughModifier = 0;
		ProjectileEnemiesToGoThroughMultiplier = 1f;
		ProjectileNumberOfBouncesModifier = 0;
		ProjectileNumberOfBouncesMultiplier = 1f;
	}

	public void OnShootProjectileHandler(Vector2 dir, Transform shootFrom)
	{
		switch (projectileType)
		{
			case ProjectileType.PROJECTILE:
				ShootProjectile(dir, shootFrom);
				break;
			case ProjectileType.HITSCAN:
				ShootProjectile(dir, shootFrom);
				break;
			default:
				break;
		}
	}

	public void OnStoppedHoldingHandler()
	{
		if (projectileInstance != null)
		{
			Projectile proj = projectileInstance.GetComponent<Projectile>();
			if (proj != null)
			{
				GameObject.Destroy(proj.gameObject);
			}
		}
	}

	private void ShootProjectile(Vector2 dir, Transform shootFrom)
	{
		if (projectileGO == null)
		{
			projectileInstance = new GameObject();
		}
		else
		{
			projectileInstance = GameObject.Instantiate(projectileGO);
		}
		projectileInstance.transform.position = shootFrom.position;

		Projectile projectile = projectileInstance.GetComponent<Projectile>();
		if (projectile == null)
		{
			projectile = projectileInstance.AddComponent<Projectile>();
		}
		if (overrideProjectileAttributes)
		{
			projectile.ProjectileType = projectileType;
			projectile.LifeTime = projectileLifetime;
			projectile.Size = projectileSize;
			projectile.Speed = (projectileSpeed + ProjectileSpeedModifier) * ProjectileSpeedMultiplier;
			projectile.MaxRange = (projectileRange + ProjectileRangeModifier) * ProjectileRangeMultiplier;
			projectile.EnemiesToGoThrough = Mathf.FloorToInt((projectileEnemiesToGoThrough + ProjectileEnemiesToGoThroughModifier) * ProjectileEnemiesToGoThroughMultiplier);
			projectile.NumberOfBounces = Mathf.FloorToInt((projectileNumberOfBounces + ProjectileNumberOfBouncesModifier) * ProjectileNumberOfBouncesMultiplier);
			projectile.IsAffectedByGravity = isProjectileAffectedByGravity;
			projectile.DontDestroyOnHit = dontDestroyProjectileOnHit;
			projectile.FolowsMouse = folowsMouse;
			projectile.Direction = dir;
			projectile.ShootFrom = shootFrom;
		}

		projectile.OnEnemyHitEvent = new UnityEvent<Projectile, Enemy>();
		projectile.OnLastEnemyHitEvent = new UnityEvent<Projectile, Enemy>();
		projectile.OnWallHitEvent = new UnityEvent<Projectile, Transform, Vector2>();
		projectile.OnLastWallHitEvent = new UnityEvent<Projectile, Transform, Vector2>();
		projectile.OnLifetimeElapsedEvent = new UnityEvent<Projectile>();

		OnProjectileFiredEvent.Invoke(projectile);
		projectile.Launch();
	}
}
