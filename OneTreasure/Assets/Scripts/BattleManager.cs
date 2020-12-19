using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
	public GameObject[] playerTeam;
	public GameObject[] enemyTeam;

	public Transform[] playerTeamSpawn;
	public Transform[] enemyTeamSpawn;

	public Button[] playerButtons;
	public Button[] enemyButtons;

	GameObject[] playerClones = new GameObject[3];
	GameObject[] enemyClones = new GameObject[3];
	GameObject playerObj;
	GameObject enemyObj;

	Entity player;
	Entity enemy;
	Button playerB;
	Button enemyB;

	int playerDead = 0;
	int enemyDead = 0;
	int playerAttacked = 0;

	public Text selectedCharacter;
	public Text selectedEnemy;
	public Text dialogueText;

	public BattleState state;

	

	void Awake()
    {
		for(int i = 0; i < 3; i++)
        {
			playerButtons[i].image.sprite = playerTeam[i].GetComponent<SpriteRenderer>().sprite;
			enemyButtons[i].image.sprite = enemyTeam[i].GetComponent<SpriteRenderer>().sprite;
			
		}
    }

    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
	}

    IEnumerator SetupBattle()
	{
		for(int i = 0; i < 3; i++)
			playerClones[i] = Instantiate(playerTeam[i], playerTeamSpawn[i]);

		for (int i = 0; i < 3; i++)
			enemyClones[i] = Instantiate(enemyTeam[i], enemyTeamSpawn[i]);

		player = playerClones[1].GetComponent<Entity>();
		enemy = enemyClones[1].GetComponent<Entity>();

		selectedCharacter.text = player.name;
		selectedEnemy.text = enemy.name;

		yield return new WaitForSeconds(1f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack(bool ranged)
	{
		dialogueText.text = player.name + " attacked " + enemy.name + " successfully!";
		playerAttacked++;

        if (ranged)
        {
			if (enemy.TakeDamage(player.rangedDamage))
				enemyDead++;
			PlayShootAnim(playerObj);
		}
        else
        {
			if (enemy.TakeDamage(player.meleeDamage))
				enemyDead++;
			PlaySwordAnim(playerObj);
		}
		yield return new WaitForSeconds(1f);
		PlayIdleAnim(playerObj);
		//SetHP(enemyB.GetComponent<Slider>(), enemy.currentHP); // Slider values
		ToggleButton(playerB, false);

		if (enemyDead == 3)
		{
			state = BattleState.WON;
			EndBattle();
		}
		else if (checkPlayerTurns())
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
			StopCoroutine(PlayerAttack(true));

		}

		Debug.Log(state);
		yield return new WaitForSeconds(1f);
	}

	IEnumerator EnemyTurn()
	{
		foreach(GameObject obj in enemyClones)
        {
			enemy = obj.GetComponent<Entity>();
            if (enemy.dead)
                continue;

			player = (!playerClones[0].GetComponent<Entity>().dead ? playerClones[0].GetComponent<Entity>() : (!playerClones[1].GetComponent<Entity>().dead ? playerClones[1].GetComponent<Entity>() : playerClones[2].GetComponent<Entity>()));
			Debug.Log(enemy.name + " attacked " + player.name + " successfully!");

			switch(Random.Range(0, 1))
            {
				case 0:
					if(player.TakeDamage(enemy.rangedDamage))
						playerDead++;
					PlayShootAnim(obj);
					break;
				case 1:
					if (player.TakeDamage(enemy.meleeDamage))
						playerDead++;
					PlaySwordAnim(obj);
					break;
			}
			yield return new WaitForSeconds(1f);
			PlayIdleAnim(obj);
			//SetHP(playerB.GetComponent<Slider>(), player.currentHP); // Slider values
		}

		Debug.Log("Attack 3 random");
		if (playerDead == 3)
		{
			state = BattleState.LOST;
			EndBattle();
		}
        else
        {
			state = BattleState.PLAYERTURN;
			EnableButtons();
			playerAttacked = 0;
			StopCoroutine(EnemyTurn());
		}

		yield return new WaitForSeconds(2f);
	}

	void PlayerTurn()
	{
		dialogueText.text = "Select a character to attack!";
	}

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
			//Load scene next level.
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN || enemy.dead)
			return;
		StartCoroutine(PlayerAttack(false));
	}

	public void OnShootButton()
	{
		if (state != BattleState.PLAYERTURN || enemy.dead)
			return;
		StartCoroutine(PlayerAttack(true));
	}

	public void OnSelectCharacter(Button button)
    {
        switch (button.name)
        {
			case "Teammate 1":
				player = playerClones[0].GetComponent<Entity>();
				playerB = button;
				playerObj = playerClones[0];
				break;
			case "Teammate 2":
				player = playerClones[1].GetComponent<Entity>();
				playerB = button;
				playerObj = playerClones[1];
				break;
			case "Teammate 3":
				player = playerClones[2].GetComponent<Entity>();
				playerB = button;
				playerObj = playerClones[2];
				break;
			case "Enemy 1":
				enemy = enemyClones[0].GetComponent<Entity>();
				enemyB = button;
				enemyObj = enemyClones[0];
				break;
			case "Enemy 2":
				enemy = enemyClones[1].GetComponent<Entity>();
				enemyB = button;
				enemyObj = enemyClones[1];
				break;
			case "Enemy 3":
				enemy = enemyClones[2].GetComponent<Entity>();
				enemyB = button;
				enemyObj = enemyClones[2];
				break;
		}
		UpdateUI();
	}

	void UpdateUI()
    {
		selectedCharacter.text = player.name;
		selectedEnemy.text = enemy.name;
	}
	void SetHP(Slider s, int hp)
	{
		s.value = hp;
	}
	void ToggleButton(Button button, bool b)
    {
		button.interactable = b;
		button.image.raycastTarget = b;
    }

	void EnableButtons() 
	{ 
		foreach(Button b in playerButtons)
			ToggleButton(b, true);
    }

	bool checkPlayerTurns() { return playerAttacked == 3 - playerDead;  }

	void PlaySwordAnim(GameObject obj)
    {
		Animator anim = obj.GetComponent<Animator>();
		anim.SetTrigger("Stab");
    }

	void PlayShootAnim(GameObject obj)
	{
		Animator anim = obj.GetComponent<Animator>();
		anim.SetTrigger("Shoot");
	}

	void PlayIdleAnim(GameObject obj)
	{
		Animator anim = obj.GetComponent<Animator>();
		anim.SetTrigger("Idle");
	}
}