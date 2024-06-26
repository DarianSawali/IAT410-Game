using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class HealthManager : MonoBehaviour
{
    public static HealthManager instance; // Singleton instance

    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;
    public GameObject Heart4;

    public int health;
    private int maxHealth = 4;
    private int defaultHealth = 3;

    public AudioManager audioManager;
    public AudioClip damage;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the current scene is scene index 6
        if (SceneManager.GetActiveScene().buildIndex == 6 || SceneManager.GetActiveScene().buildIndex == 10)
        {
            // Reset health to default health
            ResetHealth();
        }

        else
        {
            health = PlayerPrefs.GetInt("PlayerCurrentLives");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //testing purposes
        // if (Input.GetKeyDown(KeyCode.U))
        // {
        //     decreaseHealth();
        // }
        // if (Input.GetKeyDown(KeyCode.I))
        // {
        //     addHealth();
        // }

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     ResetHealth();
        // }

        switch (health)
        {
            case 0:
                {
                    Heart1.gameObject.SetActive(false);
                    Heart2.gameObject.SetActive(false);
                    Heart3.gameObject.SetActive(false);
                    Heart4.gameObject.SetActive(false);
                    break;
                }
            case 1:
                {
                    Heart1.gameObject.SetActive(true);
                    Heart2.gameObject.SetActive(false);
                    Heart3.gameObject.SetActive(false);
                    Heart4.gameObject.SetActive(false);
                    break;
                }
            case 2:
                {
                    Heart1.gameObject.SetActive(true);
                    Heart2.gameObject.SetActive(true);
                    Heart3.gameObject.SetActive(false);
                    Heart4.gameObject.SetActive(false);
                    break;
                }
            case 3:
                {
                    Heart1.gameObject.SetActive(true);
                    Heart2.gameObject.SetActive(true);
                    Heart3.gameObject.SetActive(true);
                    Heart4.gameObject.SetActive(false);
                    break;
                }
            case 4:
                {
                    Heart1.gameObject.SetActive(true);
                    Heart2.gameObject.SetActive(true);
                    Heart3.gameObject.SetActive(true);
                    Heart4.gameObject.SetActive(true);
                    break;
                }
        }

        // if (SceneManager.GetActiveScene().buildIndex == 2)
        // {
        //     if (health < defaultHealth)
        //     {
        //         ResetHealth(); // if health is below 3, reset
        //     }
        // }

        // if (SceneManager.GetActiveScene().buildIndex == 6)
        // {
        //     if (health < defaultHealth)
        //     {
        //         ResetHealth(); // if health is below 3, reset
        //         // if health is above 3, which is 4, keep it at 4 as a form
        //         // of their hardwork that pays offs
        //     }
        // }
        // Debug.Log(health);
    }

    public void decreaseHealth()
    {
        health--;
        audioManager.PlaySoundEffect(damage);
        PlayerPrefs.SetInt("PlayerCurrentLives", health);
        
        // if (health <= 0)
        // {
        //     SceneManager.LoadScene(0);
        // }
    }

    public void addHealth()
    {
        if (health < maxHealth)
        {
            health++;
            PlayerPrefs.SetInt("PlayerCurrentLives", health);
        }
    }

    public void ResetHealth()
    {
        health = defaultHealth;
        PlayerPrefs.SetInt("PlayerCurrentLives", health);
        Debug.Log("reset health" + health);
    }

}


