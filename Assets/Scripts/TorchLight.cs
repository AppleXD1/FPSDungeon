using UnityEngine;

public class TorchLight : MonoBehaviour
{
    public Light torch;
    public float maxRange = 10f;
    public float maxburnTimer = 60f;
    private float timer;
    private bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = maxburnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        float percentage = Mathf.Clamp01(timer/maxburnTimer);
        
        float flickerAmount = Mathf.Lerp(0.05f, 0.3f, 1 - percentage);
        float flicker = Random.Range(1 - flickerAmount, 1 +  flickerAmount);

        torch.intensity = 6 * percentage * flicker;
        torch.range = maxRange * percentage;
        
        if(timer <= 0)
        {
            torch.enabled = false;
            isDead = true;
        }
       
    }
}
