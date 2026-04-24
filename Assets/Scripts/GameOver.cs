using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Player player1;

    [SerializeField] private Player player2;

    [SerializeField] private TextMeshProUGUI mytext;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GetComponent<Player>();
        player2 = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Lose()
    {
        gameObject.SetActive(true);
        AudioManager.instance.PlayLose();

        Time.timeScale = 0f;
    }
    public void Win()
    {
        gameObject.SetActive(true);
        AudioManager.instance.PlayWin();
        Time.timeScale = 0f;

        // save quá trình win
    }

}
