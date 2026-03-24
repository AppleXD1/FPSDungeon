using UnityEngine;

public class LightRecharge : MonoBehaviour
{
    public TorchLight playerTorch;
    public Light stationLight;
    public bool isActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = true;
       
        playerTorch = GameObject.FindWithTag("Torch").GetComponent<TorchLight>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive)
        {
            stationLight.enabled = false;
            stationLight.intensity = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isActive && collision.gameObject.CompareTag("Player"))
        {
            playerTorch.timer += playerTorch.timer + 20f;
            isActive = false;
        }
    }
}
