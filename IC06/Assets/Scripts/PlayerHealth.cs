using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] private int _maxHealth;
	[SerializeField] private float _invincibilityTime;
	
	[Header("UI")]
	[SerializeField] private RawImage _heartImage;
	[SerializeField] private Transform _heartContainerUI;
	[SerializeField] private TextMeshProUGUI moneyText;

	[SerializeField] private Vector2 _offset;
	[SerializeField] private Vector2 _spaceBetweenImages;

	[SerializeField] private GameObject _gameOverScreen;

	private int _currentHealth;
	private PlayerController _player;

	private float _lastTimeHit;

	private List<RawImage> _heartImages;

	private int currentMoney;

	public void TakeDamage()
	{
		if (_lastTimeHit + _invincibilityTime < Time.time)
		{
			_lastTimeHit = Time.time;
			_currentHealth--;
			_heartImages[_currentHealth % _heartImages.Count].enabled = false;
			if (_currentHealth == 0)
			{
				// Game over
				_player.Deactivate();
				_gameOverScreen.SetActive(true);
				GameManager.Instance.GameOver();
			}
		}
	}

	public void Heal()
	{
		if (_currentHealth < _maxHealth)
		{
			_currentHealth++;
			_heartImages[_currentHealth - 1].enabled = true;
		}
	}

	public void SetHealth(int health)
	{
		_currentHealth = health;
		for (int i = 0; i < _maxHealth; i++)
		{
			if (i < _currentHealth)
			{
				_heartImages[i].enabled = true;
			}
			else
			{
				_heartImages[i].enabled = false;
			}
		}
	}

	public int GetCurrentMoney()
	{
		return currentMoney;
	}

	public void PayMoney(int amount)
	{
		currentMoney -= amount;
		moneyText.text = currentMoney.ToString();
	}

	public void AddMoney(int amount)
	{
		currentMoney += amount;
		moneyText.text = currentMoney.ToString();
	}

	public int GetMoney()
	{
		return currentMoney;
	}

	public int GetHealth()
	{
		return _currentHealth;
	}

	public void InteractWithStand(WeaponStand weaponStand)
	{
		Weapon weapon = GetComponent<Weapon>();
		if (weapon != null)
		{
			weapon.SetWeaponSO(weaponStand.GetWeapon());
		}
	}

	public void InteractWithStand(HealthStand healthStand)
	{
		Heal();
	}

	public void InteractWithStand(AugmentStand augmentStand)
	{
		Weapon weapon = GetComponent<Weapon>();
		if (weapon != null)
		{
			weapon.AddAugment(augmentStand.GetAugment());
		}
	}

	private void Awake()
	{
		currentMoney = 1000;
		_currentHealth = _maxHealth;
		_heartImages = new List<RawImage>();
		_player = GetComponent<PlayerController>();
		_gameOverScreen.SetActive(false);
	}

	private void Start()
	{
		PayMoney(0);
		SetupUI();
	}

	private void SetupUI()
	{
		if (_heartContainerUI != null && _heartImage != null)
		{
			for (int i = 0; i < _maxHealth; i++)
			{
				RawImage heartImage = Instantiate(_heartImage, _heartContainerUI);
				heartImage.rectTransform.anchoredPosition = (new Vector2(heartImage.rectTransform.sizeDelta.x, 0) + _spaceBetweenImages) * i + _offset;
				_heartImages.Add(heartImage.GetComponentsInChildren<RawImage>()[1]);
			}
		}
	}
}
