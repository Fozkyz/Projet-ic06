using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] private int _maxHealth;
	[SerializeField] private float _speed;
	[SerializeField] private float _detectRange;
	[SerializeField] private float _invicibilityTime;
	[SerializeField] private List<Transform> _patternPoints;

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
	}

    public void TakeDamage(int amount, bool bypassInvincibility)
	{
		if (bypassInvincibility || _lastTimeHit + _invicibilityTime < Time.time)
		{
			_currentHealth -= amount;
			_lastTimeHit = Time.time;
			if (_currentHealth <= 0)
			{
				Destroy(gameObject);
			}
		}
	}

	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_player = GameManager.Instance.Player;
		_currentHealth = _maxHealth;
		_towardsPointIndex = 0;
		_lastTimeHit = Time.time - _invicibilityTime;

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
					// Go to next point
					if ((transform.position - _patternPoints[_towardsPointIndex % _patternPoints.Count].position).magnitude < .5f)
					{
						_towardsPointIndex++;
					}
					_rb.velocity = (_patternPoints[_towardsPointIndex % _patternPoints.Count].position - transform.position).normalized * _speed;
				}
				else
				{
					_rb.velocity = Vector2.zero;
				}
			}
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
}
