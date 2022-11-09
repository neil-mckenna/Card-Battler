using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    public enum AITYPE{
        placeFromDeck, handRandomPlace, handDefensive, handAttacking
    }

    public AITYPE enemyAIType;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;
    public Transform cardSpawnPoint;

    private List<CardScriptableObject> cardsInHands = new List<CardScriptableObject>();
    public int startHandSize = 5;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetupDeck();

        if(enemyAIType != AITYPE.placeFromDeck)
        {
            SetupHand();
        }
        
    }

    private void Update()
    {
        
    }

    public void SetupDeck()
    {
        activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int interations = 0;
        while (tempDeck.Count > 0 && interations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            interations++;
        }
    }

    public void StartAction()
    {
        StartCoroutine(EnemyActionCo());
    }

    IEnumerator EnemyActionCo()
    {
        if(activeCards.Count == 0)
        {
            SetupDeck();
        }

        yield return new WaitForSeconds(0.5f);

        if(enemyAIType != AITYPE.placeFromDeck)
        {
            for (int i = 0; i < BattleController.instance.cardsToDrawPerTurn; i++)
            {
                cardsInHands.Add(activeCards[0]);
                activeCards.RemoveAt(0);

                if(activeCards.Count == 0)
                {
                    SetupDeck();

                }

            }
        }

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();

        cardPoints.AddRange(CardPointsController.instance.enemyCardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);

        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        if(enemyAIType == AITYPE.placeFromDeck || enemyAIType == AITYPE.handRandomPlace)
        {
            cardPoints.Remove(selectedPoint);

            while (selectedPoint.activeCard != null && cardPoints.Count > 0)
            {
                randomPoint = Random.Range(0, cardPoints.Count);
                selectedPoint = cardPoints[randomPoint];
                cardPoints.RemoveAt(randomPoint);
            }

        }

        CardScriptableObject selectedCard = null;
        int iterations = 0;
        List<CardPlacePoint> prefferedPoints = new List<CardPlacePoint>();
        List<CardPlacePoint> secondaryPoints = new List<CardPlacePoint>();
   
        switch(enemyAIType)
        {
            case AITYPE.placeFromDeck:

                if (selectedPoint.activeCard == null)
                {
                    Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
                    newCard.cardSO = activeCards[0];
                    activeCards.RemoveAt(0);
                    newCard.SetupCard();

                    yield return new WaitForSeconds(1f);

                    newCard.MoveToPoint(selectedPoint.transform.position, selectedPoint.transform.rotation);

                    yield return new WaitForSeconds(1f);
                    selectedPoint.activeCard = newCard;
                    newCard.assignedPlace = selectedPoint;
                }

                break;

            case AITYPE.handRandomPlace:

                selectedCard = SelectedCardToPlay();

                iterations = 50;
                while(selectedCard != null && iterations > 0 && selectedPoint.activeCard == null)
                {
                    PlayCard(selectedCard, selectedPoint);

                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.timeBetweenAttacks);

                    while (selectedPoint.activeCard != null && cardPoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPoints.Count);
                        selectedPoint = cardPoints[randomPoint];
                        cardPoints.RemoveAt(randomPoint);
                    }

                }
                
                break;

            case AITYPE.handDefensive:

                selectedCard = SelectedCardToPlay();

                prefferedPoints.Clear();
                secondaryPoints.Clear();

                for(int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerCardPoints[i].activeCard != null)
                        {
                            prefferedPoints.Add(cardPoints[i]);

                        }
                        else
                        {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }


                iterations = 50;
                while(selectedCard != null && iterations > 0 && prefferedPoints.Count + secondaryPoints.Count > 0)
                {
                    if(prefferedPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, prefferedPoints.Count);

                        selectedPoint = prefferedPoints[selectPoint];
                        prefferedPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);

                        selectedPoint = secondaryPoints[selectPoint];
                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.timeBetweenAttacks);
                }

                break;

            case AITYPE.handAttacking:

                selectedCard = SelectedCardToPlay();

                prefferedPoints.Clear();
                secondaryPoints.Clear();

                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerCardPoints[i].activeCard == null)
                        {
                            prefferedPoints.Add(cardPoints[i]);

                        }
                        else
                        {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }


                iterations = 50;
                while (selectedCard != null && iterations > 0 && prefferedPoints.Count + secondaryPoints.Count > 0)
                {
                    if (prefferedPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, prefferedPoints.Count);

                        selectedPoint = prefferedPoints[selectPoint];
                        prefferedPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);

                        selectedPoint = secondaryPoints[selectPoint];
                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.timeBetweenAttacks);
                }


                break;
        
        }
        

        yield return new WaitForSeconds(0.5f);


        BattleController.instance.AdvanceTurn();


    }

    void SetupHand()
    {
        for(int i = 0; i < startHandSize; i++)
        {
            if(activeCards.Count == 0)
            {
                SetupDeck();
            }

            cardsInHands.Add(activeCards[0]);
            activeCards.RemoveAt(0);
        }

    }

    public void PlayCard(CardScriptableObject cardSO, CardPlacePoint placePoint)
    {
        Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
        newCard.cardSO = cardSO;
        
        newCard.SetupCard();
        newCard.MoveToPoint(placePoint.transform.position, placePoint.transform.rotation);

        placePoint.activeCard = newCard;
        newCard.assignedPlace = placePoint;

        cardsInHands.Remove(cardSO);

        BattleController.instance.SpendEnemyMana(cardSO.manaCost);

        // Card Attack SFX
        AudioManager.instance.PlaySFX(4);

    }

    CardScriptableObject SelectedCardToPlay()
    {
        CardScriptableObject cardToPlay = null;

        List<CardScriptableObject> cardsToPlay = new List<CardScriptableObject>();
        
        foreach(CardScriptableObject card in cardsInHands)
        {
            if(card.manaCost <= BattleController.instance.enemyMana)
            {
                cardsToPlay.Add(card);
            }
        }

        if(cardsToPlay.Count > 0)
        {
            int selected = Random.Range(0, cardsToPlay.Count);

            cardToPlay = cardsToPlay[selected];
        }


        return cardToPlay;

    }







}
