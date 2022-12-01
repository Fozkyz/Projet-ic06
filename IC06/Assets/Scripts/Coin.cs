using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
	[SerializeField] private LayerMask _whatIsGround;
	[SerializeField] private int _minValue;
	[SerializeField] private int _maxValue;

	private Rigidbody2D _rb;

	private int _value;
	private bool _isGrounded;

	void Start()
	{
		_value = Random.Range(_minValue, _maxValue);
		_rb = GetComponent<Rigidbody2D>();
		_rb.velocity = new Vector2(0, -5f);
		_isGrounded = false;
	}

	public void Update()
	{
		if (!_isGrounded)
		{
			RaycastHit2D[] hitInfo = Physics2D.RaycastAll(transform.position, Vector2.down, .75f, _whatIsGround);
			if (hitInfo.Length > 0)
			{
				_rb.velocity = Vector2.zero;
				_isGrounded = true;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerHealth player = collision.GetComponent<PlayerHealth>();
		if (player != null)
		{
			player.AddMoney(_value);
			Destroy(gameObject);
		}
	}
}
