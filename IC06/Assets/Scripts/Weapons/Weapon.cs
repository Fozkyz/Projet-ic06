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
	[SerializeField] private AudioSource explodeSource;
	
	private Animator playerAnimator;
	private List<Augment> augments;
	
	private Camera cam;
	private bool isFacingRight;

    UnityEvent mouseButtonPressedEvent;
    UnityEvent mouseButtonReleasedEvent;

	UnityEvent<Vector2, Transform> shootProjectileEvent;

	public WeaponSO GetWeaponSO()
	{
		return weaponSO;
	}

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
		foreach (Augment augment in augments)
		{
			foreach (Effect effect in augment.Effects)
			{
				ApplyAugmentEffect(effect);
			}
		}
	}

	public void AddAugment(Augment newAugment)
	{
		augments.Add(newAugment);
		foreach (Effect effect in newAugment.Effects)
		{
			ApplyAugmentEffect(effect);
		}
	}

	private void Start()
	{
		cam = Camera.main;
		isFacingRight = true;
		playerAnimator = playerGraphics.GetComponent<Animator>();
		weaponSO = weaponsSO[0];
		augments = new List<Augment>();
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
			weaponSO.GetSoundComponent().ExplodeSource = explodeSource;

			weaponSO.GetShootComponent().OnShootEvent = new UnityEvent();
			weaponSO.GetShootComponent().OnStoppedHoldingEvent = new UnityEvent();
			weaponSO.GetShootComponent().OnStartChargingEvent = new UnityEvent();
			weaponSO.GetShootComponent().OnCanceledChargeEvent = new UnityEvent();

			weaponSO.GetOnHitComponent().OnProjectileExplodesEvent = new UnityEvent<Projectile>();
			weaponSO.GetProjectileComponent().OnProjectileFiredEvent = new UnityEvent<Projectile>();

			weaponSO.GetShootComponent().IsFiring = false;
			weaponSO.GetShootComponent().OnCooldown = false;

			mouseButtonPressedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonPressedHandler);
			mouseButtonReleasedEvent.AddListener(weaponSO.GetShootComponent().OnMouseButtonReleasedHandler);
			shootProjectileEvent.AddListener(weaponSO.GetProjectileComponent().OnShootProjectileHandler);

			weaponSO.GetShootComponent().OnShootEvent.AddListener(OnShootHandler);
			weaponSO.GetShootComponent().OnShootEvent.AddListener(weaponSO.GetSoundComponent().OnShootHandler);
			weaponSO.GetShootComponent().OnStoppedHoldingEvent.AddListener(weaponSO.GetProjectileComponent().OnStoppedHoldingHandler);
			weaponSO.GetShootComponent().OnStoppedHoldingEvent.AddListener(weaponSO.GetSoundComponent().OnStoppedHoldingHandler);
			weaponSO.GetShootComponent().OnStartChargingEvent.AddListener(weaponSO.GetSoundComponent().OnStartChargingHandler);
			weaponSO.GetOnHitComponent().OnProjectileExplodesEvent.AddListener(weaponSO.GetSoundComponent().OnProjectileExplodeHandler);

			weaponSO.GetProjectileComponent().OnProjectileFiredEvent.AddListener(weaponSO.GetOnHitComponent().OnProjectileFiredHandler);
			weaponSO.GetProjectileComponent().OnProjectileFiredEvent.AddListener(weaponSO.GetSoundComponent().OnProjectileFiredHandler);

			weaponSO.GetOnHitComponent().InitComponent();
			weaponSO.GetProjectileComponent().InitComponent();
			weaponSO.GetShootComponent().InitComponent();
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !GameManager.Instance.IsGamePaused)
		{
			mouseButtonPressedEvent.Invoke();
		}
		if (Input.GetMouseButtonUp(0))
		{
			mouseButtonReleasedEvent.Invoke();
		}

		if (Application.isEditor)
			CheckChangeWeapon();

		Vector2 shootFromPos = cam.WorldToScreenPoint(shootFrom.position);
		if (isFacingRight && Input.mousePosition.x < shootFromPos.x || !isFacingRight && Input.mousePosition.x > shootFromPos.x)
		{
			isFacingRight = !isFacingRight;
			playerGraphics.Rotate(playerGraphics.up, 180f);
			playerAnimator.SetBool("FacingRight", isFacingRight);
		}
	}

	private void ApplyAugmentEffect(Effect effect)
	{
		switch (effect.Type)
		{
			case EffectType.ON_HIT_DAMAGE_MODIFIER:
				weaponSO.GetOnHitComponent().OnHitDamageModifier = (int)effect.Value;
				break;
			case EffectType.ON_HIT_DAMAGE_MULTIPLIER:
				weaponSO.GetOnHitComponent().OnHitDamageMultiplier = effect.Value;
				break;
			case EffectType.ON_HIT_EXPLOSION_DAMAGE_MODIFIER:
				weaponSO.GetOnHitComponent().OnHitExplosionDamageModifier = (int)effect.Value;
				break;
			case EffectType.ON_HIT_EXPLOSION_DAMAGE_MULTIPLIER:
				weaponSO.GetOnHitComponent().OnHitExplosionDamageMultiplier = effect.Value;
				break;
			case EffectType.ON_HIT_EXPLOSION_RADIUS_MODIFIER:
				weaponSO.GetOnHitComponent().OnHitExplosionRadiusModifier = (int)effect.Value;
				break;
			case EffectType.ON_HIT_EXPLOSION_RADIUS_MULTIPLIER:
				weaponSO.GetOnHitComponent().OnHitExplosionRadiusMultiplier = effect.Value;
				break;
			case EffectType.ON_HIT_EXPLOSION_HITS_PLAYER_OVERRIDE:
				weaponSO.GetOnHitComponent().OnHitExplosionHitsPlayerOverride = effect.BoolValue;
				break;
			case EffectType.PROJECTILE_SPEED_MODIFIER:
				weaponSO.GetProjectileComponent().ProjectileSpeedModifier = effect.Value;
				break;
			case EffectType.PROJECTILE_SPEED_MULTIPLIER:
				weaponSO.GetProjectileComponent().ProjectileSpeedMultiplier = effect.Value;
				break;
			case EffectType.PROJECTILE_RANGE_MODIFIER:
				weaponSO.GetProjectileComponent().ProjectileRangeModifier = effect.Value;
				break;
			case EffectType.PROJECTILE_RANGE_MULTIPLIER:
				weaponSO.GetProjectileComponent().ProjectileRangeMultiplier = effect.Value;
				break;
			case EffectType.PROJECTILE_ENEMIES_TO_GO_THROUGH_MODIFIER:
				weaponSO.GetProjectileComponent().ProjectileEnemiesToGoThroughModifier = (int)effect.Value;
				break;
			case EffectType.PROJECTILE_ENEMIES_TO_GO_THROUGH_MULTIPLIER:
				weaponSO.GetProjectileComponent().ProjectileEnemiesToGoThroughMultiplier = effect.Value;
				break;
			case EffectType.PROJECTILE_NUMBER_OF_BOUNCES_MODIFIER:
				weaponSO.GetProjectileComponent().ProjectileNumberOfBouncesModifier = (int)effect.Value;
				break;
			case EffectType.PROJECTILE_NUMBER_OF_BOUNCES_MULTIPLIER:
				weaponSO.GetProjectileComponent().ProjectileNumberOfBouncesMultiplier = effect.Value;
				break;
			case EffectType.FIRE_RATE_MULTIPLIER:
				weaponSO.GetShootComponent().FireRateMultiplier = effect.Value;
				break;
			case EffectType.CHARGE_TIME_MULTIPLIER:
				weaponSO.GetShootComponent().ChargeTimeMultiplier = effect.Value;
				break;
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
