using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoomedIA : MonoBehaviour
{
    private VehicleController ParentVehicle;
    private Vector3 nextPosition;
    private Vector3 previousPosition;
    private float startTime;
    public float Speed = 10f;
    public float RangeAutoShoot = 20f;
    public float MaxDistance = 20f;

    private List<VehicleController> vehicles;
    private bool init = false;

    // Use this for initialization
    void Start()
    {
        this.ParentVehicle = this.gameObject.GetComponentInChildren<VehicleController>();
        this.nextPosition = this.ParentVehicle.transform.position;
        this.previousPosition = this.ParentVehicle.transform.position;

        //--- Stock les v�hicules en jeu
        vehicles = new List<VehicleController>();
        Object[] vehiclesObj = GameObject.FindObjectsOfType(typeof(VehicleController));
        foreach (Object vehicle in vehiclesObj)
        {
            if (vehicle != this.ParentVehicle)
                vehicles.Add((VehicleController)vehicle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //---> Une IA ne peut �tre contr�l�e par le joueur. Toutes les capacit�s ainsi que la tourelle sont d�sactiv�es
        if (!init)
        {
            this.ParentVehicle.SetCapacitiesActivation(false);
            this.gameObject.GetComponentInChildren<VehicleTurret>().enabled = false;

            init = true;
        }

        //---> Calcule une nouvelle destination si la pr�c�dente est atteinte
        if (Vector3.Distance(nextPosition, this.ParentVehicle.transform.position) < 5f)
        {
            previousPosition = this.ParentVehicle.transform.position;
            startTime = Time.time;

            bool inArena = false;

            while (!inArena)
            {
                float newDistance = Random.Range(20f, MaxDistance);
                float angle = Random.Range(0, Mathf.PI * 2f);

                nextPosition = new Vector3(Mathf.Cos(angle) * newDistance, this.ParentVehicle.transform.position.y, Mathf.Sin(angle) * newDistance);
                
                //--- Oriente le v�hicule dans la direction du d�placement
                if (nextPosition.magnitude < 130f)
                {
                    inArena = true;
                    transform.rotation = Quaternion.LookRotation(nextPosition - previousPosition);
                }
            }
        }

        //---> D�placement par interpolation
        this.ParentVehicle.transform.position = Vector3.Lerp(previousPosition, nextPosition, (Time.time - startTime) * Speed / Vector3.Distance(previousPosition, nextPosition));

        //---> Tir automatique si un v�hicule est � port�e
        foreach (VehicleController vehicle in vehicles)
        {
            if (Vector3.Distance(vehicle.transform.position, this.ParentVehicle.transform.position) < RangeAutoShoot)
            {
                this.ParentVehicle.AutoShoot(vehicle.transform.position);
            }
        }
    }
}
