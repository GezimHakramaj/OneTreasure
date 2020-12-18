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

	public Animator anim;
	

	private void Awake()
    {
		currentHP = maxHP;
		anim = GetComponent<Animator>();

    }
	public bool TakeDamage(int dmg)
    {
		currentHP -= dmg;
		anim.SetTrigger("Hurt");
		dead = currentHP <= 0;
		if (dead == true)
		{
			anim.SetTrigger("Dead");
			return dead;
		}
		anim.SetTrigger("Idle");
		return dead;
	}

	public void SetHUD(Entity entity)
	{
		hpSlider.maxValue = maxHP;
		hpSlider.value = currentHP;
	}
}
