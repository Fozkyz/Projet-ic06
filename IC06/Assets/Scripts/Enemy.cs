using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] private int maxHealth;

	private int currentHealth;

    public void TakeDamage(int amount)
	{
		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		currentHealth = maxHealth;
	}
}
