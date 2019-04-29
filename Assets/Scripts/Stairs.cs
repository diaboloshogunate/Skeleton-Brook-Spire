using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    private SceneChange sceneChange;

    void Start()
    {
        this.sceneChange = FindObjectOfType<SceneChange>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            this.sceneChange.FadeToLevel("Win");
        }
    }
}
