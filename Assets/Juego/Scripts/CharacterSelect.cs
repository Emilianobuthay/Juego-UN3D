using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public GameObject Robot1;
    public GameObject Robot2;
    public GameObject Robot3;
    public GameObject Robot4;

    public int Character;

    public GameObject Panel;
    public GameObject texto;

    // Update is called once per frame
    void Update()
    {
        if(Character == 1)
        {
            Robot1.SetActive(true);
            Robot2.SetActive(false);
            Robot3.SetActive(false);
            Robot4.SetActive(false);
        }
        else if (Character == 2)
        {
            Robot1.SetActive(false);
            Robot2.SetActive(true);
            Robot3.SetActive(false);
            Robot4.SetActive(false);
        }
        else if (Character == 3)
        {
            Robot1.SetActive(false);
            Robot2.SetActive(false);
            Robot3.SetActive(true);
            Robot4.SetActive(false);

        }
        else if (Character == 4)
        {
            Robot1.SetActive(false);
            Robot2.SetActive(false);
            Robot3.SetActive(false);
            Robot4.SetActive(true);
        }

    }

    public void Siguiente()
    {
        Character += 1;
        if(Character > 4)
        {
            Character = 4;
        }

    }

    public void Atras()
    {
        Character -= 1;
        if(Character < 1)
        {
            Character = 1;
        }
    }

    public void Aceptar()
    {
        PlayerPrefs.SetInt("Character", Character);
        Panel.SetActive(false);
        Robot1.SetActive(false);
        Destroy(Robot1);
        Robot2.SetActive(false);
        Destroy(Robot2);
        Robot3.SetActive(false);
        Destroy(Robot3);
        Robot4.SetActive(false);
        Destroy(Robot4);
        texto.SetActive(false);
    }
}
