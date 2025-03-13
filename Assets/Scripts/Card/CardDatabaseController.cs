using System.Collections.Generic;
using UnityEngine;
public class CardDatabaseController : MonoBehaviour
{
    public static CardDatabaseController Instance { get; private set; }
    public CardCollection CardData { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            LoadCardData();

            if (CardData == null || CardData.cards == null || CardData.cards.Count == 0)
            {
                Debug.LogError("Card database is missing or empty.");
            }
        }

    }

    [System.Serializable]
    public class CardCollection
    {
        public List<CardDataController> cards;
    }

    void LoadCardData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(Paths.Data.CardData);

        if (textAsset != null)
        {
            CardData = JsonUtility.FromJson<CardCollection>(textAsset.text);
        }
        else
        {
            Debug.LogError("Cannot find cards.json file in Resources folder");
        }
    }    
}