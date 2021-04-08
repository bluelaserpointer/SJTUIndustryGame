using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigrateController : MonoBehaviour
{
    // Start is called before the first frame update

    static Vector3[] migrateDestinations = { new Vector3(250, 0, 200),new Vector3(150,0,120),new Vector3(230,0,65)};
    Vector3 currentDestination = migrateDestinations[0];
    public float migrateSpeed = 0.1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int month = Timer.GetMonth();
        Vector3 newDestnation = migrateDestinations[(month-1) / 4];
        if(newDestnation != currentDestination)
        {
            currentDestination = newDestnation;
            
        }
        if(transform.position!=currentDestination)
        MigrateBirdFlock();

    }

    void MigrateBirdFlock()
    {
        transform.position = Vector3.Lerp(transform.position, currentDestination, migrateSpeed* Time.deltaTime);
    }
}
