using UnityEngine;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public FPSBody body;
    public Button level1Button;
    public Button level2Button;
    public Slider HPSilder;
    public float maxHealth;
    public float currHealth;
    public FPSCam cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GameObject.FindWithTag("Player").GetComponent<FPSBody>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<FPSCam>();
        level1Button.enabled = false;
        level2Button.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        currHealth = body.Health;
        maxHealth = body.MaxHealth;
        
        HPSilder.value = currHealth;
        HPSilder.maxValue = maxHealth;

        if(Input.GetKeyDown(KeyCode.R) && cam.camLock)
        {
            level1Button.enabled=true;
            level2Button.enabled = true;
        }
        else if(Input.GetKeyDown(KeyCode.R) && !cam.camLock)
        {
            level1Button.enabled=false;
            level2Button.enabled=false;
            cam.camLock = true;
        }
    }
}
