using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    public List<GameObject> coinsObiects = new List<GameObject>();

    public Transform coinContainer;

    int currentPoints;

    public Text currentPointCounter;
    public Text maxPointCounter;

    private void Start()
    {
        foreach (Transform coinObiect in coinContainer)
        {
            Debug.Log(coinObiect);
            if(coinObiect.transform.tag == "Coin")
            {
                coinObiect.gameObject.GetComponent<Coin>().pointsManager = this;
                coinsObiects.Add(coinObiect.gameObject);
            }
        }
        currentPoints = 0;
        currentPointCounter.text = currentPoints.ToString();
        maxPointCounter.text = "/" + coinsObiects.Count;
    }

    public void GetCoin(GameObject coinObiect)
    {
        var currentCoinId = coinsObiects.FindLastIndex(a => a == coinObiect);
        coinsObiects[currentCoinId].SetActive(false);
        currentPoints++;
        currentPointCounter.text = currentPoints.ToString();
    }



}
