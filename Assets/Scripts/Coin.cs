using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;  // Value of the coin
    public float collectRange = 2.0f;  // Range at which the player can collect the coin

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    void CollectCoin()
    {
        // Notify the game manager to increase balance
        GameManager.instance.AddCoins(coinValue);
        
        // Destroy the coin after collection
        Destroy(gameObject);
    }
}