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
    public int health;
    public int cost;
    public int damage;
    public Sprite sprite;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public Image spriteImage;
    public bool is_being_played = false;

    void Start()
    {
        card_name = data.card_name;
        description = data.description;
        health = data.health;
        cost = data.cost;
        damage = data.damage;
        sprite = data.sprite;
        nameText.text = card_name;
        descriptionText.text = description;
        healthText.text = health.ToString();
        costText.text = cost.ToString();
        damageText.text = damage.ToString();
        spriteImage.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Card[] allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card card in allCards)
        {
            card.is_being_played = false;
        }

        is_being_played = true;
        Debug.Log($"{card_name} is being played!");
    }
}