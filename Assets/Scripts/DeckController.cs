using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;
    public Transform topOfDeckPos;

    public int drawCardBaseCost = 2;

    public float waitBetweenDrawingCards = 0.45f;
    


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupDeck();

        
        
    }




    public void SetupDeck()
    {
        activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int interations = 0;
        while(tempDeck.Count > 0 && interations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);
            
            interations++;

        }

    }

    public void DrawCardToHand()
    {
        if(activeCards.Count == 0)
        {
            SetupDeck();
        }

        Card newCard =  Instantiate(cardToSpawn, topOfDeckPos.position, topOfDeckPos.rotation);

        newCard.cardSO = activeCards[0];
        newCard.SetupCard();

        activeCards.RemoveAt(0);

        HandController.instance.AddCardToHand(newCard);

        // Draw Card SFX
        AudioManager.instance.PlaySFX(3);

    }

    public void DrawCardForMana()
    {
        if(BattleController.instance.playerMana >= drawCardBaseCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerMana(drawCardBaseCost);

        }
        else
        {
            UIController.instance.ShowManaWarning();
            UIController.instance.drawCardButton.SetActive(false);

        }

    }

    public void DrawMultipleCards(int amountToDraw)
    {
        StartCoroutine(DrawMultipleCo(amountToDraw));
    }

    IEnumerator DrawMultipleCo(int amountToDraw)
    {
        yield return new WaitForSeconds(waitBetweenDrawingCards);

        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand();

            yield return new WaitForSeconds(waitBetweenDrawingCards);
        }


    }


}
