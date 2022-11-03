using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
	[SerializeField] private Teleporter leftTeleporter;
	[SerializeField] private Teleporter rightTeleporter;
	[SerializeField] private Teleporter topTeleporter;
	[SerializeField] private Teleporter bottomTeleporter;

	public Teleporter GetLeftTeleporter()
	{
		return leftTeleporter;
	}

	public Teleporter GetRightTeleporter()
	{
		return rightTeleporter;
	}

	public Teleporter GetTopTeleporter()
	{
		return topTeleporter;
	}
	
	public Teleporter GetBotTeleporter()
	{
		return bottomTeleporter;
	}
}
