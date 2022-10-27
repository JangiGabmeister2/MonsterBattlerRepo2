using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GameStates { player, enemy, win, lose } //game states
public enum EnemyStates { neutral, attacking, superattack, healing, blocking } //enemy states

public class GameSystem : MonoBehaviour
{
    #region Variables
    [Header("Action Texts")]
    //texts which tell the player which state the game and enemy has entered
    public Text enemyStatesText;
    public Text gameStatesText;

    [Header("Action Buttons")]
    //player's heal and attack buttons, will be disabled when its enemy's turn
    public Button healButton;
    public Button damageButton;
    public Button superDamageButton;
    public Button blockButton;

    [Header("Current States")]
    //tells the player which state the game and enemy has entered
    public GameStates gameState;
    public EnemyStates enemyState;

    //each fighter's classes
    public PlayerFighter _player;
    public EnemyFighter _enemy;

    private int blockOrNot;
    #endregion

    public void Start()
    {
        //when starting, game is firstly player's turn, and enemy is not doing anything
        gameState = GameStates.player;
        enemyState = EnemyStates.neutral;

        //tells player the states for game and enemy
        enemyStatesText.text = "Enemy State: Neutral";
        gameStatesText.text = "Game State: Your Turn";
    }

    #region States
    public void NextGameState()
    {
        //player = player's turn
        //enemy = enemy's turn
        //win = player wins, enemy dies
        //lose = enemy wins, player dies
        switch (gameState)
        {
            case GameStates.player:
                StartCoroutine(PlayerTurn());
                break;
            case GameStates.enemy:
                StartCoroutine(EnemyTurn());
                break;
            case GameStates.win:
                StartCoroutine(Win());
                break;
            case GameStates.lose:
                StartCoroutine(Lose());
                break;
            default:
                break;
        }
    }

    public void NextEnemyState()
    {
        //neutral = not doing anything
        //attacking = attacks player
        //healing = heals self if health is low
        //blocking = blocks player's next turn if that turn is to attack the enemy, will deal 10% of normal damage
        switch (enemyState)
        {
            case EnemyStates.neutral:
                break;
            case EnemyStates.attacking:
                StartCoroutine(Attack());
                break;
            case EnemyStates.superattack:
                StartCoroutine(SuperAttack());
                break;
            case EnemyStates.healing:
                StartCoroutine(Healng());
                break;
            case EnemyStates.blocking:
                StartCoroutine(Blocking());
                break;
            default:
                break;
        }
    }
    #endregion

    #region Game States
    public IEnumerator PlayerTurn()
    {
        //if its player's turn
        if (gameState == GameStates.player)
        {
            yield return new WaitForSeconds(1f);

            _player.healthText.color = Color.white;

            _player.isBlocking = false;

            //lets player know its player's turn
            gameStatesText.text = "Game State: Your Turn";
            gameStatesText.color = Color.red;

            yield return new WaitForSeconds(1f);

            gameStatesText.color = Color.white;

            //enables buttons, used for future turns after enemy's turns
            healButton.interactable = true;
            damageButton.interactable = true;
            blockButton.interactable = true;

            if (_player.isStaminaFull())
            {
                superDamageButton.interactable = true;
            }
        }
        yield return null;
    }

    public IEnumerator EnemyTurn()
    {
        //if its enemy's turn
        if (gameState == GameStates.enemy)
        {
            yield return new WaitForSeconds(1f);

            _enemy.healthText.color = Color.white;
            _player.healthText.color = Color.white;

            _enemy.isBlocking = false;

            //lets player know its enemy's turn
            gameStatesText.text = "Game State: Enemy's Turn";
            gameStatesText.color = Color.red;

            yield return new WaitForSeconds(1f);

            gameStatesText.color = Color.white;

            //enemy decides which action to take according to their health status
            DecideAction();
        }

        yield return null;
    }
    public IEnumerator Win()
    {
        //if player wins
        if (gameState == GameStates.win)
        {
            gameStatesText.text = "Game State: You Won!";
            gameStatesText.color = Color.red;

            yield return new WaitForSeconds(5f);

            gameStatesText.text = "Ending Game...";

            EndGame();
        }
        yield return null;
    }

    public IEnumerator Lose()
    {
        //if player loses
        if (gameState == GameStates.lose)
        {
            gameStatesText.text = "Game State: Enemy Won!";
            gameStatesText.color = Color.red;

            yield return new WaitForSeconds(5f);

            gameStatesText.text = "Ending Game...";

            EndGame();
        }
        yield return null;
    }
    #endregion

    #region Enemy States
    public void DecideAction()
    {
        //checks enemy's health status
        //low = 50% or less of max health
        if (_enemy.isHealthLow())
        {
            if (blockOrNot == 1)
            {
                enemyState = EnemyStates.blocking;
                NextEnemyState();
            }
            else
            {
                //if health is low, requires healing
                enemyState = EnemyStates.healing;
                NextEnemyState();
            }
        }
        else
        {
            if (_enemy.isStaminaFull())
            {
                //if stamina is full, initiates super attack
                enemyState = EnemyStates.superattack;
                NextEnemyState();
            }
            else
            {
                //if stamina is not full, does normal attack
                enemyState = EnemyStates.attacking;
                NextEnemyState();
            }
        }

    }

    #region Normal Attack
    public IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);

        //checks if health is high enough to prioritise attacking
        if (enemyState == EnemyStates.attacking)
        {
            //if enemy's stamina is 50 or more, enemy will do super attack
            enemyStatesText.text = "Enemy State: Attacking";
            enemyStatesText.color = Color.red;

            yield return new WaitForSeconds(1f);

            enemyStatesText.color = Color.white;

            //checks if player is blocking
            if (_player.isBlocking)
            {
                //if true, deals 10% damage
                _player.Damage(1);
            }
            else
            {
                //if false, deals full damage
                _player.Damage(10);
            }

            //increase stamina by 10
            _enemy.stamina += 10;

            _player.healthText.color = Color.red;

            yield return new WaitForSeconds(1f);

            _player.healthText.color = Color.white;

            if (_player.isDead())
            {
                gameState = GameStates.lose;
                NextGameState();
            }

            //after turn, enemy becomes neutral
            enemyState = EnemyStates.neutral;
            NextEnemyState();

            //switches to player's turn
            gameState = GameStates.player;
            NextGameState();
        }
    }
    #endregion
    #region Super Attack
    public IEnumerator SuperAttack()
    {
        yield return new WaitForSeconds(1f);

        //checks if health is high enough to prioritise attacking
        if (enemyState == EnemyStates.superattack)
        {
            //if enemy's stamina is 50 or more, enemy will do super attack
            enemyStatesText.text = "Enemy State: Attacking";
            enemyStatesText.color = Color.red;

            yield return new WaitForSeconds(1f);

            enemyStatesText.color = Color.white;

            //checks if player is blocking
            if (_player.isBlocking)
            {
                //if true, deals 10% damage
                _player.Damage(3);
            }
            else
            {
                //if false, deals full damage
                _player.Damage(30);
            }

            //resets enemy stamina after attack
            _enemy.stamina = 0;

            _player.healthText.color = Color.red;

            yield return new WaitForSeconds(1f);

            _player.healthText.color = Color.white;

            if (_player.isDead())
            {
                gameState = GameStates.lose;
                NextGameState();
            }

            //after turn, enemy becomes neutral
            enemyState = EnemyStates.neutral;
            NextEnemyState();

            //switches to player's turn
            gameState = GameStates.player;
            NextGameState();
        }
    }
    #endregion
    #region Healing
    public IEnumerator Healng()
    {
        yield return new WaitForSeconds(1f);

        //checks if its enemy's turn
        if (gameState == GameStates.enemy)
        {
            //checks if health is low and requires healing
            if (enemyState == EnemyStates.healing)
            {
                //lets player know enemy is healing
                enemyStatesText.text = "Enemy State: Healing";
                enemyStatesText.color = Color.red;

                yield return new WaitForSeconds(1f);

                enemyStatesText.color = Color.white;

                //heals enemy's health by 20
                _enemy.Heal(20);
                _enemy.healthText.color = Color.green;

                yield return new WaitForSeconds(1f);

                _enemy.healthText.color = Color.white;
            }
        }

        //after turn, enemy becomes neutral
        enemyState = EnemyStates.neutral;
        NextEnemyState();

        //switches to player's turn
        gameState = GameStates.player;
        NextGameState();
    }
    #endregion
    #region Blocking
    public IEnumerator Blocking()
    {
        yield return new WaitForSeconds(1f);

        if (gameState == GameStates.enemy)
        {
            if (enemyState == EnemyStates.blocking)
            {
                //lets player know enemy is blocking
                enemyStatesText.text = "Enemy State: Blocking";
                enemyStatesText.color = Color.red;

                yield return new WaitForSeconds(1f);

                enemyStatesText.color = Color.white;

                _enemy.isBlocking = true;
            }
        }
    }
    #endregion
    #endregion

    #region Player Actions
    public void OnAttack()
    {
        //when any button is clicked, disables their interactability, prevents player from clicking buttons during enemy's turns
        healButton.interactable = false;
        damageButton.interactable = false;
        blockButton.interactable = false;
        superDamageButton.interactable = false;

        //checks if enemy is blocking
        if (_enemy.isBlocking)
        {
            //if true, attacks deal 1 damage
            _enemy.Damage(1);
        }
        else
        {
            //if false, attacks deal 10 damage
            _enemy.Damage(10);
        }

        _enemy.healthText.color = Color.red;

        if (_enemy.isDead())
        {
            gameState = GameStates.win;
            NextGameState();
        }
        else
        {
            //increase stamina by 10 after turn
            _player.stamina += 10;

            //switches to enemy's turn
            gameState = GameStates.enemy;
            NextGameState();
        }
    }

    public void OnSuperAttack()
    {
        healButton.interactable = false;
        damageButton.interactable = false;
        blockButton.interactable = false;
        superDamageButton.interactable = false;

        if (_enemy.isBlocking)
        {
            _enemy.Damage(3);

            //super attacks deal 30 damage to enemy's stamina instead
            _enemy.stamina -= 30;
        }
        else
        {
            _enemy.Damage(30);
        }

        _enemy.healthText.color = Color.red;

        if (_enemy.isDead())
        {
            gameState = GameStates.win;
            NextGameState();
        }
        else
        {
            //increase stamina by 10 after turn
            _player.stamina += 10;

            //switches to enemy's turn
            gameState = GameStates.enemy;
            NextGameState();
        }
    }
    public void OnHeal()
    {
        healButton.interactable = false;
        damageButton.interactable = false;
        blockButton.interactable = false;
        superDamageButton.interactable = false;

        _player.healthText.color = Color.green;

        _player.stamina += 10;

        gameState = GameStates.enemy;
        NextGameState();
    }

    public void onBlock()
    {
        healButton.interactable = false;
        damageButton.interactable = false;
        blockButton.interactable = false;
        superDamageButton.interactable = false;

        //sets blocking bool to true
        _player.isBlocking = true;

        _player.stamina += 10;

        gameState = GameStates.enemy;
        NextGameState();
    }
    #endregion
    public void BlockOrNot()
    {
        blockOrNot = Random.Range(0, 1);
    }


    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
