using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
