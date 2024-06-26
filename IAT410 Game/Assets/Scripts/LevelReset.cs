using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelReset : MonoBehaviour
{
    private string initialSceneName;
    private Vector3 playerStartPosition;
    private Vector3 skunkStartPosition;
    private Vector3 fishStartPosition;
    private Vector3 pigeonStartPosition;

    private void Start()
    {
        Time.timeScale = 1f;
        SaveInitialState();
    }

    private void SaveInitialState()
    {
        // Save the initial scene name
        initialSceneName = SceneManager.GetActiveScene().name;

        // Optionally save the initial player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject skunk = GameObject.FindGameObjectWithTag("Skunk");
        GameObject pigeon = GameObject.FindGameObjectWithTag("Pigeon");
        GameObject fish = GameObject.FindGameObjectWithTag("Fish");
        if (player != null)
        {
            playerStartPosition = player.transform.position;
        }
        if (skunk != null)
        {
            skunkStartPosition = skunk.transform.position;
        }
        if (pigeon != null)
        {
            pigeonStartPosition = pigeon.transform.position;
        }
        if (fish != null)
        {
            fishStartPosition = fish.transform.position;
        }

        Debug.Log("saved");
        Debug.Log(playerStartPosition);
        Debug.Log(fishStartPosition);
    }

    public void ResetLevel()
    {
        // Reload the scene to revert to its initial state
        SceneManager.LoadScene(initialSceneName);

        // Optionally reset player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject skunk = GameObject.FindGameObjectWithTag("Skunk");
        GameObject pigeon = GameObject.FindGameObjectWithTag("Pigeon");
        GameObject fish = GameObject.FindGameObjectWithTag("Fish");
        if (player != null)
        {
            player.transform.position = playerStartPosition;
        }
        if (skunk != null)
        {
            skunk.transform.position = skunkStartPosition;
        }
        if (pigeon != null)
        {
            pigeon.transform.position = pigeonStartPosition;
        }
        if (fish != null)
        {
            fish.transform.position = fishStartPosition;
        }

        Debug.Log("reset");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetLevel();
        }
    }

    public void OnReset()
    {
        Debug.Log("reset pressed");
        // if (value.isPressed)
        // {
        //     ResetLevel();
        //     Debug.Log("reset pressed");
        // }
    }
}
