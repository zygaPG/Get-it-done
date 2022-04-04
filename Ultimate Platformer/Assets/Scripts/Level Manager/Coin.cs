using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coin : MonoBehaviour
{
    public PointsManager pointsManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            pointsManager.GetCoin(this.gameObject);
        }
    }

}
