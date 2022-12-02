using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class InteractableStand : MonoBehaviour
{
	[SerializeField] protected AudioClip _buySound;
	[SerializeField] protected SpriteRenderer _canPlayerPressRenderer;
	[SerializeField] protected TextMeshProUGUI _priceText;
	[SerializeField] protected KeyCode _keyCode;
	[SerializeField] protected int _price;

	protected bool _canBuy;
	private AudioSource _source;
	private bool _canPlayerPress;
	protected PlayerHealth _player;
	
	private void Start()
	{
		_canBuy = true;
		_canPlayerPress = false;
		_canPlayerPressRenderer.enabled = false;
		_source = GetComponent<AudioSource>();
		_source.playOnAwake = false;
		_source.loop = false;
		_source.clip = _buySound;
		SetPriceText();
	}

	private void Update()
	{
		if (Input.GetKeyDown(_keyCode) && _canBuy && _canPlayerPress && _player.GetCurrentMoney() >= _price)
		{
			_player.PayMoney(_price);
			Interact(_player);
			_source.Play();
			_price = 0;
			_priceText.enabled = false;
		}
	}

	protected abstract void Interact(PlayerHealth player);

	protected void SetPriceText()
	{
		_priceText.text = _price.ToString();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerHealth player = collision.GetComponent<PlayerHealth>();
		if (player != null)
		{
			_canPlayerPressRenderer.enabled = true;
			_player = player;
			_canPlayerPress = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		PlayerHealth player = collision.GetComponent<PlayerHealth>();
		if (player != null)
		{
			_canPlayerPressRenderer.enabled = false;
			_canPlayerPress = false;
		}
	}
}
