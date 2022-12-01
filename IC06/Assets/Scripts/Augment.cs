using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Augment", menuName = "Augment")]
public class Augment : ScriptableObject
{
	[field: SerializeField] public string Name { get; set; }
	[field: SerializeField] public Sprite Sprite { get; set; }
	[field: SerializeField] public List<Effect> Effects { get; set; }
}

[System.Serializable]
public class Effect
{
	[field: SerializeField] public EffectType Type { get; set; }
	[field: SerializeField] public float Value { get; set; }
	[field: SerializeField] public bool BoolValue { get; set; }
}

public enum EffectType
{
	ON_HIT_DAMAGE_MODIFIER,
	ON_HIT_DAMAGE_MULTIPLIER,
	ON_HIT_EXPLOSION_DAMAGE_MODIFIER,
	ON_HIT_EXPLOSION_DAMAGE_MULTIPLIER,
	ON_HIT_EXPLOSION_RADIUS_MODIFIER,
	ON_HIT_EXPLOSION_RADIUS_MULTIPLIER,
	ON_HIT_EXPLOSION_HITS_PLAYER_OVERRIDE,
	PROJECTILE_SPEED_MODIFIER,
	PROJECTILE_SPEED_MULTIPLIER,
	PROJECTILE_RANGE_MODIFIER,
	PROJECTILE_RANGE_MULTIPLIER,
	PROJECTILE_ENEMIES_TO_GO_THROUGH_MODIFIER,
	PROJECTILE_ENEMIES_TO_GO_THROUGH_MULTIPLIER,
	PROJECTILE_NUMBER_OF_BOUNCES_MODIFIER,
	PROJECTILE_NUMBER_OF_BOUNCES_MULTIPLIER,
	FIRE_RATE_MULTIPLIER,
	CHARGE_TIME_MULTIPLIER
}