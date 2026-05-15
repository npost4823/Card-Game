using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Card_data data;

    public string card_name;
    public string description;
    public int cost;
    public int damage;
    public Sprite sprite;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public Image spriteImage;
    public bool is_being_played = false;
    public bool ai_card = false;

    void Start()
    {
        card_name = data.card_name;
        description = data.description;
        cost = data.cost;
        damage = data.damage;
        sprite = data.sprite;
        nameText.text = card_name;
        descriptionText.text = description;
        costText.text = cost.ToString();
        damageText.text = damage.ToString();
        spriteImage.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ai_card)
        {
            Debug.Log($"Cannot play {card_name}. This is an AI card!");
            return;
        }

        Card[] allCards = FindObjectsByType<Card>();
        foreach (Card card in allCards)
        {
            card.is_being_played = false;
        }

        is_being_played = true;
        Debug.Log($"{card_name} is being played!");

        // Notify the GameManager
        GameManager.gm.PlayCard(this);
}
}