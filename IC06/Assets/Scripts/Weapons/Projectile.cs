using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

	public float LifeTime { get; set; }
	public int EnemiesToGoThrough { get; set; }
	public int NumberOfBounces { get; set; }
	public int DamageAmount { get; set; }

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
		CalculateRotation();
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
			Debug.Log("Hit wall");
			if (bounces < NumberOfBounces)
			{
				Vector2 normal = collision.GetContact(0).normal;
				rb.velocity = rb.velocity - 2 * Vector2.Dot(normal, rb.velocity) * normal;
				
				bounces++;
				CalculateRotation();
			}
			else
			{
				Destroy(gameObject);
			}
		}
		// Hit an enemy
		if (((1<<collision.gameObject.layer) & enemyLayer) != 0 && !previousHits.Contains(collision.transform))
		{
			Debug.Log("Hit enemy");
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			enemy.TakeDamage(DamageAmount);
			if (previousHits.Count >= EnemiesToGoThrough)
			{
				Destroy(gameObject);
			}
			previousHits.Add(collision.transform);
			return;
		}
	}

	private void CalculateRotation()
	{
		float angle = Vector3.Angle(Vector3.right, rb.velocity);
		if (rb.velocity.y < 0)
		{
			angle *= -1;
		}
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	IEnumerator DoLifeTime()
	{
		yield return new WaitForSeconds(LifeTime);
		Destroy(gameObject);
	}
}
