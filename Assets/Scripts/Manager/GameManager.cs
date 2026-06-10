using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Nhân vật game")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;
    // Start is called before the first frame update

    public void LoadMap(string name)
    {
        string mapName = "Maps/" + name;
        GameObject map = Resources.Load<GameObject>(mapName);
        Instantiate(map);
    }

}
