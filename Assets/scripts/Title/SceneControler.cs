using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject Rocket;
    private Transform RocketTransform;
    private AudioSource RocketSounds;
    private AudioSource BottonSounds;

    void Start()
    {
        Rocket = GameObject.Find("Rocket");
        if (Rocket != null)
        {
            RocketTransform = Rocket.transform;
        }
        else
        {
            Debug.LogError("Rocket object not found!");
        }

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            RocketSounds = audioSources[0];
            BottonSounds = audioSources[1];
        }
        else
        {
            Debug.LogError("Not enough AudioSource components found!");
        }
    }

    public void LoadGameScene()
    {
        if (RocketSounds != null)
        {
            RocketSounds.Play();
        }
        else
        {
            Debug.LogError("RocketSounds is not set!");
        }

        if (Rocket != null)
        {
            Rigidbody2D rb = Rocket.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, 5f);
                StartCoroutine(CheckRocketPosition());
            }
            else
            {
                Debug.LogError("Rigidbody2D component not found on Rocket!");
            }
        }
        else
        {
            Debug.LogError("Rocket is not set!");
        }
    }

    private IEnumerator CheckRocketPosition()
    {
        while (true)
        {
            if (RocketTransform != null)
            {
                Debug.Log("Rocket Position: " + RocketTransform.position.y);
                if (RocketTransform.position.y > 4f)
                {
                    SceneManager.LoadScene("GameScene");
                    yield break;
                }
            }
            else
            {
                Debug.LogError("RocketTransform is not set!");
                yield break;
            }
            yield return null;
        }
    }

    public void LoadManualScene()
    {
        if (BottonSounds != null)
        {
            BottonSounds.PlayOneShot(BottonSounds.clip);
        }
        else
        {
            Debug.LogError("BottonSounds is not set!");
        }
        SceneManager.LoadScene("ManualScene");
    }

    public void LoadTitleScene()
    {
        if (BottonSounds != null)
        {
            BottonSounds.PlayOneShot(BottonSounds.clip);
        }
        else
        {
            Debug.LogError("BottonSounds is not set!");
        }
        SceneManager.LoadScene("TitleScene");
    }

    public void QuitGame()
    {
        if (BottonSounds != null)
        {
            BottonSounds.PlayOneShot(BottonSounds.clip);
        }
        else
        {
            Debug.LogError("BottonSounds is not set!");
        }
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}