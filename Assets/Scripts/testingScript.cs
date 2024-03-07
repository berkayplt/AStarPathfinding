using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class testingScript : MonoBehaviour
{
    [SerializeField] private GameObject[] players;
    [SerializeField] private int playerIndex = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerIndex < players.Length)
            {
                players[playerIndex].SetActive(true);
                playerIndex++;
            }
          
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);

        }
    }
}
