using UnityEngine;

public class PowerUp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            GameManager.Instance.AddPowerUpPoints(10); // o cualquier valor que quieras
            Destroy(gameObject);
        }
    }
}