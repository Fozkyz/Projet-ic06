using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class ShootComponent
{
	public UnityEvent shootEvent;
	public UnityEvent stoppedHoldingEvent;
	public UnityEvent canceledChargeEvent;

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
				break;

			case ShootType.HOLD:
				shootEvent.Invoke();
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
		isFiring = false;
	}

	async Task StartFiringFullAuto()
	{
		while(isFiring)
		{
			shootEvent.Invoke();
			await Task.Delay((int) (1000 / fireRate));
		}
	}

	async Task StartFiringSemiAuto()
	{
		shootEvent.Invoke();
		await Task.Delay((int)(1000 / fireRate));	
	}

	async Task StartFiringCharged()
	{
		var end = Time.time + chargeTime;
		while (Time.time < end)
		{
			if (!isFiring)
			{
				canceledChargeEvent.Invoke();
				return;
			}
			await Task.Yield();
		}
		shootEvent.Invoke();
	}
}
