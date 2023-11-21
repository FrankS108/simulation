using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CitizenController : MonoBehaviour
{
    //variables para logica de infeccion/estados
    [SerializeField] private Virus virus;
    public State estado;
    public enum State
    {
        SANO,
        INFECTADO,
        RECUPERADO,
        MUERTO
    }
    [SerializeField] public bool startInfected = false;
    public bool cubrebocas;
    public ManagerCondition managerCondition;
    [SerializeField] private GameObject state;

    //variables para logica de comportamiento
    public Transform[] keyWaypoints;  // Waypoints clave (PRIORITARIOS) desde la definicion de ruta
    public Transform[] spawnPoints;  //waypoints para spawnear from getSpawnPoints()
    public float speed = 5f;
    public int tipoDeruta;  //1 = prioritarios, 2= no prioritarios, 3 o cualquier otro = mixto

    private Transform spawnOrigin;
    private Transform currentTarget;
    private Transform[] waypointsAhead;
    private bool recorridoTerminado = false;

    private int currentKeyWaypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        //inicializa waypointts a la vistaa para recorrido automatico
        waypointsAhead = new Transform[0];
        // get automatically spawnpoints
        getSpawnPoints();
        // set key points (recorrido) en base a la variable prioritario
        setRoute();

        virus = new Virus();

        estado = startInfected ? State.INFECTADO : State.SANO;

        managerCondition = new ManagerCondition(state, estado.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        //logica de movimiento
        MoveToClosestWaypoint();
    }

    void getSpawnPoints()
    {
       // Debug.Log("gettin all these spawners bro");
        // Encuentra todos los GameObjects en la escena con la etiqueta "SPAWN"
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("SPAWN");

        // Array de Transforms para almacenar los componentes Transform de los objetos con la etiqueta
        spawnPoints = new Transform[objectsWithTag.Length];

        // Obtiene los componentes Transform de los GameObjects
        for (int i = 0; i < objectsWithTag.Length; i++)
        {
            spawnPoints[i] = objectsWithTag[i].transform;
        }

        // una vez se obtienen los spawnpoints, se selecciona aleatoriamente uno
        setInitialPosition();
    }

    void setRoute()
    {
        //Debug.Log("defining my routes bro " + tipoDeruta);
        //obteniendo keypoints de acuerdo a la configuracion
        if(tipoDeruta == 1)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("PRIOR");

            keyWaypoints = new Transform[objectsWithTag.Length];

            // Obtiene los componentes Transform de los GameObjects
            for (int i = 0; i < objectsWithTag.Length; i++)
            {
                keyWaypoints[i] = objectsWithTag[i].transform;
            }

            // Elimina aleatoriamente algunos elementos del array
            int elementsToRemove = Random.Range(1, keyWaypoints.Length);
            for (int i = 0; i < elementsToRemove; i++)
            {
                int indexToRemove = Random.Range(0, keyWaypoints.Length);
                keyWaypoints = keyWaypoints.Where((source, index) => index != indexToRemove).ToArray();
            }

            // Reordena aleatoriamente los elementos restantes del array
            keyWaypoints = keyWaypoints.OrderBy(x => Random.value).ToArray();
        }
        else if(tipoDeruta == 2)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("NON-PRIOR");

            // Array de Transforms para almacenar los componentes Transform de los objetos con la etiqueta
            keyWaypoints = new Transform[objectsWithTag.Length];

            // Obtiene los componentes Transform de los GameObjects
            for (int i = 0; i < objectsWithTag.Length; i++)
            {
                keyWaypoints[i] = objectsWithTag[i].transform;
            }

            // Elimina aleatoriamente algunos elementos del array
            int elementsToRemove = Random.Range(1, keyWaypoints.Length);
            for (int i = 0; i < elementsToRemove; i++)
            {
                int indexToRemove = Random.Range(0, keyWaypoints.Length);
                keyWaypoints = keyWaypoints.Where((source, index) => index != indexToRemove).ToArray();
            }

            // Reordena aleatoriamente los elementos restantes del array
            keyWaypoints = keyWaypoints.OrderBy(x => Random.value).ToArray();
        }
        else
        {
            // Obtiene todos los objetos con la primera etiqueta
            GameObject[] objectsWithTag1 = GameObject.FindGameObjectsWithTag("PRIOR");

            // Obtiene todos los objetos con la segunda etiqueta
            GameObject[] objectsWithTag2 = GameObject.FindGameObjectsWithTag("NON-PRIOR");

            // Combina ambos arreglos de GameObjects en uno solo
            GameObject[] combinedObjects = objectsWithTag1.Concat(objectsWithTag2).ToArray();

            // Array de Transforms para almacenar los componentes Transform de los objetos con la etiqueta
            keyWaypoints = new Transform[combinedObjects.Length];

            // Obtiene los componentes Transform de los GameObjects
            for (int i = 0; i < combinedObjects.Length; i++)
            {
                keyWaypoints[i] = combinedObjects[i].transform;
            }

            // Elimina aleatoriamente algunos elementos del array
            int elementsToRemove = Random.Range(1, keyWaypoints.Length);
            for (int i = 0; i < elementsToRemove; i++)
            {
                int indexToRemove = Random.Range(0, keyWaypoints.Length);
                keyWaypoints = keyWaypoints.Where((source, index) => index != indexToRemove).ToArray();
            }

            // Reordena aleatoriamente los elementos restantes del array
            keyWaypoints = keyWaypoints.OrderBy(x => Random.value).ToArray();
        }

        //Debug.Log("Cantidad de keypoints seleccionados: " + keyWaypoints.Length);
        
    }

    void setInitialPosition()
    {
       // Debug.Log("settin position");
        // seleccionar aleatoriamente un de los transform de los spawners
        if (spawnPoints.Length > 0)
        {
            // Elegir un índice aleatorio dentro del rango del arreglo
            int randomIndex = Random.Range(0, spawnPoints.Length);

            // Obtener el Transform en el índice aleatorio
            Transform randomWaypoint = spawnPoints[randomIndex];

            //guardar el spawn point para redigir al terminar recorrido
            spawnOrigin = randomWaypoint;

            // Posicionar el objeto en el waypoint aleatorio
            transform.position = randomWaypoint.position;

            // Ahora puedes usar 'randomWaypoint' según tus necesidades
            //Debug.Log("Waypoint aleatorio seleccionado: " + randomWaypoint.name);
        }
        else
        {
            //Debug.LogWarning("El arreglo de spawnpoints está vacío.");
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
                //comprobar si se llego al spawn original y el recorrido termino
                if(currentTarget == spawnOrigin && recorridoTerminado == true)
                {
                    //destruir objeto
                    //Debug.Log("adios papu");
                    Destroy(gameObject, 5);
                }
                //flujo normal si lleega a un keypoint en el arreglo
                else if (currentTarget == keyWaypoints[currentKeyWaypointIndex] && recorridoTerminado == false)
                {
                    
                    currentKeyWaypointIndex = (currentKeyWaypointIndex + 1); // Avanza al siguiente keyWaypoint
                    //Debug.LogWarning("LLEGAMOS AL key BRO: " + currentTarget.name);
                    //Debug.LogWarning("avanzando al key numero " + currentKeyWaypointIndex);
                    // verificar si se han visitado todos los keypoints
                    if (currentKeyWaypointIndex == keyWaypoints.Length)
                    {
                        //Debug.LogWarning("todos los keypoints han sido visitados, caminando de regreso al spawn");
                        //si termino el recorrido, se agregar al arreglo de keypoints ir al 
                        // Redimensionas el arreglo para agregar un nuevo Transform
                        System.Array.Resize(ref keyWaypoints, keyWaypoints.Length + 1);

                        // Asignas el nuevo Transform al final del arreglo
                        keyWaypoints[keyWaypoints.Length - 1] = spawnOrigin;
                        //condicional para eliminar al llegar al spawn
                        recorridoTerminado = true;
                    }
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
        //Debug.Log("objetivo actual: " + nextKeyWaypoint.name);

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
                    //Debug.Log("current actual: " + currentTarget.name);
                }
            }
        }
    }

    void actualizaEstado(bool cubrebocas, State estado)
    {

        //logica para calcular si me enferme xd
        double probability = virus.InfectionRate;

        if (!cubrebocas)
        {
            probability += virus.NoFaceMaskRate;
        }

        double result = Random.Range(1, 100) / 100f;

        Debug.Log("La probabilidad es: " + probability + " y el resultado es: " + result);

        if(result <= probability)
        {
            this.estado = estado;
            managerCondition.UpdateCondition(estado.ToString());
        }

        /*
        if (!cubrebocas)
        {
            Debug.LogWarning("Ahhhh ese vato no traia cubrebocas y me tosio en la cara DX");
        }
        if(estado == State.INFECTADO)
        {
            Debug.LogWarning("¡Chin, me enfermé, voy a esperar un ratito en lo q agarro aire mano");
            StartCoroutine(ReactivarDespuesDeTiempo(5f));
        }
        if (estado == State.RECUPERADO)
        {
            Debug.LogWarning("a seguirle xd");
        }
        */
    }

    IEnumerator ReactivarDespuesDeTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo); // Esperar durante 'tiempo' segundos

        estado = State.RECUPERADO; // Cambiar el estado a "activo" después de esperar
        //Debug.LogWarning("ya me siento mejor manito");
        //llamda solo para pruebas, no necesario
        //actualizaEstado(true);
    }

    // Puedes agregar la lógica de detección de colisionadores aquí si es necesario
    void OnTriggerEnter2D(Collider2D other)
    {

        // Verifica si el objeto colisionado tiene un nombre
        if (other.CompareTag("STREET"))
        {
            // Agrega el waypoint al arreglo waypointsAhead si aún no está presente
            if (waypointsAhead.Length == 0)
            {
                waypointsAhead = new Transform[] { other.transform };
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
            else if (System.Array.IndexOf(waypointsAhead, other.transform) == -1)
            {
                System.Array.Resize(ref waypointsAhead, waypointsAhead.Length + 1);
                waypointsAhead[waypointsAhead.Length - 1] = other.transform;
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
        }
        else if (other.CompareTag("PRIOR"))
        {
            // Agrega el waypoint al arreglo waypointsAhead si aún no está presente
            if (waypointsAhead.Length == 0)
            {
                waypointsAhead = new Transform[] { other.transform };
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
            else if (System.Array.IndexOf(waypointsAhead, other.transform) == -1)
            {
                System.Array.Resize(ref waypointsAhead, waypointsAhead.Length + 1);
                waypointsAhead[waypointsAhead.Length - 1] = other.transform;
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
        }
        else if (other.CompareTag("NON-PRIOR"))
        {
            // Agrega el waypoint al arreglo waypointsAhead si aún no está presente
            if (waypointsAhead.Length == 0)
            {
                waypointsAhead = new Transform[] { other.transform };
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
            else if (System.Array.IndexOf(waypointsAhead, other.transform) == -1)
            {
                System.Array.Resize(ref waypointsAhead, waypointsAhead.Length + 1);
                waypointsAhead[waypointsAhead.Length - 1] = other.transform;
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
        }
        else if (other.CompareTag("SPAWN"))
        {
            // Agrega el waypoint al arreglo waypointsAhead si aún no está presente
            if (waypointsAhead.Length == 0)
            {
                waypointsAhead = new Transform[] { other.transform };
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
            else if (System.Array.IndexOf(waypointsAhead, other.transform) == -1)
            {
                System.Array.Resize(ref waypointsAhead, waypointsAhead.Length + 1);
                waypointsAhead[waypointsAhead.Length - 1] = other.transform;
                //closestObject se actualiza por medioo del collider ontriogger enter
                // Encuentra el objetivo más cercano al próximo keyWaypoint
                FindClosestTarget();
            }
        }
        else if (other.CompareTag("CITIZEN"))
        {
            CitizenController otroCiudadano = other.GetComponent<CitizenController>();

            if (otroCiudadano != null)
            {
                if (otroCiudadano.estado == State.INFECTADO)
                {
                    //detectar el uso de cubrebocas, mandar a la funcion el valor de cubrebocas
                    actualizaEstado(otroCiudadano.cubrebocas, otroCiudadano.estado);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("STREET"))
        {
            // Remueve el waypoint del arreglo waypointsAhead
            waypointsAhead = System.Array.FindAll(waypointsAhead, waypoint => waypoint != other.transform);

        }
        else if (other.CompareTag("PRIOR"))
        {
            // Remueve el waypoint del arreglo waypointsAhead
            waypointsAhead = System.Array.FindAll(waypointsAhead, waypoint => waypoint != other.transform);
        }
        else if (other.CompareTag("NON-PRIOR"))
        {
            // Remueve el waypoint del arreglo waypointsAhead
            waypointsAhead = System.Array.FindAll(waypointsAhead, waypoint => waypoint != other.transform);
        }
        else if (other.CompareTag("SPAWN"))
        {
            // Remueve el waypoint del arreglo waypointsAhead
            waypointsAhead = System.Array.FindAll(waypointsAhead, waypoint => waypoint != other.transform);
        }
        else if (other.CompareTag("CITIZEN"))
        {
            CitizenController otroCiudadano = other.GetComponent<CitizenController>();

            if (otroCiudadano != null)
            {
                //controlar si baja la probabilidad ya q se vaya del rango de 2mts
            }
        }
    }
}

    
