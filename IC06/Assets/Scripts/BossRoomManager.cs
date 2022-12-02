using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class BossRoomManager : RoomManager
{
	[SerializeField] private int _numberOfWaves;
	[SerializeField] private GameObject _portalPrefab;
	[SerializeField] private Transform _portalSpawnPoint;
	[SerializeField] private GameObject _enemyPrefab;
	[SerializeField] private List<Transform> _spawnPoints;

	private int _currentEnemiesCount;
	private int _currentWave;
	private bool _hasBossEncounterHappened;

	protected override void OnPlayerEnteredRoomHandler(PlayerController player, Teleporter teleporter)
	{
		base.OnPlayerEnteredRoomHandler(player, teleporter);
		if (!_hasBossEncounterHappened)
		{
			StartBossEncounter();
			_hasBossEncounterHappened = true;
		}
	}

	protected override void OnPlayerLeftRoomHandler(PlayerController player, Teleporter teleporter)
	{
		base.OnPlayerLeftRoomHandler(player, teleporter);
	}

	protected override void Start()
	{
		base.Start();
		_hasBossEncounterHappened = false;
	}

	private void StartBossEncounter()
	{
		_currentWave = 0;
		TeleporterManager tpManager = GetComponent<TeleporterManager>();
		if (tpManager != null)
		{
			tpManager.LockAllTeleporters();
		}
		StartCoroutine(nameof(DoNextWave));
	}

	private IEnumerator DoNextWave()
	{
		yield return new WaitForSeconds(3);
		if (_currentWave < _numberOfWaves)
		{
			_currentWave++;
			SpawnEnemies();
		}
		else
		{
			EndBossEncounter();
		}
	}

	private void SpawnEnemies()
	{
		foreach (Transform spawnPoint in _spawnPoints)
		{
			GameObject enemyGO = Instantiate(_enemyPrefab, spawnPoint.position, Quaternion.identity);
			Enemy enemy = enemyGO.GetComponentInChildren<Enemy>();
			if (enemy != null)
			{
				_currentEnemiesCount++;
				enemy.SetActive(true);
				enemy.OnEnemyKilledEvent = new UnityEvent<Enemy>();
				enemy.OnEnemyKilledEvent.AddListener(OnEnemyKilledHandler);
			}
		}
	}

	private void OnEnemyKilledHandler(Enemy enemy)
	{
		_currentEnemiesCount--;
		if (_currentEnemiesCount <= 0)
		{
			StartCoroutine(nameof(DoNextWave));
		}
	}

	private void EndBossEncounter()
	{
		TeleporterManager tpManager = GetComponent<TeleporterManager>();
		if (tpManager != null)
		{
			tpManager.UnlockAllTeleporters();
		}
		Instantiate(_portalPrefab, _portalSpawnPoint.position, Quaternion.identity);
	}
}
