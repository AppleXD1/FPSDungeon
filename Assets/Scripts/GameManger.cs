using UnityEngine;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public FPSBody body;
    public Slider HPSilder;
    public float maxHealth;
    public float currHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GameObject.FindWithTag("Player").GetComponent<FPSBody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        currHealth = body.Health;
        maxHealth = body.MaxHealth;
        
        HPSilder.value = currHealth;
        HPSilder.maxValue = maxHealth;
    }
}
