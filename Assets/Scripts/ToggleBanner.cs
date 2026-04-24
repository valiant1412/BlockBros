using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToggleBanner : MonoBehaviour
{   

    // Start is called before the first frame update
    void Awake()
    {
        // Mặc định ẩn đi khi mới vào game
        gameObject.SetActive(false); 
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
    }
    public void ShowWarning()
    {
        StopAllCoroutines();

          
            gameObject.SetActive(true);
            StartCoroutine(Toggle());
        
        
    }
    public IEnumerator Toggle()
    {   
        
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        
    }
}
