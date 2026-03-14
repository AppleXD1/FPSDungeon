using UnityEngine;

public class FPSBody : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public bool gotKey;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        MaxHealth = 10;
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Key"))
        {
            gotKey = true;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Door") && gotKey)
        {
            Debug.Log("level complete");
        }
    }

    
}
