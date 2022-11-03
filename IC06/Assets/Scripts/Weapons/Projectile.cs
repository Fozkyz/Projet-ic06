using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;

	[SerializeField] LayerMask enemyLayer;
	[SerializeField] LayerMask groundLayer;

	public float LifeTime { get; set; }
	public int EnemiesToGoThrough { get; set; }
	public int NumberOfBounces { get; set; }
	public int DamageAmount { get; set; }

	private int bounces;
	private int enemiesTraversed;

	private List<Transform> previousHits;

	public void Launch()
	{
		rb = GetComponent<Rigidbody2D>();
		previousHits = new List<Transform>();
		bounces = 0;
		StartCoroutine("DoLifeTime");
	}

	private void Update()
	{
		
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Hit a wall
		if (((1<<collision.gameObject.layer) & groundLayer) != 0)
		{
			if (bounces < NumberOfBounces)
			{
				Vector2 normal = collision.GetContact(0).normal;
				rb.velocity = rb.velocity - 2 * Vector2.Dot(normal, rb.velocity) * normal;
				
				bounces++;
			}
		}
		// Hit an enemy
		if (((1<<collision.gameObject.layer) & enemyLayer) != 0 && !previousHits.Contains(collision.transform))
		{
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			enemy.TakeDamage(DamageAmount);
			if (enemiesTraversed >= EnemiesToGoThrough)
			{
				Destroy(gameObject);
			}
			previousHits.Add(collision.transform);
		}
	}

	IEnumerator DoLifeTime()
	{
		yield return new WaitForSeconds(LifeTime);
		Destroy(gameObject);
	}
}
