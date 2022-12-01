using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundComponent
{
	public AudioSource ShootSource { get; set; }
	public AudioSource HitSource { get; set; }
	public AudioSource ExplodeSource { get; set; }
	public bool LoopClip { get; set; }

	[SerializeField] private AudioClip onShootSound;
	[SerializeField] private AudioClip onHitSound;
	[SerializeField] private AudioClip onStartChargingSound;
	[SerializeField] private AudioClip onCancelShootSound;
	[SerializeField] private AudioClip onProjectileExplodeSound;

	public void OnStartChargingHandler()
	{
		if (ShootSource != null && onStartChargingSound != null)
		{
			ShootSource.clip = onStartChargingSound;
			ShootSource.loop = false;
			ShootSource.Play();
		}
	}

	public void OnShootHandler()
	{
		if (ShootSource != null && onShootSound != null)
		{
			ShootSource.clip = onShootSound;
			ShootSource.loop = LoopClip;
			ShootSource.Play();
		}
	}

	public void OnProjectileHitHandler(Projectile proj, Enemy enemy)
	{
		if (HitSource != null && onHitSound != null)
		{
			HitSource.clip = onHitSound;
			HitSource.loop = false;
			HitSource.Play();
		}
	}

	public void OnStopShootingHandler()
	{
		if (ShootSource != null)
		{
			ShootSource.loop = false;
			if (onCancelShootSound != null)
			{
				ShootSource.clip = onCancelShootSound;
				ShootSource.Play();
			}
		}
	}

	public void OnStoppedHoldingHandler()
	{
		if (ShootSource != null)
		{
			ShootSource.Stop();
		}
	}

	public void OnProjectileExplodeHandler(Projectile proj)
	{
		if (ExplodeSource != null && onProjectileExplodeSound != null)
		{
			ExplodeSource.clip = onProjectileExplodeSound;
			ExplodeSource.loop = false;
			ExplodeSource.Play();
		}
	}

	public void OnProjectileFiredHandler(Projectile proj)
	{
		proj.OnLifetimeElapsedEvent.AddListener(OnProjectileExplodeHandler);
	}
}
