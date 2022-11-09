using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileComponent
{
	enum ProjectileType { PROJECTILE, HITSCAN};

	[SerializeField] private Sprite projectileSprite;
	[SerializeField] private ProjectileType projectileType;
	[SerializeField] private int projectileDamage;
	[SerializeField] private float projectileSpeed;
	[SerializeField] private float projectileSize;
	[SerializeField] private float projectileLifetime;
	[SerializeField] private int projectileEnemiesToGoThrough;
	[SerializeField] private int projectileNumberOfBounces;

	public void OnShootProjectileHandler(Vector2 dir, Transform shootFrom)
	{
		GameObject projectileGO = new GameObject();
		projectileGO.transform.position = shootFrom.position;

		//float angle = Vector3.Angle(Vector3.right, dir);
		//if (dir.y < 0)
		//{
		//	angle *= -1;
		//}
		//projectileGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		//projectileGO.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, dir.y));
		SpriteRenderer renderer = projectileGO.AddComponent<SpriteRenderer>();
		renderer.sprite = projectileSprite;
		BoxCollider2D col = projectileGO.AddComponent<BoxCollider2D>();
		col.size = renderer.localBounds.size;
		Rigidbody2D rb = projectileGO.AddComponent<Rigidbody2D>();
		rb.velocity = dir * projectileSpeed;
		rb.isKinematic = true;
		rb.useFullKinematicContacts = true;
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		Projectile projectile = projectileGO.AddComponent<Projectile>();
		projectileGO.transform.localScale = Vector3.one * projectileSize;

		projectile.LifeTime = projectileLifetime;
		projectile.EnemiesToGoThrough = projectileEnemiesToGoThrough;
		projectile.NumberOfBounces = projectileNumberOfBounces;
		projectile.DamageAmount = projectileDamage;

		projectile.Launch();
	}
}
