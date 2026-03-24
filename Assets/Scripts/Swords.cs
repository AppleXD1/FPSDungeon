using UnityEngine;

public class Swords : MonoBehaviour
{
    public float Damage = 5;
    public float Durability = 3;
    public FPSBody FPSBody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FPSBody = GameObject.FindWithTag("Player").GetComponent<FPSBody>();
    }

    // Update is called once per frame
    void Update()
    {
        if( Durability <= 0)
        {
            Destroy(this.gameObject);
            FPSBody.isEquiped = false;
        }
    }
}
