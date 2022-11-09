using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("General UI Settings")]
    public GameObject manaWarningObj;
    public float manaWarningTime = 1.0f;
    private float manaWarningCounter;
    public GameObject drawCardButton;
    public GameObject endTurnButton;

    [Header("Damage Indicator and UI Text")]
    public TMP_Text playerManaText, enemyManaText, playerHealthText, enemyHealthText;
    public UIDamageIndicator playerDamageIndicator;
    public UIDamageIndicator enemyDamageIndicator;

    [Header("Battle Ended Settings")]
    public GameObject battleEndScreen;
    public TMP_Text battleResultText;


    [Header("Scene Settings")]
    public string mainMenuScene = "Main Menu";
    public string battleSelectScene = "Battle_Select";
    public GameObject pauseScreen;



    private void Awake()
    {
        instance = this;
        
    }

    


    private void Update()
    {
        if (manaWarningCounter > 0)
        {
            manaWarningCounter -= Time.deltaTime;

            if(manaWarningCounter <= 0)
            {
                manaWarningObj.SetActive(false);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void SetPlayerManaText(int manaAmount)
    {
        string manaStr = manaAmount.ToString();
        playerManaText.text =  $"Mana: {manaStr}";

    }

    public void SetEnemyManaText(int manaAmount)
    {
        string manaStr = manaAmount.ToString();
        enemyManaText.text = $"Mana: {manaStr}";

    }

    public void SetPlayerHealthText(int amount)
    {
        string amtStr = amount.ToString();
        playerHealthText.text = $"Player Health: {amtStr}";
    }

    public void SetEnemyHealthText(int amount)
    {
        string amtStr = amount.ToString();
        enemyHealthText.text = $"Enemy Health: {amtStr}";
    }

    public void ShowManaWarning()
    {
        manaWarningObj.SetActive(true);
        manaWarningCounter = manaWarningTime;

    }

    public void DrawCard()
    {
        DeckController.instance.DrawCardForMana();
        // Button Press SFX
        AudioManager.instance.PlaySFX(0);
    }

    public void EndTurn()
    {
        BattleController.instance.EndPlayerTurn();
        // Button Press SFX
        AudioManager.instance.PlaySFX(0);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1f;

        // Button Press SFX
        AudioManager.instance.PlaySFX(0);

    }

    public void RestartLevel()
    {
        var curScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(curScene.name);
        Time.timeScale = 1f;

        // Button Press SFX
        AudioManager.instance.PlaySFX(0);
    }

    public void ChooseNewBattle()
    {
        SceneManager.LoadScene(battleSelectScene);
        Time.timeScale = 1f;

        // Button Press SFX
        AudioManager.instance.PlaySFX(0);

    }

    public void PauseUnpause()
    {
        if(pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }

        // Button Press SFX
        AudioManager.instance.PlaySFX(0);



    }



}
