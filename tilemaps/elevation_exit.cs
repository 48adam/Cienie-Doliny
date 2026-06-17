using UnityEngine;

public class elevation_exit : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundryColliders;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Coœ wesz³o w trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("To jest gracz!");

            foreach (Collider2D col in mountainColliders)
            {
                col.enabled = true;
            }
            foreach (Collider2D boundry in boundryColliders)
            {
                boundry.enabled = false;
            }

            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 1;
            }
        }
    }
}

