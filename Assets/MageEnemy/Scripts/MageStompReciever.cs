using UnityEngine;

public class MageStompReciever : MonoBehaviour
{
    public MageEnemyController mageController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            //if (playerRb != null && playerRb.linearVelocityY <= 0f) 
            //{     
                mageController.HandleStomp(collision.gameObject);
            //}
        }
    }
}
