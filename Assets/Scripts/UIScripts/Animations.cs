using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{

    public Animator transition;


    public void GameendAnimStart()
    {
        //��� ���� ������� ���� ��� ������� �������� ����� ����
        transition.SetTrigger("Start");
    }
}
