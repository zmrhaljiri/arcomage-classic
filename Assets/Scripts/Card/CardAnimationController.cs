using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimationController : MonoBehaviour
{
    public static CardAnimationController Instance { get; private set; }

    AudioManager _audioManager;

    List<GameObject> _activeAnimatedObjects = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }        
    }

    void Start()
    {
        _audioManager = AudioManager.Instance;
    }

    public void ShiftCard(RectTransform card, Vector2 offset)
    {
        card.anchoredPosition += offset;        
    }

    public IEnumerator AnimateCardToHand(GameObject cardBack, Vector3 position, float animationDuration)
    {
        _audioManager.PlayGameplaySound(Constants.Sounds.MoveCard);

        yield return StartCoroutine(MoveOverSeconds(cardBack, position, animationDuration));
    }

    public IEnumerator AnimateCardToActionZone(GameObject cardObject, Vector3 position, float animationDuration)
    {
        yield return StartCoroutine(MoveOverSeconds(cardObject, position, animationDuration));
    }

    public IEnumerator AnimateCardToUsedZone(GameObject cardObject, Vector3 position, float animationDuration)
    {
        yield return StartCoroutine(MoveOverSeconds(cardObject, position, animationDuration));
    }    

    public IEnumerator AnimateShuffle(List<GameObject> usedCards, List<Vector3> cardPositions, RectTransform deck, Transform playArea)
    {
        for (int i = 0; i < usedCards.Count; i++)
        {
            GameObject usedCard = usedCards[i];
            RectTransform usedCardRectTransform = usedCard.GetComponent<RectTransform>();

            usedCard.transform.SetParent(playArea, false);
            usedCardRectTransform.position = cardPositions[i];
            yield return null;

            StartCoroutine(MoveOverSeconds(usedCard, deck.position, Constants.Durations.ShuffleCard, true));
        }
    }    

    public void RegisterAnimatedObject(GameObject obj)
    {
        _activeAnimatedObjects.Add(obj);
    }

    public void ClearAnimatedObjects()
    {
        _activeAnimatedObjects.Clear();
    }
    public void StopAllCardCoroutines()
    {
        StopAllCoroutines();

        // Clean up all active animated objects
        foreach (GameObject obj in _activeAnimatedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        ClearAnimatedObjects();
    }

    IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 endPosition, float seconds, bool destroyObject = false)
    {
        RegisterAnimatedObject(objectToMove);
        float elapsedTime = 0;
        Vector3 startPosition = objectToMove.transform.position;

        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return null; // Use "yield return null" instead of "WaitForEndOfFrame();" for smoother frame updates
        }

        // Ensure the object reaches the exact end position
        objectToMove.transform.position = endPosition;

        if (destroyObject)
        {
            Destroy(objectToMove);
        }
    }
}
