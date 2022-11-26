using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundComponent : MonoBehaviour
{
	public AudioSource ShootSource { get; set; }
	public AudioSource HitSource { get; set; }

	[SerializeField] private AudioClip onShootSound;
	[SerializeField] private AudioClip onHitSound;
	
	private void OnShootHandler()
	{
		ShootSource.clip = onShootSound;
		ShootSource.loop = true;
		ShootSource.Play();
	}

	private void OnProjectileHitHandler(Projectile proj, Enemy enemy)
	{
		HitSource.clip = onHitSound;
		HitSource.loop = false;
		HitSource.Play();
	}

	private void OnStopShootingHandler()
	{
		ShootSource.loop = false;
	}

	private void OnStoppedHoldingHandler()
	{
		ShootSource.Stop();
	}
}
