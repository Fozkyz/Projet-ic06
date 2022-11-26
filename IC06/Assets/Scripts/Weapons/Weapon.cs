using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSO;
	[SerializeField] private List<WeaponSO> weaponsSO;
	[SerializeField] private Transform shootFrom;
	[SerializeField] private Transform playerGraphics;
	[SerializeField] private TextMeshProUGUI weaponText;
	[SerializeField] private AudioSource shootSource;
	[SerializeField] private AudioSource hitSource;
	
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

	public void SetWeaponSO(WeaponSO newWeaponSO)
	{
		weaponSO = newWeaponSO;
		InitWeapon();
	}

	private void Start()
	{
		cam = Camera.main;
		isFacingRight = true;
		playerAnimator = playerGraphics.GetComponent<Animator>();
		weaponSO = weaponsSO[0];
		InitWeapon();
	}

	private void InitWeapon()
	{
		if (weaponSO != null)
		{
			weaponText.text = weaponSO.name;
			mouseButtonPressedEvent = new UnityEvent();
			mouseButtonReleasedEvent = new UnityEvent();
			shootProjectileEvent = new UnityEvent<Vector2, Transform>();

			weaponSO.GetSoundComponent().ShootSource = shootSource;
			weaponSO.GetSoundComponent().HitSource = hitSource;

			weaponSO.GetShootComponent().OnShootEvent = new UnityEvent();
			weaponSO.GetShootComponent().OnStoppedHoldingEvent = new UnityEvent();
			weaponSO.GetShootComponent().OnCanceledChargeEvent = new UnityEvent();
			weaponSO.GetProjectileComponent().OnProjectileFiredEvent = new UnityEvent<Projectile>();

			weaponSO.GetShootComponent().IsFiring = false;
			weaponSO.GetShootComponent().OnCooldown = false;

			mouseButtonPressedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonPressedHandler);
			mouseButtonReleasedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonReleasedHandler);
			shootProjectileEvent.AddListener(weaponSO.GetProjectileComponent().OnShootProjectileHandler);

			weaponSO.GetShootComponent().OnShootEvent.AddListener(OnShootHandler);
			weaponSO.GetShootComponent().OnStoppedHoldingEvent.AddListener(weaponSO.GetProjectileComponent().OnStoppedHoldingHandler);

			weaponSO.GetProjectileComponent().OnProjectileFiredEvent.AddListener(weaponSO.GetOnHitComponent().OnProjectileFiredHandler);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !GameManager.Instance.IsGamePaused)
		{
			mouseButtonPressedEvent.Invoke();
		}
		if (Input.GetMouseButtonUp(0) && !GameManager.Instance.IsGamePaused)
		{
			mouseButtonReleasedEvent.Invoke();
		}

		CheckChangeWeapon();

		Vector2 shootFromPos = cam.WorldToScreenPoint(shootFrom.position);
		if (isFacingRight && Input.mousePosition.x < shootFromPos.x || !isFacingRight && Input.mousePosition.x > shootFromPos.x)
		{
			isFacingRight = !isFacingRight;
			playerGraphics.Rotate(playerGraphics.up, 180f);
			playerAnimator.SetBool("FacingRight", isFacingRight);
		}
	}

	private void CheckChangeWeapon()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ChangeWeapon(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ChangeWeapon(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ChangeWeapon(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			ChangeWeapon(3);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			ChangeWeapon(4);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			ChangeWeapon(5);
		}
	}

	private void ChangeWeapon(int i)
	{
		
		if (weaponsSO.Count > i && weaponSO != weaponsSO[i])
		{
			weaponSO = weaponsSO[i];
			InitWeapon();
		}
	}
}
