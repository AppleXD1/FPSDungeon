using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class EnemyNotfity : MonoBehaviour
{
    public Transform[] enemies; 
    public AudioSource heartbeatSource;
    public float maxDistance = 20f; 
    public float minPitch = 0.5f; 
    public float maxPitch = 2.0f; 

    void Update()
    {
        float closestDistance = Mathf.Infinity;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }


        if (closestDistance < maxDistance)
        {
            float normalizedDist = closestDistance / maxDistance;
            float pitch = Mathf.Lerp(maxPitch, minPitch, normalizedDist);
            heartbeatSource.pitch = pitch;

            if (!heartbeatSource.isPlaying) heartbeatSource.Play();
        }
        else
        {
            heartbeatSource.Stop();
        }

    }
}
