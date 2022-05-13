
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.AI;

namespace Juego.unity
{



  public class enemigo : MonoBehaviour
  {

    public NavMeshAgent agent;

    public float distance;
    public float vida = -0.01f;
    

   
    void Update()
    {   //Verificamos que si la distancia del jugador es menor a distance definida del enemigo para que se active.
            

        if (Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position) < distance)
        {
               agent.SetDestination(GameObject.FindWithTag("Player").transform.position);
               agent.speed = 10;

        }
        else
        {
            agent.speed = 0;
        }

        






    }
  }
}