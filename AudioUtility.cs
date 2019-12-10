using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioUtility : MonoBehaviour
{
    public AudioClip[] impact; // List of audio slips you want to use.
    AudioSource audioSource; // Assign a gameObject that you want the audio to play from
    private int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(impact[0], 0.7F);
    }

    void OnCollisionEnter(Collision other)
    {
        // if there is a collision with an object tagged below then we play a random audio clip
        if (other.gameObject.tag == "rewardCollector")
        {
            // Here we are getting a random number between 0 and 2
            num = Random.Range(0, 3);
            // Depending on the number an audio clip will play.
            if (num == 1)
            {
                audioSource.PlayOneShot(impact[0], 0.7F);
            }
            else if (num == 2)
            {
                audioSource.PlayOneShot(impact[1], 0.7F);
            }
            else
            {
                audioSource.PlayOneShot(impact[2], 0.7F);
            }
        }
    }
}