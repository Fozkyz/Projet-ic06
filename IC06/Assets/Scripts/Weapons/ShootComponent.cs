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

	Task shootTask;

	public void OnMouseButtonPressedHandler()
	{
		isFiring = true;
		switch(shootType)
		{
			case ShootType.FULL_AUTO:
				if (shootTask == null || shootTask.Status.Equals(TaskStatus.RanToCompletion))
				{
					shootTask = StartFiringFullAuto();
				}
				break;

			case ShootType.SEMI_AUTO:
				if (shootTask == null || shootTask.Status.Equals(TaskStatus.RanToCompletion))
				{
					shootTask = StartFiringSemiAuto();
				}
				else
				{
					Debug.Log(shootTask.Exception.Message);
				}
				break;

			case ShootType.HOLD:
				OnShootEvent.Invoke();
				break;

			case ShootType.CHARGED:
				if (shootTask == null || shootTask.Status.Equals(TaskStatus.RanToCompletion))
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
		while(isFiring)
		{
			OnShootEvent.Invoke();
			await Task.Delay((int) (1000 / fireRate));
		}
	}

	async Task StartFiringSemiAuto()
	{
		OnShootEvent.Invoke();
		await Task.Delay((int)(1000 / fireRate));	
	}

	async Task StartFiringCharged()
	{
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
	}
}
