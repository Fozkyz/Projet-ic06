using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] GameObject _coinGO;
	[SerializeField] private int _baseMaxHealth;
	[SerializeField] private int _maxHealthMultiplier;
	[SerializeField] private float _speed;
	[SerializeField] private float _detectRange;
	[SerializeField] private float _invicibilityTime;
	[SerializeField] private Transform _patternPointsHolder;
	[SerializeField] private SpriteRenderer _spriteRenderer;

	public UnityEvent<Enemy> OnEnemyKilledEvent;

	private List<Transform> _patternPoints;

	private Rigidbody2D _rb;
	private PlayerController _player;

	private int _currentHealth;
	private bool _isActive;
	private bool _wasActive;
	private bool _hasDetectedPlayer;
	private int _towardsPointIndex;
	private float _lastTimeHit;

	public void SetActive(bool newIsActive)
	{
		_isActive = newIsActive;
		if (_rb != null)
		{
			_rb.velocity = Vector2.zero;
		}
	}

    public void TakeDamage(int amount, bool bypassInvincibility)
	{
		if (bypassInvincibility || _lastTimeHit + _invicibilityTime < Time.time)
		{
			_currentHealth -= amount;
			_lastTimeHit = Time.time;
			if (_currentHealth <= 0)
			{
				Instantiate(_coinGO, transform.position, Quaternion.identity);
				OnEnemyKilledEvent.Invoke(this);
				Destroy(gameObject);
			}
			else
			{
				StartCoroutine(nameof(Blink));
			}
		}
	}

	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_player = GameManager.Instance.Player;
		_currentHealth = _baseMaxHealth + GameManager.NbGame * _maxHealthMultiplier;
		_towardsPointIndex = 0;
		_lastTimeHit = Time.time - _invicibilityTime;

		_patternPoints = new List<Transform>();
		foreach (Transform patternPoint in _patternPointsHolder.GetComponentsInChildren<Transform>())
		{
			_patternPoints.Add(patternPoint);
		}

		GameManager.Instance.OnGamePausedEvent.AddListener(OnGamePausedHandler);
		GameManager.Instance.OnGameResumeEvent.AddListener(OnGameResumeHandler);
	}

	private void OnGamePausedHandler()
	{
		_wasActive = _isActive;
		_isActive = false;
		_rb.velocity = Vector2.zero;
	}

	private void OnGameResumeHandler()
	{
		_isActive = _wasActive;
	}

	private void Update()
	{
		if (_isActive)
		{
			_hasDetectedPlayer = (transform.position - _player.transform.position).magnitude < _detectRange;
			if (_hasDetectedPlayer)
			{
				// Go towards player
				_rb.velocity = (_player.transform.position - transform.position).normalized * _speed;
			}
			else
			{
				if (_patternPoints.Count > 0)
				{
					if (_patternPoints.Count > 1)
					{
						if ((transform.position - _patternPoints[_towardsPointIndex % _patternPoints.Count].position).magnitude < .5f)
						{
							_towardsPointIndex++;
						}
						_rb.velocity = (_patternPoints[_towardsPointIndex % _patternPoints.Count].position - transform.position).normalized * _speed;
					}
					else
					{
						if ((transform.position - _patternPoints[_towardsPointIndex % _patternPoints.Count].position).magnitude < .5f)
						{
							_rb.velocity = Vector2.zero;
						}
						else
						{
							_rb.velocity = (_patternPoints[_towardsPointIndex % _patternPoints.Count].position - transform.position).normalized * _speed;
						}
					}
				}
				else
				{
					_rb.velocity = Vector2.zero;
				}
			}
			transform.localScale = _rb.velocity.x > 0 ? new Vector2(1, 1) : new Vector2(-1, 1);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerHealth player = collision.transform.GetComponent<PlayerHealth>();
		if (player != null)
		{
			player.TakeDamage();
			return;
		}
	}

	private IEnumerator Blink()
	{
		Color defaultColor = _spriteRenderer.color;
		_speed *= .5f;

		for (int i = 0; i < 2; i++)
		{
			_spriteRenderer.color = new Color(1, 1, 1, 0);
			yield return new WaitForSeconds(_invicibilityTime / 4);
			_spriteRenderer.color = defaultColor;
			yield return new WaitForSeconds(_invicibilityTime / 4);
		}
		_speed *= 2f;
	}
}
