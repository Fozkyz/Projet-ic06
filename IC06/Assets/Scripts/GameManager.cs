using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	[field: SerializeField] public PlayerController Player { get; set; }
	public UnityEvent OnGamePausedEvent, OnGameResumeEvent;
	public bool IsGamePaused;

	public static int NbGame { get; set; }
	public static int PlayerHealth { get; set; }
	public static int PlayerMoney { get; set; }
	public static WeaponSO PlayerWeapon { get; set; }
	public static List<Augment> PlayerAugments { get; set; }

	[SerializeField] private GameObject _pauseScreen;
	[SerializeField] private AudioSource _musicSource;

	[SerializeField] private List<AudioClip> _cyberpunkMusics;

	[SerializeField] private List<WeaponSO> weaponsSO;
	[SerializeField] private List<Augment> augmentsSO;

	private Vector2 startPos;
	private int _musicIndex;

	public void GameOver()
	{
		if (!IsGamePaused)
		{
			IsGamePaused = true;
			OnGamePausedEvent.Invoke();
		}
	}

	public void PauseGame()
	{
		if (!IsGamePaused)
		{
			IsGamePaused = true;
			OnGamePausedEvent.Invoke();
			_pauseScreen.SetActive(true);
		}
	}

	public void ResumeGame()
	{
		if (IsGamePaused)
		{
			IsGamePaused = false;
			OnGameResumeEvent.Invoke();
			_pauseScreen.SetActive(false);
		}
	}

	public void ResetPlayerPosition()
	{
		Player.transform.position = startPos;
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void QuitQame()
	{
		Application.Quit();
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void ReloadScene()
	{
		PlayerHealth p = Player.GetComponent<PlayerHealth>();
		if (p != null)
		{
			PlayerHealth = p.GetHealth();
			PlayerMoney = p.GetMoney();
		}
		
		Weapon w = Player.GetComponent<Weapon>();
		if (w != null)
		{
			PlayerWeapon = w.GetWeaponSO();
			PlayerAugments = w.GetAugments();
		}

		NbGame++;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public List<WeaponSO> GetWeaponsSO()
	{
		return weaponsSO;
	}

	public List<Augment> GetAugmentsSO()
	{
		return augmentsSO;
	}

	private void Start()
	{
		if (Player != null)
		{
			startPos = Player.transform.position;
			PlayerHealth p = Player.GetComponent<PlayerHealth>();
			if (p != null && PlayerHealth > 0)
			{
				p.AddMoney(PlayerMoney);
				p.SetHealth(PlayerHealth);
			}
			Weapon w = Player.GetComponent<Weapon>();
			if (w != null && PlayerWeapon != null)
			{
				w.SetWeaponSO(PlayerWeapon);
				foreach (Augment augment in PlayerAugments)
				{
					w.AddAugment(augment);
				}
			}
		}
		if (_pauseScreen != null)
		{
			_pauseScreen.SetActive(false);
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Update()
	{
		if (_musicSource != null && !_musicSource.isPlaying && _cyberpunkMusics.Count > 0)
		{
			_musicIndex = (_musicIndex + 1) % _cyberpunkMusics.Count;
			_musicSource.clip = _cyberpunkMusics[_musicIndex];
			_musicSource.Play();
		}

		if (Input.GetKeyDown(KeyCode.P) && Application.isEditor)
		{
			ReloadScene();
		}
	}
}
