using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    [Header("Scripts")]
    public FPSBody body;
    public FPSCam cam;
    public TorchLight torch;
    [Header("UI")]
    public GameObject DeathUI;
    public GameObject level1Button;
    public GameObject level2Button;
    public Slider HPSilder;
    
    public float maxHealth;
    public float currHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GameObject.FindWithTag("Player").GetComponent<FPSBody>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<FPSCam>();
        torch = GameObject.FindWithTag("Torch").GetComponent<TorchLight>();
        DeathUI.SetActive(false);



    }

    // Update is called once per frame
    void Update()
    {
        currHealth = body.Health;
        maxHealth = body.MaxHealth;
        
        HPSilder.value = currHealth;
        HPSilder.maxValue = maxHealth;

        if(cam.camLock)
        {
            level1Button.SetActive(false);
            level2Button.SetActive(false);
        }
        else if(!cam.camLock)
        {
            level1Button.SetActive(true);
            level2Button.SetActive(true);

        }

        NextScene();
        Death();
    }

    void NextScene()
    {
        if(body.lvlComplete)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void Death()
    {
        if(body.Health <= 0 || torch.isDead)
        {
            DeathUI.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
