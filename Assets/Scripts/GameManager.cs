
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

    public Card blank;
    public Canvas canvas;
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
    // Start is called before the first frame update
    void Start()
    {
        ai_hand_spawnpoint = player_hand_spawnpoint + new Vector3(0,300, 0); //Makes ai hand spawn point above the player hand spawn point
        canvas = FindAnyObjectByType<Canvas>();
        player_hand_spawnpoint += new Vector3(0, 10, 0); //Makes player hand spawn point slightly above the bottom of the screen
        Deal();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Deal()
    {
        Shuffle(player_deck);
        Shuffle(ai_deck);
        for (int i = 0; i < 2; i++)
        {
            //player_hand.Add(player_deck[i]);
            Card current_card = Instantiate(blank, player_hand_spawnpoint + offset, Quaternion.identity, canvas.transform);
            offset.x += 300;
            current_card.data = player_deck[i];
            //player_deck.Remove(current_card.data);
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

    void Shuffle(List<Card_data> _deck )
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



    
}
