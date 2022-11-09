using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public enum TurnOrder
    {
        playerActive, playerCardAttacks, enemyActive, enemyCardAttacks
    }

    public TurnOrder currentPhase;


    public static BattleController instance;

    
    

    [Header("Deck Settings")]
    public Transform enemyDiscardPoint;
    public Transform playerDiscardPoint;

    public Vector3 discardFloatOffset = new Vector3(0f, 0.25f, 0f);

    public int startingMana = 4, maxMana = 12;
    public int playerMana, enemyMana;
    public int currentPlayerMaxMana, currentEnemyMaxMana;
    public int playerHealth = 20;
    public int enemyHealth = 20;
    public bool battleEnded = false;

    [Range(0f, 1f)]
    public float playerFirstChance = 0.5f;

    public int startingCardsAmount = 5;
    public int cardsToDrawPerTurn = 1;

    public float waitTimeForBattleScreen = 1.5f;

    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        currentPlayerMaxMana = startingMana;
        FillPlayerMana();

        currentEnemyMaxMana = startingMana;
        FillEnemyMana();

        UIController.instance.SetPlayerHealthText(playerHealth);
        UIController.instance.SetEnemyHealthText(enemyHealth);
        UIController.instance.SetPlayerManaText(playerMana);
        UIController.instance.SetEnemyManaText(enemyMana);


        DeckController.instance.DrawMultipleCards(startingCardsAmount);

        if(Random.value > playerFirstChance)
        {
            currentPhase = TurnOrder.playerCardAttacks;
            AdvanceTurn();
        }

        AudioManager.instance.PlayBGM();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {

           AdvanceTurn();
            
            
        }
        

    }

    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana -= amountToSpend;

        if (playerMana < 0)
        {
            playerMana = 0;
        }

        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void SpendEnemyMana(int amountToSpend)
    {
        enemyMana -= amountToSpend;

        if (enemyMana < 0)
        {
            enemyMana = 0;
        }

        UIController.instance.SetEnemyManaText(enemyMana);
    }

    public void FillPlayerMana()
    {
        playerMana = currentPlayerMaxMana;
        UIController.instance.SetPlayerManaText(playerMana);

    }

    public void FillEnemyMana()
    {
        enemyMana = currentEnemyMaxMana;
        UIController.instance.SetEnemyManaText(enemyMana);

    }

    public void AdvanceTurn()
    {
        if(battleEnded == false)
        {
            currentPhase++;

            if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
            {
                currentPhase = 0;
            }

            switch (currentPhase)
            {
                case TurnOrder.playerActive:
                    UIController.instance.endTurnButton.SetActive(true);
                    UIController.instance.drawCardButton.SetActive(true);

                    // max 12
                    if (currentPlayerMaxMana < maxMana)
                    {
                        currentPlayerMaxMana++;
                    }

                    FillPlayerMana();

                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);

                    break;

                case TurnOrder.playerCardAttacks:

                    CardPointsController.instance.PlayerAttack();

                    break;

                case TurnOrder.enemyActive:

                    // max 12
                    if (currentEnemyMaxMana < maxMana)
                    {
                        currentEnemyMaxMana++;
                    }

                    FillEnemyMana();

                    EnemyController.instance.StartAction();



                    break;

                case TurnOrder.enemyCardAttacks:
                    CardPointsController.instance.EnemyAttack();


                    break;

            } 
        
        }

    }

    public void EndPlayerTurn()
    {
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawCardButton.SetActive(false);

        AdvanceTurn();

    }

    

    public void DamagePlayer(int damageAmount)
    {
        if(playerHealth > 0 || battleEnded == false)
        {

            playerHealth -= damageAmount;
            if(playerHealth <= 0)
            {
                playerHealth = 0;
                EndBattle();
            }
        }

        UIController.instance.SetPlayerHealthText(playerHealth);

        UIDamageIndicator damageClone = Instantiate(UIController.instance.playerDamageIndicator, UIController.instance.playerDamageIndicator.transform.parent);
        damageClone.damageText.text = damageAmount.ToString();
        damageClone.gameObject.SetActive(true);

        // Damage Player SFX
        AudioManager.instance.PlaySFX(6);
    }

    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0 || battleEnded == false)
        {

            enemyHealth -= damageAmount;
            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                EndBattle();
            }
        }

        UIController.instance.SetEnemyHealthText(enemyHealth);

        UIDamageIndicator damageClone = Instantiate(UIController.instance.enemyDamageIndicator, UIController.instance.enemyDamageIndicator.transform.parent);
        damageClone.damageText.text = damageAmount.ToString();
        damageClone.gameObject.SetActive(true);


        // Damage Enemy SFX
        AudioManager.instance.PlaySFX(5);
    }

    void EndBattle()
    {
        battleEnded = true;

        HandController.instance.EmptyHand(true);

        if(enemyHealth <= 0)
        {
            UIController.instance.battleResultText.text = "Well Done, You Won!";

            foreach(CardPlacePoint point in CardPointsController.instance.enemyCardPoints)
            {
                if(point.activeCard != null)
                {
                    point.activeCard.MoveToPoint(enemyDiscardPoint.position, point.activeCard.transform.rotation);
                }
            }
        }
        else
        {
            UIController.instance.battleResultText.text = "You Lost!, better luck next time!";

            foreach (CardPlacePoint point in CardPointsController.instance.playerCardPoints)
            {
                if (point.activeCard != null)
                {
                    point.activeCard.MoveToPoint(playerDiscardPoint.position, point.activeCard.transform.rotation);
                }
            }
        }

        StartCoroutine(ShowResultCo());

    }

    IEnumerator ShowResultCo()
    {
        yield return new WaitForSeconds(waitTimeForBattleScreen);
        UIController.instance.battleEndScreen.SetActive(true);

    }
}
