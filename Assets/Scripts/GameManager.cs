using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public List<Card_data> deck = new List<Card_data>();
    public List<Card_data> player_deck = new List<Card_data>();
    public List<Card_data> ai_deck = new List<Card_data>();
    public List<Card_data> player_hand = new List<Card_data>();
    public List<Card_data> ai_hand = new List<Card_data>();
    public List<Card_data> discard_pile = new List<Card_data>();
    public List<Card_data> playing_field = new List<Card_data>();
    public List<Card_data> Playing_field_ai = new List<Card_data>();


    public Card blank;
    public Canvas canvas;
    public TextMeshProUGUI turnDisplay;
    public TextMeshProUGUI playerHealthDisplay;
    public TextMeshProUGUI aiHealthDisplay;
    public TextMeshProUGUI goldDisplay;
    public TextMeshProUGUI eventNotificationDisplay;
    public TextMeshProUGUI payoutCountdownDisplay;
    public Transform playingFieldPlayerSpawnpoint; // Drag the empty GameObject here in the Inspector
    public Transform playingFieldAISpawnpoint;     // Drag the empty GameObject here in the Inspector
    public Vector3 playingFieldOffset;             // Set X: 300 in the Inspector to space cards out
    public Vector3 player_hand_spawnpoint;
    private Vector3 ai_hand_spawnpoint;
    public Vector3 offset;
    public Vector3 ai_offset;
    public Vector3 y_offset;
    private Card playerCardGameObject;
    private Card aiCardGameObject;
    private int playerHealth = 5;
    private int aiHealth = 5;
    private int playerGold = 6;
    private int turnCounter = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (gm != null && gm != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gm = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        ai_hand_spawnpoint = player_hand_spawnpoint + new Vector3(0, 550, 0);
        canvas = FindAnyObjectByType<Canvas>();
        player_hand_spawnpoint += new Vector3(0, 10, 0);
        
        if (turnDisplay == null)
        {
            turnDisplay = FindAnyObjectByType<TextMeshProUGUI>();
        }
        
        if (turnDisplay != null)
        {
            turnDisplay.text = "Turn: Player";
        }
        
        if (playerHealthDisplay != null)
        {
            playerHealthDisplay.text = $"Player Health: {playerHealth}";
        }
        
        if (aiHealthDisplay != null)
        {
            aiHealthDisplay.text = $"AI Health: {aiHealth}";
        }
        
        if (goldDisplay != null)
        {
            goldDisplay.text = $"Gold: {playerGold}";
        }
        
        UpdatePayoutCountdown();
        Deal();
    }

    void Update()
    {
        
    }

    void ShowEventNotification(string message)
    {
        if (eventNotificationDisplay != null && !isGameOver)
        {
            eventNotificationDisplay.text = message;
            eventNotificationDisplay.gameObject.SetActive(true);
            CancelInvoke("HideEventNotification");
            Invoke("HideEventNotification", 3f);
        }
    }

    void ShowEventNotification(string message, bool permanent)
    {
        if (eventNotificationDisplay != null)
        {
            eventNotificationDisplay.text = message;
            eventNotificationDisplay.gameObject.SetActive(true);
            CancelInvoke("HideEventNotification");
            if (!permanent)
            {
                Invoke("HideEventNotification", 3f);
            }
        }
    }

    void HideEventNotification()
    {
        if (eventNotificationDisplay != null)
        {
            eventNotificationDisplay.gameObject.SetActive(false);
        }
    }

    void UpdatePayoutCountdown()
    {
        if (payoutCountdownDisplay != null)
        {
            int turnsUntilPayout = 2 - (turnCounter % 2);
            if (turnsUntilPayout == 0)
            {
                turnsUntilPayout = 2;
            }
            payoutCountdownDisplay.text = $"Payout in: {turnsUntilPayout} turn{(turnsUntilPayout != 1 ? "s" : "")}";
        }
    }

    void GameOver(string winnerMessage)
    {
        isGameOver = true;
        ShowEventNotification(winnerMessage, true);
        Debug.Log(winnerMessage);
        
        // Disable all card interaction
        Card[] allCards = FindObjectsByType<Card>();
        foreach (Card card in allCards)
        {
            card.enabled = false;
        }
        
        // Update turn display
        if (turnDisplay != null)
        {
            turnDisplay.text = "GAME OVER";
        }
        
        // You can add additional logic here like showing a game over screen,
        // resetting the game, or navigating to a menu
    }

    void Deal()
    {
        Shuffle(player_deck);
        Shuffle(ai_deck);
        for (int i = 0; i < 2; i++)
        {
            Card current_card = Instantiate(blank, player_hand_spawnpoint + offset, Quaternion.identity, canvas.transform);
            offset.x += 300;
            current_card.data = player_deck[i];
            player_hand.Add(current_card.data);
            current_card.transform.SetParent(canvas.transform);
        }
        for (int i = 0; i < 2; i++)
        {
            Card current_card = Instantiate(blank, ai_hand_spawnpoint + ai_offset, Quaternion.identity, canvas.transform);
            ai_offset.x += 300;
            current_card.data = ai_deck[i];
            current_card.ai_card = true;
            ai_hand.Add(current_card.data);
            current_card.transform.SetParent(canvas.transform);
        }
    }

    void Shuffle(List<Card_data> _deck)
    {
        System.Random rng = new System.Random();
        for (int i = 0; i < _deck.Count; i++)
        {
            int j = rng.Next(_deck.Count);
            Card_data temp = _deck[i];
            _deck[i] = _deck[j];
            _deck[j] = temp;
        }
    }

    Card_data GetRandomCardFromDeck(List<Card_data> deck)
    {
        if (deck.Count == 0)
            return null;
        
        int randomIndex = Random.Range(0, deck.Count);
        return deck[randomIndex];
    }

    bool PlayerCanMakeAMove()
    {
        // Check if player can afford any card in their hand
        foreach (Card_data card in player_hand)
        {
            if (playerGold >= card.cost)
            {
                return true;
            }
        }
        
        // Check if player can afford to draw a new hand
        if (playerGold >= 2)
        {
            return true;
        }
        
        return false;
    }

    void AI_Turn()
    {
        // Find all AI cards that are currently in hand
        Card[] allCards = FindObjectsByType<Card>();
        List<Card> aiCardsInHand = new List<Card>();
        
        foreach (Card card in allCards)
        {
            if (card.ai_card && ai_hand.Contains(card.data))
            {
                aiCardsInHand.Add(card);
            }
        }
        
        // If there are AI cards to play
        if (aiCardsInHand.Count > 0)
        {
            // Choose a random card from AI hand
            int randomIndex = Random.Range(0, aiCardsInHand.Count);
            Card cardToPlay = aiCardsInHand[randomIndex];
            
            // Only play if the AI playing field is empty
            if (Playing_field_ai.Count == 0)
            {
                Playing_field_ai.Add(cardToPlay.data);
                ai_hand.Remove(cardToPlay.data);
                aiCardGameObject = cardToPlay;
                
                cardToPlay.transform.SetParent(canvas.transform);
                cardToPlay.transform.position = playingFieldAISpawnpoint.position + new Vector3(0, -50, 0);
                
                if (turnDisplay != null)
                {
                    turnDisplay.text = "Turn: Player";
                }
                Debug.Log($"{cardToPlay.card_name} played by AI to the playing field!");
                
                // Check if both cards have been played
                if (playing_field.Count > 0 && Playing_field_ai.Count > 0)
                {
                    Invoke("ResolveRound", 2f);
                }
            }
            else
            {
                Debug.Log("AI playing field is full! Card cannot be played.");
            }
        }
        else
        {
            Debug.Log("AI has no cards to play!");
            
            // Deal 2 new cards to AI if they run out
            if (ai_hand.Count == 0 && ai_deck.Count > 0)
            {
                DealAICards();
            }
        }
    }
    
    void ResolveRound()
    {
        if (playing_field.Count > 0 && Playing_field_ai.Count > 0 && playerCardGameObject != null && aiCardGameObject != null)
        {
            Card_data playerCard = playing_field[0];
            Card_data aiCard = Playing_field_ai[0];
            
            int playerDamage = playerCard.damage;
            int aiDamage = aiCard.damage;
            
            string result = "";
            if (playerDamage > aiDamage)
            {
                result = "PLAYER WINS";
                aiHealth--;
            }
            else if (aiDamage > playerDamage)
            {
                result = "AI WINS";
                playerHealth--;
            }
            else
            {
                result = "TIE";
            }
            
            Debug.Log(result);
            
            // Update health displays
            if (playerHealthDisplay != null)
            {
                playerHealthDisplay.text = $"Player Health: {playerHealth}";
            }
            if (aiHealthDisplay != null)
            {
                aiHealthDisplay.text = $"AI Health: {aiHealth}";
            }
            
            // Check if game is over
            if (playerHealth <= 0)
            {
                GameOver("AI WINS THE GAME!");
                return;
            }
            else if (aiHealth <= 0)
            {
                GameOver("PLAYER WINS THE GAME!");
                return;
            }
            
            // Remove cards from the playing field lists
            playing_field.Clear();
            Playing_field_ai.Clear();
            
            // Destroy only the specific card GameObjects that were played
            if (playerCardGameObject != null)
            {
                Destroy(playerCardGameObject.gameObject);
            }
            if (aiCardGameObject != null)
            {
                Destroy(aiCardGameObject.gameObject);
            }
            
            playerCardGameObject = null;
            aiCardGameObject = null;
            
            if (turnDisplay != null)
            {
                turnDisplay.text = "Turn: Player";
            }
            
            // Increment turn counter
            turnCounter++;
            
            // Update the payout countdown display
            UpdatePayoutCountdown();
            
            // Give player 3 gold every 2 turns
            if (turnCounter % 2 == 0)
            {
                playerGold += 3;
                if (goldDisplay != null)
                {
                    goldDisplay.text = $"Gold: {playerGold}";
                }
                ShowEventNotification("Gold Payout! +3 Gold");
                Debug.Log($"Gold payout! Player gained 3 gold. (Turn {turnCounter})");
                UpdatePayoutCountdown();
            }
            
            // Check if both hands are empty
            if (player_hand.Count == 0 && ai_hand.Count == 0)
            {
                RefillHands();
                
                // Check if player is out of moves
                if (!PlayerCanMakeAMove())
                {
                    GameOver("AI WINS! Player out of gold!");
                    return;
                }
            }
        }
    }
    
    void RefillHands()
    {
        // Reset offsets for new card placement
        offset = Vector3.zero;
        ai_offset = Vector3.zero;
        
        // Deal 2 new cards to each player
        for (int i = 0; i < 2; i++)
        {
            if (player_deck.Count > 0)
            {
                Card current_card = Instantiate(blank, player_hand_spawnpoint + offset, Quaternion.identity, canvas.transform);
                offset.x += 300;
                current_card.data = GetRandomCardFromDeck(player_deck);
                player_hand.Add(current_card.data);
                current_card.transform.SetParent(canvas.transform);
            }
        }
        
        for (int i = 0; i < 2; i++)
        {
            if (ai_deck.Count > 0)
            {
                Card current_card = Instantiate(blank, ai_hand_spawnpoint + ai_offset, Quaternion.identity, canvas.transform);
                ai_offset.x += 300;
                current_card.data = GetRandomCardFromDeck(ai_deck);
                current_card.ai_card = true;
                ai_hand.Add(current_card.data);
                current_card.transform.SetParent(canvas.transform);
            }
        }
        
        // Give player +5 gold cashout for running out of cards
        playerGold += 5;
        if (goldDisplay != null)
        {
            goldDisplay.text = $"Gold: {playerGold}";
        }
        
        ShowEventNotification("Hand Empty! Refilled Hand +5 Gold");
        Debug.Log("Hands refilled! Player gained 5 gold cashout.");
    }
    
    void DealAICards()
    {
        // Reset AI offset for new card placement
        ai_offset = Vector3.zero;
        
        // Deal 2 new cards to AI
        for (int i = 0; i < 2; i++)
        {
            if (ai_deck.Count > 0)
            {
                Card current_card = Instantiate(blank, ai_hand_spawnpoint + ai_offset, Quaternion.identity, canvas.transform);
                ai_offset.x += 300;
                current_card.data = GetRandomCardFromDeck(ai_deck);
                current_card.ai_card = true;
                ai_hand.Add(current_card.data);
                current_card.transform.SetParent(canvas.transform);
            }
        }
        
        Debug.Log("AI drew 2 new cards.");
    }
    
    public void NewHandButton()
    {
        // Check if player has enough gold
        if (playerGold < 2)
        {
            ShowEventNotification("Not enough gold! Need 2 gold");
            Debug.Log("Not enough gold to draw a new hand! Need 2 gold.");
            return;
        }
        
        // Destroy all current cards in player's hand
        Card[] allCards = FindObjectsByType<Card>();
        foreach (Card card in allCards)
        {
            if (!card.ai_card && player_hand.Contains(card.data))
            {
                Destroy(card.gameObject);
            }
        }
        
        // Clear the player hand list
        player_hand.Clear();
        
        // Reset offsets for new card placement
        offset = Vector3.zero;
        
        // Deal 2 new cards to player
        for (int i = 0; i < 2; i++)
        {
            if (player_deck.Count > 0)
            {
                Card current_card = Instantiate(blank, player_hand_spawnpoint + offset, Quaternion.identity, canvas.transform);
                offset.x += 300;
                current_card.data = GetRandomCardFromDeck(player_deck);
                player_hand.Add(current_card.data);
                current_card.transform.SetParent(canvas.transform);
            }
        }
        
        // Deduct 2 gold
        playerGold -= 2;
        if (goldDisplay != null)
        {
            goldDisplay.text = $"Gold: {playerGold}";
        }
        
        ShowEventNotification("New Hand Drawn! -2 Gold");
        Debug.Log("Player drew 2 new cards for 2 gold.");
        
        // Check if player is out of moves
        if (!PlayerCanMakeAMove())
        {
            GameOver("AI WINS! Player out of gold!");
        }
    }

    public void PlayCard(Card card)
    {
        if (!playing_field.Contains(card.data))
        {
            // Check if player has enough gold
            if (playerGold < card.cost)
            {
                ShowEventNotification($"Not enough gold! Need {card.cost}, have {playerGold}");
                Debug.Log($"Not enough gold to play {card.card_name}! Need {card.cost} gold, but only have {playerGold}.");
                return;
            }
            
            // Only add to list if the playing field is empty
            if (playing_field.Count == 0)
            {
                playing_field.Add(card.data);
                player_hand.Remove(card.data);
                playerCardGameObject = card;
                
                // Deduct gold
                playerGold -= card.cost;
                if (goldDisplay != null)
                {
                    goldDisplay.text = $"Gold: {playerGold}";
                }

                card.transform.SetParent(canvas.transform);
                card.transform.position = playingFieldPlayerSpawnpoint.position + new Vector3(0, -50, 0);

                if (turnDisplay != null)
                {
                    turnDisplay.text = "Turn: AI";
                }
                ShowEventNotification($"{card.card_name} Played! -{card.cost} Gold");
                Debug.Log($"{card.card_name} added to the playing field!");
                
                // Call AI turn after 2 seconds
                Invoke("AI_Turn", 2f);
                
                // Check if both cards have been played after AI plays
                if (Playing_field_ai.Count > 0)
                {
                    Invoke("ResolveRound", 5f);
                }
            }
            else
            {
                // Don't add to list, just keep it where it is
                Debug.Log($"Playing field is full! {card.card_name} cannot be added to the list.");
            }
        }
    }
}