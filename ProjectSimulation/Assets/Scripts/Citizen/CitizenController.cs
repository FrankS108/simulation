using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CitizenController : MonoBehaviour
{
    
    public Transform[] keyWaypoints;  // Waypoints clave (PRIORITARIOS)
    public Transform[] spawnPoints;  //waypoints para spawnear
    public float speed = 5f;

    private Transform currentTarget;
    private Transform[] waypointsAhead;

    private int currentKeyWaypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
       
        //set initial position with spawn
        setInitialPosition();
        //inicializa waypointts a la vistaa
        waypointsAhead = new Transform[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (keyWaypoints.Length == 0)
        {
            Debug.LogWarning("Asegúrate de asignar waypoints clave y calles en el inspector.");
            return;
        }

        MoveToClosestWaypoint();
    }

    void setInitialPosition()
    {
        // seleccionar aleatoriamente un de los transform de los spawners
        if (spawnPoints.Length > 0)
        {
            // Elegir un índice aleatorio dentro del rango del arreglo
            int randomIndex = Random.Range(0, spawnPoints.Length);

            // Obtener el Transform en el índice aleatorio
            Transform randomWaypoint = spawnPoints[randomIndex];

            // Posicionar el objeto en el waypoint aleatorio
            transform.position = randomWaypoint.position;

            // Ahora puedes usar 'randomWaypoint' según tus necesidades
            Debug.Log("Waypoint aleatorio seleccionado: " + randomWaypoint.name);
        }
        else
        {
            Debug.LogWarning("El arreglo de spawnpoints está vacío.");
        }
    }

    void MoveToClosestWaypoint()
    {
        
        // Mueve el objeto hacia el objetivo más cercano
        if (currentTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, Time.deltaTime);

            // Verifica si ha llegado al objetivo más cercano y actualiza el índice del keyWaypoint
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
            {
                Debug.Log("Objetivo alcanzado: " + currentTarget.name);
                if (currentTarget.CompareTag("PRIOR"))
                {
                    currentKeyWaypointIndex = (currentKeyWaypointIndex + 1) % keyWaypoints.Length; // Avanza al siguiente keyWaypoint
                    Debug.Log("LLEGAMOS AL PRIOR BRO: " + currentTarget.name);
                }
                FindClosestTarget();
            }
        }
    }

    void FindClosestTarget()
    {
        // Inicializa el objetivo más cercano
        currentTarget = null;
        float closestDistance = Mathf.Infinity;

        // Encuentra el próximo keyWaypoint
        Transform nextKeyWaypoint = keyWaypoints[currentKeyWaypointIndex];
        Debug.Log("objetivo actual: " + nextKeyWaypoint.name);

        // Encuentra el objetivo más cercano al próximo keyWaypoint
        foreach (Transform waypoint in waypointsAhead)
        {
            // Excluye el propio collider del objeto actual
            if (waypoint != GetComponent<Collider2D>())
            {
                
                float distanceToNextKeyWaypoint = Vector2.Distance(waypoint.position, nextKeyWaypoint.position);

                if (currentTarget == null || distanceToNextKeyWaypoint < Vector2.Distance(currentTarget.position, nextKeyWaypoint.position))
                {
                    currentTarget = waypoint;
                    closestDistance = distanceToNextKeyWaypoint;
                    Debug.Log("current actual: " + currentTarget.name);
                }
            }
        }
    }


    // Puedes agregar la lógica de detección de colisionadores aquí si es necesario
    void OnTriggerEnter2D(Collider2D other)
    {

        // Verifica si el objeto colisionado tiene un nombre
        if (other.CompareTag("STREET"))
        {
            // Imprime el nombre del objeto en la consola de debug
            Debug.Log("Colisión con calle: " + other.gameObject.name);

            // Agrega el waypoint al arreglo waypointsAhead si aún no está presente
            if (waypointsAhead.Length == 0)
            {
                waypointsAhead = new Transform[] { other.transform };
                Debug.Log("guardando calle en arreglo vacio: " + other.gameObject.name);
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
            else if (System.Array.IndexOf(waypointsAhead, other.transform) == -1)
            {
                System.Array.Resize(ref waypointsAhead, waypointsAhead.Length + 1);
                waypointsAhead[waypointsAhead.Length - 1] = other.transform;
                Debug.Log("guardando calle: " + other.gameObject.name);
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
        }
        else if (other.CompareTag("PRIOR"))
        {
            // Imprime el nombre del objeto en la consola de debug
            Debug.Log("Colisión con key: " + other.gameObject.name);

            // Agrega el waypoint al arreglo waypointsAhead si aún no está presente
            if (System.Array.IndexOf(waypointsAhead, other.transform) == -1)
            {
                System.Array.Resize(ref waypointsAhead, waypointsAhead.Length + 1);
                waypointsAhead[waypointsAhead.Length - 1] = other.transform;
                Debug.Log("guardando key: " + other.gameObject.name);
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
        }
        else if (other.CompareTag("SPAWN"))
        {
            // Imprime el nombre del objeto en la consola de debug
            Debug.Log("Colisión con spawn: " + other.gameObject.name);

            // Agrega el waypoint al arreglo waypointsAhead si aún no está presente
            if (System.Array.IndexOf(waypointsAhead, other.transform) == -1)
            {
                System.Array.Resize(ref waypointsAhead, waypointsAhead.Length + 1);
                waypointsAhead[waypointsAhead.Length - 1] = other.transform;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("STREET"))
        {
            // Imprime el nombre del objeto en la consola de debug
            Debug.Log("sacando calle: " + other.gameObject.name);

            // Remueve el waypoint del arreglo waypointsAhead
            waypointsAhead = System.Array.FindAll(waypointsAhead, waypoint => waypoint != other.transform);
            //closestObject se actualiza por medioo del collider ontriogger enter
            // Encuentra el objetivo más cercano al próximo keyWaypoint
            FindClosestTarget();

        }
        else if (other.CompareTag("PRIOR"))
        {
            // Imprime el nombre del objeto en la consola de debug
            Debug.Log("Colisión con key: " + other.gameObject.name);

            // Remueve el waypoint del arreglo waypointsAhead
            waypointsAhead = System.Array.FindAll(waypointsAhead, waypoint => waypoint != other.transform);
        }
        else if (other.CompareTag("SPAWN"))
        {
            // Imprime el nombre del objeto en la consola de debug
            Debug.Log("Colisión con spawn: " + other.gameObject.name);

            // Remueve el waypoint del arreglo waypointsAhead
            waypointsAhead = System.Array.FindAll(waypointsAhead, waypoint => waypoint != other.transform);
        }
    }
}

    
