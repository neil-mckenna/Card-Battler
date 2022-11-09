using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Scripatble Object")]
    public CardScriptableObject cardSO;

    [Header("Variables: integers")]
    public int currentHealth = 0;
    public int attackPower = 0;
    public int manaCost = 0;

    [Header("Connect Text to UI elements")]
    public TMP_Text healthText, attackText, manaCostText;

    public TMP_Text nameText, actionDescriptionText, loreText;

    public Image characterArt, bgArt;

    public bool isPlayer;

    
    private Vector3 targetPoint;
    private Quaternion targetRot;

    public float moveSpeed = 5f;
    public float rotateSpeed = 540f;
    private Transform deckStartingPos;

    

    
    public bool inHand = false;
    public int handPosition = 0;

    private HandController handController;

    private bool isSelected = false;
    private Collider theCol;

    public LayerMask whatIsDesktop;
    public LayerMask whatIsPlacement;

    private bool justPressed = false;

    public CardPlacePoint assignedPlace;

    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        if(targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }

        handController = FindObjectOfType<HandController>();
        theCol = GetComponent<Collider>();

        SetupCard();

    }

    // Update is called once per frame
    void Update()
    {
        var delta = Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * delta * 0.9f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * delta * 0.3f);

        if(isSelected && BattleController.instance.battleEnded == false && Time.timeScale != 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0f, 1.2f, 0f), Quaternion.identity);

            }

            // right click
            if(Input.GetMouseButtonDown(1) && BattleController.instance.battleEnded == false)
            {
                ReturnToHand();

            }

            // place card left click
            if (Input.GetMouseButtonDown(0) && justPressed == false && BattleController.instance.battleEnded == false)
            {

                if(Physics.Raycast(ray, out hit, 100f, whatIsPlacement) && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
                {
                    CardPlacePoint selectedPoint = hit.collider.gameObject.GetComponent<CardPlacePoint>();
                    
                    if(selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        if(BattleController.instance.playerMana >= manaCost)
                        {
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;

                            MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
                            inHand = false;
                            isSelected = false;

                            handController.RemoveCardFromHand(this);

                            BattleController.instance.SpendPlayerMana(manaCost);

                            // Place Card SFX
                            AudioManager.instance.PlaySFX(4);

                        }
                        else
                        {

                            ReturnToHand();
                            UIController.instance.ShowManaWarning();
                        }

                    }
                    else
                    {
                        ReturnToHand();
                    }


                }
                else
                {
                    ReturnToHand();

                }
            }

        }

        justPressed = false;

    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;

        UpdateCardDisplay();
        
        nameText.text = cardSO.cardName.ToString();
        actionDescriptionText.text = cardSO.actionDescription.ToString();
        loreText.text = cardSO.cardLore.ToString();

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;

    }


    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;

    }

    private void OnMouseOver()
    {
        if (inHand && isPlayer && BattleController.instance.battleEnded == false)
        {
            MoveToPoint(handController.cardPositions[handPosition] + new Vector3(0f, 1f, 1.1f), Quaternion.identity);

        }

    }

    private void OnMouseExit()
    {
        if (inHand && isPlayer && BattleController.instance.battleEnded == false)
        {
            MoveToPoint(handController.cardPositions[handPosition], handController.minPos.rotation);

        }

    }

    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive 
            && isPlayer && BattleController.instance.battleEnded == false
            && Time.timeScale != 0f)
        {
            isSelected = true;
            theCol.enabled = false;
            justPressed = true;

        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;

        MoveToPoint(handController.cardPositions[handPosition], handController.minPos.rotation);

    }

    public void DamageCard(int damageAmount, string who)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
        {
            currentHealth = 0;

            assignedPlace.activeCard = null;

            if(who == "Enemy")
            {
                MoveToPoint(BattleController.instance.playerDiscardPoint.position + BattleController.instance.discardFloatOffset, BattleController.instance.playerDiscardPoint.rotation);

            }
            
            if(who == "Player")
            {
                MoveToPoint(BattleController.instance.enemyDiscardPoint.position + BattleController.instance.discardFloatOffset, BattleController.instance.enemyDiscardPoint.rotation);

            }



            

            anim.SetTrigger("Jump");
            Destroy(gameObject, 3f);

            // Card Defeat SFX
            AudioManager.instance.PlaySFX(2);
        }
        else
        {
            // Card Attack SFX
            AudioManager.instance.PlaySFX(1);
        }

        anim.SetTrigger("Hurt");

        UpdateCardDisplay();

       
    }

    public void UpdateCardDisplay()
    {
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        manaCostText.text = manaCost.ToString();

    }


}
