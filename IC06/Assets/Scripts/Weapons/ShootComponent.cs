using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

public enum ShootType { FULL_AUTO, SEMI_AUTO, HOLD, CHARGED};
[System.Serializable]
public class ShootComponent
{
	public UnityEvent OnShootEvent;
	public UnityEvent OnStoppedHoldingEvent;
	public UnityEvent OnStartChargingEvent;
	public UnityEvent OnCanceledChargeEvent;

	[SerializeField] private ShootType shootType;

	[SerializeField] private float fireRate;
	[SerializeField] private float chargeTime;

	public bool IsFiring { get; set; }
	public bool OnCooldown { get; set; }

	Task shootTask;

	public void OnMouseButtonPressedHandler()
	{
		IsFiring = true;
		switch(shootType)
		{
			case ShootType.FULL_AUTO:
				if (!OnCooldown)
				{
					shootTask = StartFiringFullAuto();
				}
				break;

			case ShootType.SEMI_AUTO:
				if (!OnCooldown)
				{
					shootTask = StartFiringSemiAuto();
				}
				break;

			case ShootType.HOLD:
				OnShootEvent.Invoke();
				break;

			case ShootType.CHARGED:
				if (!OnCooldown)
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
		IsFiring = false;
	}

	public ShootType GetShootType()
	{
		return shootType;
	}

	async Task StartFiringFullAuto()
	{
		OnCooldown = true;
		while(IsFiring)
		{
			OnShootEvent.Invoke();
			await Task.Delay((int) (1000 / fireRate));
		}
		OnCooldown = false;
	}

	async Task StartFiringSemiAuto()
	{
		OnCooldown = true;
		OnShootEvent.Invoke();
		await Task.Delay((int)(1000 / fireRate));
		OnCooldown = false;
	}

	async Task StartFiringCharged()
	{
		OnCooldown = true;
		var end = Time.time + chargeTime;
		OnStartChargingEvent.Invoke();
		while (Time.time < end)
		{
			if (!IsFiring)
			{
				OnCanceledChargeEvent.Invoke();
				return;
			}
			await Task.Yield();
		}
		OnShootEvent.Invoke();
		OnCooldown = false;
	}
}
