using UnityEngine;

public class Bullet3D : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Debug line to double check what the bullet is actually hitting in your console log
        Debug.Log("Bullet hit: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            GameController3D controller = FindFirstObjectByType<GameController3D>();
            if (controller != null)
            {
                controller.LoseGame();
            }
            
            // Instantly delete the bullet so it stops traveling
            Destroy(gameObject);
        }
    }
}
