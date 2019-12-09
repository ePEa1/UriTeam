using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCheck : MonoBehaviour
{
    float delay = 3.0f;

    public int enemyCount = 5;


    [SerializeField] GameObject des;

    // Update is called once per frame
    void Update()
    {
        if (enemyCount <= 0 || GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>().IsDied.Value)
            delay -= Time.deltaTime;

        if (delay<=0)
        {
            Destroy(des);
            SceneManager.LoadScene("SceTitle");
        }

    }
}
