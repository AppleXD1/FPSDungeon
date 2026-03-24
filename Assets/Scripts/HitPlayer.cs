using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    public bool hitPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hitPlayer = true;
            Debug.Log("Player entered punch hitbox");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hitPlayer = false;
        }
    }
}

