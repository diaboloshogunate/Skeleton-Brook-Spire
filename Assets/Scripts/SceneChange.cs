using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private Animator animator;
    private string nextLevel;

    private void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    public void FadeToLevel(string level)
    {
        this.animator.SetTrigger("FadeOut");
        this.nextLevel = level;
    }

    public void OnComplete()
    {
        SceneManager.LoadScene(this.nextLevel);
    }
}
