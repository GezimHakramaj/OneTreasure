using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
	public Slider hpSlider;

	public string unitName;

	public int meleeDamage;
	public int rangedDamage;

	public int maxHP;
	public int currentHP;

	public bool dead;
    private void Awake()
    {
		currentHP = maxHP;
    }
	public bool TakeDamage(int dmg)
    {
		currentHP -= dmg;
		dead = currentHP <= 0;
		return dead;
	}

	public void SetHUD(Entity entity)
	{
		hpSlider.maxValue = maxHP;
		hpSlider.value = currentHP;
	}
}
