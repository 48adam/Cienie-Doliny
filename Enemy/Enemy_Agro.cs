using UnityEngine;

public class Enemy_Agro : MonoBehaviour
{
    [SerializeField] private Enemy_Movement enemyMovement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyMovement.StartChasing(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyMovement.StopChasing();
        }
    }
}