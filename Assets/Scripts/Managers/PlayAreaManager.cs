using System.Collections.Generic;
using UnityEngine;

public class PlayAreaManager : MonoBehaviour
{
    public static PlayAreaManager Instance { get; private set; }

    public GameObject DiscardText;
    public GameObject HandSlot;
    public RectTransform Deck;
    public RectTransform UsedZone;
    public Transform PlayArea;
    public Transform PlayerHand;
    public Transform EnemyHand;
    public Transform ActionZone;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SwitchHands(bool isPlayerTurn)
    {
        PlayerHand.gameObject.SetActive(isPlayerTurn);
        EnemyHand.gameObject.SetActive(!isPlayerTurn);
    }

    public void SetDiscardTextVisible(bool isVisible)
    {
        DiscardText.SetActive(isVisible);
    }

    public void ClearUI()
    {        
        ClearHand(PlayerHand);
        ClearHand(EnemyHand);
        ClearUsedZone();
    }

    public bool IsCardInPlayerHand(Transform hand)
    {
        return hand == PlayerHand;
    }

    public List<CardInstanceController> GetEnemyCards()
    {
        List<CardInstanceController> cards = new List<CardInstanceController>();

        foreach (Transform handSlot in EnemyHand)
        {
            CardInstanceController card = handSlot.GetComponentInChildren<CardInstanceController>();
            if (card != null)
            {
                cards.Add(card);
            }
        }
        return cards;
    }

    void ClearHand(Transform hand)
    {        
        for (int i = 0; i < hand.childCount; i++)
        {
            Transform child = hand.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    void ClearUsedZone()
    {
        bool usedZoneContainsCards = UsedZone.childCount > 1;

        if (usedZoneContainsCards)
        {
            int skipDeckIndex = 1;

            for (int i = skipDeckIndex; i < UsedZone.childCount; i++)
            {
                Transform child = UsedZone.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }
}
