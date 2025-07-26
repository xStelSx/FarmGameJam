using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{

    public Animator transition;


    public void GameendAnimStart()
    {
        //вот твоя строчка кода для запуска анимации конца игры
        transition.SetTrigger("Start");
    }
}
