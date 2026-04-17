using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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
    public Transform player_hand_spawnpoint;
    public Vector3 offset;



    

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
        canvas = FindAnyObjectByType<Canvas>();
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
            Card current_card = Instantiate(blank, player_hand_spawnpoint.position + offset, Quaternion.identity, canvas.transform);
            offset.x += 300;
            current_card.data = player_deck[i];
            //player_deck.Remove(current_card.data);
            player_hand.Add(current_card.data);
            current_card.transform.SetParent(canvas.transform);
        }
        for (int i = 0; i < ai_deck.Count; i++)
        {
            ai_hand.Add(ai_deck[i]);
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
