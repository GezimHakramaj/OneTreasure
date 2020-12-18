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

	Entity player;
	Entity enemy;
	Button playerB;
	Button enemyB;

	Slider slider;

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

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack(bool ranged)
	{
		dialogueText.text = player.name + " attacked " + enemy.name + " successfully!";
		playerAttacked++;
		
		if (ranged ? (enemy.TakeDamage(player.rangedDamage)) : enemy.TakeDamage(player.meleeDamage))
			enemyDead++;
		//SetHP(enemyB.GetComponent<Slider>(), enemy.currentHP); // Slider values
		ToggleButton(playerB, false);

		yield return new WaitForSeconds(2f);

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
		yield return new WaitForSeconds(2f);
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
			if (Random.Range(0, 1) == 0 ? player.TakeDamage(enemy.rangedDamage) : player.TakeDamage(enemy.meleeDamage))
				playerDead++;
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
				break;
			case "Teammate 2":
				player = playerClones[1].GetComponent<Entity>();
				playerB = button;
				break;
			case "Teammate 3":
				player = playerClones[2].GetComponent<Entity>();
				playerB = button;
				break;
			case "Enemy 1":
				enemy = enemyClones[0].GetComponent<Entity>();
				enemyB = button;
				break;
			case "Enemy 2":
				enemy = enemyClones[1].GetComponent<Entity>();
				enemyB = button;
				break;
			case "Enemy 3":
				enemy = enemyClones[2].GetComponent<Entity>();
				enemyB = button;
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

	bool checkPlayerTurns() { return playerAttacked == 3;  }
}