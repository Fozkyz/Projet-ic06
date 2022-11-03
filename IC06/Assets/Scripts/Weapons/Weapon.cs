using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSO;
	[SerializeField] private Transform shootFrom;

    UnityEvent mouseButtonPressedEvent;
    UnityEvent mouseButtonReleasedEvent;

	UnityEvent<Vector2, Transform> shootProjectileEvent;

	public void ShootHandler()
	{
		Debug.Log("Shoot");
		shootProjectileEvent.Invoke(Vector2.right, shootFrom);
	}

	private void Start()
	{
		if (weaponSO != null)
		{
			if (mouseButtonPressedEvent == null)
			{
				mouseButtonPressedEvent = new UnityEvent();
			}
			if (mouseButtonReleasedEvent == null)
			{
				mouseButtonReleasedEvent = new UnityEvent();
			}
			if (shootProjectileEvent == null)
			{
				shootProjectileEvent = new UnityEvent<Vector2, Transform>();
			}
			mouseButtonPressedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonPressedHandler);
			mouseButtonReleasedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonReleasedHandler);
			shootProjectileEvent.AddListener(weaponSO.GetProjectileComponent().OnShootProjectileHandler);

			weaponSO.GetShootComponent().shootEvent.AddListener(ShootHandler);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			mouseButtonPressedEvent.Invoke();
		}
		if (Input.GetMouseButtonUp(0))
		{
			mouseButtonReleasedEvent.Invoke();
		}
	}
}
