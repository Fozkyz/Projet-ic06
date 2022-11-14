using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class ShootComponent
{
	public UnityEvent OnShootEvent;
	public UnityEvent OnStoppedHoldingEvent;
	public UnityEvent OnCanceledChargeEvent;

	enum ShootType { FULL_AUTO, SEMI_AUTO, HOLD, CHARGED};
	[SerializeField] private ShootType shootType;

	[SerializeField] private float fireRate;
	[SerializeField] private float chargeTime;

	private bool isFiring;
	private bool onCooldown;

	Task shootTask;

	public void OnMouseButtonPressedHandler()
	{
		isFiring = true;
		switch(shootType)
		{
			case ShootType.FULL_AUTO:
				if (!onCooldown)
				{
					shootTask = StartFiringFullAuto();
				}
				break;

			case ShootType.SEMI_AUTO:
				if (!onCooldown)
				{
					shootTask = StartFiringSemiAuto();
				}
				break;

			case ShootType.HOLD:
				OnShootEvent.Invoke();
				break;

			case ShootType.CHARGED:
				if (!onCooldown)
				{
					shootTask = StartFiringCharged();
				}
				break;
		}
	}

	public void OnMouseButtonReleasedHandler()
	{
		if (shootType == ShootType.HOLD)
		{
			OnStoppedHoldingEvent.Invoke();
		}
		isFiring = false;
	}

	async Task StartFiringFullAuto()
	{
		onCooldown = true;
		while(isFiring)
		{
			OnShootEvent.Invoke();
			await Task.Delay((int) (1000 / fireRate));
		}
		onCooldown = false;
	}

	async Task StartFiringSemiAuto()
	{
		onCooldown = true;
		OnShootEvent.Invoke();
		await Task.Delay((int)(1000 / fireRate));
		onCooldown = false;
	}

	async Task StartFiringCharged()
	{
		onCooldown = true;
		var end = Time.time + chargeTime;
		while (Time.time < end)
		{
			if (!isFiring)
			{
				OnCanceledChargeEvent.Invoke();
				return;
			}
			await Task.Yield();
		}
		OnShootEvent.Invoke();
		onCooldown = false;
	}
}
