using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Card> heldCards = new List<Card>();

    public Transform minPos;
    public Transform maxPos;
    
    

    public List<Vector3> cardPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        SetCardPositionsInHand();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCardPositionsInHand()
    {
        // clear list 
        cardPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;

        if(heldCards.Count > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);

        }

        for (int i = 0; i < heldCards.Count; i++)
        {
            cardPositions.Add(minPos.position + distanceBetweenPoints * i);

            heldCards[i].MoveToPoint(cardPositions[i], minPos.rotation);

            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;

        }

    }

    public void RemoveCardFromHand(Card cardToRemove)
    {
        if(heldCards[cardToRemove.handPosition] == cardToRemove)
        {
            heldCards.RemoveAt(cardToRemove.handPosition);
        }
        else
        {
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the card being removed from hand!");
        }

        SetCardPositionsInHand();

    }

    public void AddCardToHand(Card cardToAdd)
    {
        heldCards.Add(cardToAdd);
        SetCardPositionsInHand();

    }

    public void EmptyHand(bool isPlayer)
    {
        foreach(Card heldCard in heldCards)
        {
            heldCard.inHand = false;
            if(isPlayer)
            {
                heldCard.MoveToPoint(BattleController.instance.playerDiscardPoint.position, heldCard.transform.rotation);
            }
            else
            {
                heldCard.MoveToPoint(BattleController.instance.enemyDiscardPoint.position, heldCard.transform.rotation);
            }
        }

        heldCards.Clear();

    }

    
}
