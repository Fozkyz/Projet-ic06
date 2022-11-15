using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSO;
	[SerializeField] private Transform shootFrom;
	[SerializeField] private Transform playerGraphics;
	
	private Animator playerAnimator;
	
	private Camera cam;
	private bool isFacingRight;

    UnityEvent mouseButtonPressedEvent;
    UnityEvent mouseButtonReleasedEvent;

	UnityEvent<Vector2, Transform> shootProjectileEvent;

	public void OnShootHandler()
	{
		Vector2 shootFromPos =  cam.WorldToScreenPoint(shootFrom.position);
		Vector2 shootDir = new Vector2(Input.mousePosition.x - shootFromPos.x, Input.mousePosition.y - shootFromPos.y).normalized;
		shootProjectileEvent.Invoke(shootDir, shootFrom);
	}

	private void Start()
	{
		cam = Camera.main;
		isFacingRight = true;
		playerAnimator = playerGraphics.GetComponent<Animator>();
		if (weaponSO != null)
		{
			mouseButtonPressedEvent = new UnityEvent();
			mouseButtonReleasedEvent = new UnityEvent();
			shootProjectileEvent = new UnityEvent<Vector2, Transform>();

			weaponSO.GetShootComponent().OnShootEvent = new UnityEvent();
			weaponSO.GetShootComponent().OnStoppedHoldingEvent = new UnityEvent();
			weaponSO.GetShootComponent().OnCanceledChargeEvent = new UnityEvent();
			weaponSO.GetProjectileComponent().OnProjectileFiredEvent = new UnityEvent<Projectile>();

			mouseButtonPressedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonPressedHandler);
			mouseButtonReleasedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonReleasedHandler);
			shootProjectileEvent.AddListener(weaponSO.GetProjectileComponent().OnShootProjectileHandler);

			weaponSO.GetShootComponent().OnShootEvent.AddListener(OnShootHandler);
			weaponSO.GetShootComponent().OnStoppedHoldingEvent.AddListener(weaponSO.GetProjectileComponent().OnStoppedHoldingHandler);

			weaponSO.GetProjectileComponent().OnProjectileFiredEvent.AddListener(weaponSO.GetOnHitComponent().OnProjectileFiredHandler);

			weaponSO.GetOnHitComponent().DestroyOnHit = weaponSO.GetShootComponent().GetShootType() != ShootType.HOLD;
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
		Vector2 shootFromPos = cam.WorldToScreenPoint(shootFrom.position);
		if (isFacingRight && Input.mousePosition.x < shootFromPos.x || !isFacingRight && Input.mousePosition.x > shootFromPos.x)
		{
			isFacingRight = !isFacingRight;
			playerGraphics.Rotate(playerGraphics.up, 180f);
			playerAnimator.SetBool("FacingRight", isFacingRight);
		}
	}
}
