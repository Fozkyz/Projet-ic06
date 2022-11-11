using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public UnityEvent<Projectile, Enemy> OnEnemyHitEvent;
	public UnityEvent<Projectile, Enemy> OnLastEnemyHitEvent;
	public UnityEvent<Projectile, Collision2D> OnWallHitEvent;
	public UnityEvent<Projectile, Collision2D> OnLastWallHitEvent;
	public UnityEvent<Projectile> OnLifetimeElapsedEvent;

	private Rigidbody2D rb;

	public float LifeTime { get; set; }
	public int EnemiesToGoThrough { get; set; }
	public int NumberOfBounces { get; set; }

	private LayerMask groundLayer;
	private LayerMask enemyLayer;

	private int bounces;

	private List<Transform> previousHits;

	public void Launch()
	{
		rb = GetComponent<Rigidbody2D>();
		previousHits = new List<Transform>();
		bounces = 0;
		groundLayer = LayerMask.GetMask("Wall");
		enemyLayer = LayerMask.GetMask("Enemy");
		CalculateProjectileRotation();
		StartCoroutine("DoLifeTime");
	}

	public void CalculateProjectileRotation()
	{
		float angle = Vector3.Angle(Vector3.right, rb.velocity);
		if (rb.velocity.y < 0)
		{
			angle *= -1;
		}
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void DestroyProjectile()
	{
		Destroy(gameObject);
	}

	private void Start()
	{
		
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Hit a wall
		if (((1<<collision.gameObject.layer) & groundLayer) != 0)
		{
			OnWallHitEvent.Invoke(this, collision);
			if (bounces >= NumberOfBounces)
			{
				OnLastWallHitEvent.Invoke(this, collision);
			}
		}
		// Hit an enemy
		if (((1<<collision.gameObject.layer) & enemyLayer) != 0 && !previousHits.Contains(collision.transform))
		{
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			if (enemy != null)
			{
				OnEnemyHitEvent.Invoke(this, enemy);
				if (previousHits.Count >= EnemiesToGoThrough)
				{
					OnLastEnemyHitEvent.Invoke(this, enemy);
				}
				previousHits.Add(collision.transform);
			}
			return;
		}
	}

	IEnumerator DoLifeTime()
	{
		yield return new WaitForSeconds(LifeTime);
		OnLifetimeElapsedEvent.Invoke(this);
	}
}
