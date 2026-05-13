using System.Collections.Generic;
using UnityEngine;

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

    public Card blank;
    public Canvas canvas;
    public Transform playingFieldPlayerSpawnpoint; // Drag the empty GameObject here in the Inspector
    public Vector3 playingFieldOffset;             // Set X: 300 in the Inspector to space cards out
    public Vector3 player_hand_spawnpoint;
    private Vector3 ai_hand_spawnpoint;
    public Vector3 offset;
    public Vector3 ai_offset;
    public Vector3 y_offset;

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
        Deal();
    }

    void Update()
    {
        
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

    void AI_Turn()
    {

    }

    public void PlayCard(Card card)
    {
        if (!playing_field.Contains(card.data))
        {
            playing_field.Add(card.data);
            player_hand.Remove(card.data);

            // Space each card out based on how many are already on the field
            int cardIndex = playing_field.Count - 1;
            card.transform.SetParent(canvas.transform);
            card.transform.position = playingFieldPlayerSpawnpoint.position 
                                      + (playingFieldOffset * cardIndex);

            Debug.Log($"{card.card_name} added to the playing field!");
        }
    }
}