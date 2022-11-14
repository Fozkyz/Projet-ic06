using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public UnityEvent<Projectile, Enemy> OnEnemyHitEvent;
	public UnityEvent<Projectile, Enemy> OnLastEnemyHitEvent;
	public UnityEvent<Projectile, Transform, Vector2> OnWallHitEvent;
	public UnityEvent<Projectile, Transform, Vector2> OnLastWallHitEvent;
	public UnityEvent<Projectile> OnLifetimeElapsedEvent;

	[SerializeField] ParticleSystem particles;

	[field: SerializeField] public ProjectileType ProjectileType { get; set; }
	[field: SerializeField] public float LifeTime { get; set; }
	[field: SerializeField] public float Size { get; set; }
	[field: SerializeField] public float Speed { get; set; }
	[field: SerializeField] public float MaxRange { get; set; }
	[field: SerializeField] public int EnemiesToGoThrough { get; set; }
	[field: SerializeField] public int NumberOfBounces { get; set; }
	[field: SerializeField] public bool IsAffectedByGravity { get; set; }
	[field: SerializeField] public bool FolowsMouse { get; set; }

	[HideInInspector] public Vector2 Direction { get; set; }
	[HideInInspector] public Transform ShootFrom { get; set; }

	private Rigidbody2D rb;
	private LineRenderer lr;

	private LayerMask groundLayer;
	private LayerMask enemyLayer;

	private int bounces;

	private float refreshTime;

	private List<Transform> previousHits;

	public void Launch()
	{
		groundLayer = LayerMask.GetMask("Wall");
		enemyLayer = LayerMask.GetMask("Enemy");
		previousHits = new List<Transform>();
		bounces = 0;

		switch (ProjectileType)
		{
			case ProjectileType.PROJECTILE:
				LaunchProjectile();
				break;
			case ProjectileType.HITSCAN:
				LaunchHitscan();
				break;
			default:
				break;
		}
		if (FolowsMouse)
		{
			StartCoroutine("FolowMouse");
		}

		if (LifeTime >= 0)
		{
			StartCoroutine("DoLifeTime");
		}
	}

	private void LaunchProjectile()
	{
		rb = GetComponent<Rigidbody2D>();
		if (rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody2D>();
		}
		rb.velocity = Direction * Speed;
		rb.useFullKinematicContacts = true;
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb.isKinematic = !IsAffectedByGravity;
		transform.localScale = Vector3.one * Size;
		CalculateProjectileRotation();
	}

	private void LaunchHitscan()
	{
		lr = GetComponent<LineRenderer>();
		if (lr == null)
		{
			lr = gameObject.AddComponent<LineRenderer>();
		}
		lr.widthMultiplier = Size;
		SetLineRendererPositions();
	}

	private void SetLineRendererPositions()
	{
		previousHits = new List<Transform>();
		bounces = 0;
		List<Vector2> pointsPositions = CalculateHitscan(ShootFrom.position, Direction, MaxRange);
		lr.positionCount = pointsPositions.Count + 1;
		lr.SetPosition(0, ShootFrom.position);
		for (int i = 0; i < pointsPositions.Count; i++)
		{
			lr.SetPosition(i + 1, pointsPositions[i]);
		}
	}

	private void CalculateBounce(Collision2D collision)
	{
		Vector2 normal = collision.GetContact(0).normal;
		rb.velocity = rb.velocity - 2 * Vector2.Dot(normal, rb.velocity) * normal;
		CalculateProjectileRotation();
	}

	private void CalculateProjectileRotation()
	{
		float angle = Vector3.Angle(Vector3.right, rb.velocity);
		if (rb.velocity.y < 0)
		{
			angle *= -1;
		}
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private List<Vector2> CalculateHitscan(Vector2 startPos, Vector2 direction, float range)
	{
		List<Vector2> positions = new List<Vector2>();

		RaycastHit2D[] hitInfo = Physics2D.RaycastAll(startPos, direction, range, enemyLayer | groundLayer);
		if (hitInfo.Length > 0)
		{
			foreach (RaycastHit2D hit in hitInfo)
			{
				if (previousHits.Contains(hit.transform))
					continue;
				// Do smth
				Enemy enemy = hit.transform.GetComponent<Enemy>();
				if (enemy != null)
				{
					OnEnemyHitEvent.Invoke(this, enemy);
					if (previousHits.Count >= EnemiesToGoThrough)
					{
						OnLastEnemyHitEvent.Invoke(this, enemy);
						positions.Add(hit.transform.position);
						return positions;
					}
					previousHits.Add(hit.transform);
				}
				else
				{
					OnWallHitEvent.Invoke(this, hit.transform, hit.point);
					if (bounces < NumberOfBounces)
					{
						bounces++;
						positions.Add(hit.point);
						previousHits.Add(hit.transform);
						Vector2 newDir = direction - 2 * Vector2.Dot(hit.normal, direction) * hit.normal;
						List<Vector2> pos = CalculateHitscan(hit.point, newDir, range - (startPos - hit.point).magnitude);
						foreach (Vector2 p in pos)
						{
							positions.Add(p);
						}
						return positions;
					}
					else
					{
						positions.Add(hit.point);
						previousHits.Add(hit.transform);
						OnLastWallHitEvent.Invoke(this, hit.transform, hit.point);
						return positions;
					}
				}

			}
		}
		positions.Add(startPos + direction * range);
		return positions;
	}

	private void DestroyProjectile()
	{
		if (particles != null)
		{
			Instantiate(particles, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}

	private void Start()
	{
		refreshTime = Time.deltaTime * 5;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Hit a wall
		if (((1<<collision.gameObject.layer) & groundLayer) != 0)
		{
			OnWallHitEvent.Invoke(this, collision.transform, collision.GetContact(0).point);
			if (bounces >= NumberOfBounces)
			{
				OnLastWallHitEvent.Invoke(this, collision.transform, collision.GetContact(0).point);
			}
			CalculateBounce(collision);
			bounces++;
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

	IEnumerator FolowMouse()
	{
		switch (ProjectileType)
		{
			case ProjectileType.PROJECTILE:
				while(true)
				{
					yield return new WaitForSeconds(refreshTime);
					Vector2 currPos = Camera.main.WorldToScreenPoint(transform.position);
					Direction = new Vector2(Input.mousePosition.x - currPos.x, Input.mousePosition.y - currPos.y);
					rb.velocity = Direction * Speed;
				}
			case ProjectileType.HITSCAN:
				while (true)
				{
					yield return new WaitForSeconds(refreshTime);
					Vector2 shootFromPos = Camera.main.WorldToScreenPoint(ShootFrom.position);
					Direction = new Vector2(Input.mousePosition.x - shootFromPos.x, Input.mousePosition.y - shootFromPos.y);
					SetLineRendererPositions();
				}
			default:
				break;
		}
		
	}

	IEnumerator DoLifeTime()
	{
		yield return new WaitForSeconds(LifeTime);
		DestroyProjectile();
		OnLifetimeElapsedEvent.Invoke(this);
	}
}
