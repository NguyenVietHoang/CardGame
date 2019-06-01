using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuleControl : MonoBehaviour
{
    public delegate void OnTriggerDownEvt(CardModel card);
    public OnTriggerDownEvt onCardClicked;

    public IngameView view;

    public List<CardModel> allCard;
    public List<CardObject> cardsData;
    public List<Transform> plateCardPos;
    public Transform centerPlatePos;
    public List<Transform> playerCardPos;
    public List<Transform> botCarPos;

    public List<Transform> versusCardPos;

    List<int> cardsId;

    List<CardModel> playerCards;
    List<CardModel> botCards;
    CardModel playerCardOnPlate;
    CardModel botCardOnPlate;
    int gamePhase;//0: Set up phase, 1: Player Picking phase, 2: Combat Phase
    int playerCardsNb;
    int botCardsNb;
    int currentTurn;//ident with card team id
    float battleCD;

    int botScore;
    int playerScore;
    private void Start()
    {
        cardsId = Enumerable.Range(0, allCard.Count).ToList();
        cardsId = ShuffleList(cardsId);

        //Setting up all the cards
        for(int i = 0; i < allCard.Count; i++)
        {
            allCard[i].SetCardData(cardsData[cardsId[i]]);
            allCard[i].onLockCard = true;
        }

        playerCards = new List<CardModel>();
        botCards = new List<CardModel>();

        //Initialize the game parameter
        gamePhase = 0;
        playerCardsNb = 0;
        botCardsNb = 0;
        currentTurn = 0;
        playerScore = 0;
        botScore = 0;
        onCardClicked = null;
        battleCD = -1;

        view.OpenIngameUI();
        view.UpdateScore(0, 0);
        view.UpdateNotification("Please wait...");
        view.goPlayAgain.onClick.AddListener(ReplayGame);
        view.goQuit.onClick.AddListener(QuitGame);
        StartCoroutine(ShuffleTheDeckAnimation());
    }

    private void Update()
    {
        if(gamePhase == 2 && battleCD != -1)
        {
            if(battleCD > 0)
            {
                battleCD -= Time.deltaTime;
            }
            else
            {
                CardBattle(currentTurn);
            }
        }
    }

    //Phase 1: picking up card
    void AddToPlayerHand(CardModel card)
    {
        if(playerCardsNb < 5)
        {
            card.MoveCard(playerCardPos[playerCardsNb].position, playerCardPos[playerCardsNb].localScale);
            card.onLockCard = true;
            card.teamID = 1;
            playerCards.Add(card);
            card.ToFaceUp();
            allCard.Remove(card);
            
            //The player already pickup all cards-> all the reaminng card belong to bot
            if (playerCardsNb == 4)
            {
                onCardClicked = null;
                StartCoroutine(BotPickingCard());
            }
            else
            {
                playerCardsNb++;
            }          
        }
    }

    //Phase 2: Card Battle
    void SummonCard(CardModel card)
    {
        if(gamePhase != 2)
        {
            return;
        }
        else
        {
            //At the beginning, bot will play the card, the switch turn to the player
            if(playerCardOnPlate == null && botCardOnPlate == null)
            {
                card.MoveCard(versusCardPos[1].position, versusCardPos[1].localScale);
                card.ToFaceUp();
                currentTurn = 1;
                botCards.Remove(card);
                botCardOnPlate = card;
                LockTheCardTrigger(false);
                view.UpdateNotification("Your turn.");
                Debug.Log("Bot Play first card");
            }
            else
            {
                //Player Turn
                if (currentTurn == 1)
                {
                    card.MoveCard(versusCardPos[0].position, versusCardPos[0].localScale);
                    card.ToFaceUp();
                    playerCardOnPlate = card;
                    playerCards.Remove(card);
                    LockTheCardTrigger(true);
                    Debug.Log("Player play card");
                    StartCoroutine(BriefDelayBeforeCardBattle(1));
                }
                //Bot Turn    
                if (currentTurn == -1)
                {
                    card.MoveCard(versusCardPos[1].position, versusCardPos[1].localScale);
                    card.ToFaceUp();
                    botCards.Remove(card);
                    botCardOnPlate = card;
                    Debug.Log("Bot play card");
                    StartCoroutine(BriefDelayBeforeCardBattle(-1));
                }
            }
        }
    }

    IEnumerator ShuffleTheDeckAnimation()
    {
        yield return new WaitForSeconds(1);
        //Debug.Log("Shuffle");
        //Gather all cards
        foreach (CardModel c in allCard)
        {
            c.MoveCard(centerPlatePos.position, centerPlatePos.localScale);
        }
        yield return new WaitForSeconds(2);

        for(int i = 0; i < allCard.Count; i++)
        {
            allCard[i].MoveCard(plateCardPos[i].position, plateCardPos[i].localScale);
        }

        yield return new WaitForSeconds(2);

        for (int i = 0; i < allCard.Count; i++)
        {
            allCard[i].onLockCard = false;
        }
        gamePhase = 1;
        onCardClicked += AddToPlayerHand;
        view.UpdateNotification("Please pick up 5 cards");
    }

    IEnumerator BotPickingCard()
    {
        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < 5; i++)
        {
            allCard[i].MoveCard(botCarPos[i].position, botCarPos[i].localScale);
            allCard[i].onLockCard = true;
            allCard[i].teamID = -1;
            botCards.Add(allCard[i]);
        }

        allCard.Clear();
        yield return new WaitForSeconds(2);
        //Start the game here
        StartGame();        
    }

    //first attack: the player or the bot
    void CardBattle(int firstAttack)
    {
        currentTurn = firstAttack;
        Debug.Log("Attack : " + firstAttack);
        if (botCardOnPlate.curDef != 0 && playerCardOnPlate.curDef != 0)
        {
            //Player attack the bot
            if (firstAttack == 1)
            {
                playerCardOnPlate.ToFaceUp();
                botCardOnPlate.TakeDmg(playerCardOnPlate.currAtk);
                //The bot card is not dead
                if(botCardOnPlate.curDef > 0)
                {
                    currentTurn = -1;
                    battleCD = 1;
                }
                else
                {
                    battleCD = -1;
                    BattleEndPhase();
                }
            }
            else if(firstAttack == -1)
            {
                botCardOnPlate.ToFaceUp();
                playerCardOnPlate.TakeDmg(botCardOnPlate.currAtk);
                //The player card is not dead
                if (playerCardOnPlate.curDef > 0)
                {
                    currentTurn = 1;
                    battleCD = 1;
                }
                else
                {
                    battleCD = -1;
                    BattleEndPhase();
                }
            }
        }    
    }

    void BattleEndPhase()
    {
        Debug.Log("End Battle.");
        //After the battle, the next person is the defeat one
        if (currentTurn == -1)
        {
            botScore++;
            //The player lost his card, so he need to play the new one
            Debug.Log("Bot Score: " + botScore);
            view.UpdateScore(playerScore, botScore);
            if (EndGameChecking())
            {
                gamePhase = 3;
                StartCoroutine(BriefDelayBeforeGameEnd(false));

                Debug.Log("Game End!");
            }
            else
            {
                view.UpdateNotification("Your turn.");
                LockTheCardTrigger(false);
            }          
        }
        if (currentTurn == 1)
        {
            playerScore++;
            view.UpdateScore(playerScore, botScore);
            Debug.Log("Player Score: " + playerScore);
            if (EndGameChecking())
            {
                gamePhase = 3;
                StartCoroutine(BriefDelayBeforeGameEnd(true));
                Debug.Log("Game End!");
            }
            else
            {
                view.UpdateNotification("Bot's turn.");
                StartCoroutine(BriefDelayBeforeBotPlayCard());
            }           
        }
        currentTurn = currentTurn * -1;     
    }

    bool EndGameChecking()
    {
        if(playerScore >= 5)
        {
            Debug.Log("Player Win");
            view.UpdateNotification("You win.");
            return true;
        }
        if (botScore >= 5)
        {
            Debug.Log("Bot win.");
            return true;
        }
        return false;
    }

    IEnumerator BriefDelayBeforeBotPlayCard()
    {
        yield return new WaitForSeconds(1.0f);
        SummonCard(botCards[0]);
    }

    IEnumerator BriefDelayBeforeCardBattle(int _currTurn)
    {
        yield return new WaitForSeconds(1.0f);
        CardBattle(_currTurn);
    }

    IEnumerator BriefDelayBeforeGameEnd(bool winner)
    {
        yield return new WaitForSeconds(1.0f);
        if (PlayerPrefs.GetInt("High Score") < playerScore)
        {
            PlayerPrefs.SetInt("High Score", playerScore);
        }
        view.OpenGameOverUI();
        if(winner)
            view.UpdateGameOverNoti("Congratulation! \nYour Score:" + playerScore + "\nHigh Score:" + PlayerPrefs.GetInt("High Score"));
        else
            view.UpdateGameOverNoti("Almost done! \nYour score:" + playerScore + "\nHigh Score:" + PlayerPrefs.GetInt("High Score"));
    }

    void StartGame()
    {
        gamePhase = 2;
        view.UpdateNotification("Game Start...");
        SummonCard(botCards[0]);
        //Add the summon card event for the cards
        onCardClicked = null;
        onCardClicked += SummonCard;
        //Free the card so it can be clicked
        LockTheCardTrigger(false);
    }

    //Lock/unlock the player card trigger
    void LockTheCardTrigger(bool state)
    {
        for (int i = 0; i < playerCards.Count; i++)
        {
            playerCards[i].onLockCard = state;
        }
    }

    #region Tool
    //Shuffle all the element inside the list lst
    List<int> ShuffleList(List<int> lst)
    {
        System.Random r = new System.Random();
        IEnumerable<int> rand = lst.OrderBy(x => r.Next()).Take(lst.Count);
        return rand.ToList();
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene("Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
