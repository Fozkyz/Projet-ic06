using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSO;
	[SerializeField] private Transform shootFrom;
	
	private Camera cam;

    UnityEvent mouseButtonPressedEvent;
    UnityEvent mouseButtonReleasedEvent;

	UnityEvent<Vector2, Transform> shootProjectileEvent;

	public void ShootHandler()
	{
		Vector2 shootFromPos =  cam.WorldToScreenPoint(shootFrom.position);
		Vector2 shootDir = new Vector2(Input.mousePosition.x - shootFromPos.x, Input.mousePosition.y - shootFromPos.y).normalized;
		shootProjectileEvent.Invoke(shootDir, shootFrom);
	}

	private void Start()
	{
		cam = Camera.main;
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
