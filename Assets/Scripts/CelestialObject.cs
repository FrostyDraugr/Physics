using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CelestialObject : MonoBehaviour
{
    public Rigidbody Rigidbody;
    [SerializeField] bool _isStatic;
    [SerializeField] CelestialObject _sun;
    [SerializeField] float boostMod;
    List<CelestialObject> otherObjects;



    //Running on a scale of 1:10, increase everything by 10!
    const float OFFSET = 0.01f;
    const double GCONST = 0.667408;
    //const double EARTH_SIZE = 5.9736;
    //const double SUN_SIZE = EARTH_SIZE * 333000;
    //const double DISTANCE_EARTH_SUN = 14.9;

    private void Awake()
    {
        otherObjects = new();
        CelestialObject[] CelestialObjects = FindObjectsOfType<CelestialObject>();
        foreach (var cObject in CelestialObjects)
        {
            if (cObject != this)
                otherObjects.Add(cObject);
        }
    }
    private void Start()
    {
        Time.timeScale = 10f;

        if (_isStatic)
            return;

        InitialAddForce(new Vector3(0, 0, 1), _sun.gameObject, OFFSET * boostMod, ForceMode.VelocityChange, _sun.Rigidbody.mass);

    }

    void FixedUpdate()
    {
        foreach (var cObject in otherObjects)
        {
            AddForce(cObject.transform.position, cObject.gameObject, OFFSET, ForceMode.Force, cObject.Rigidbody.mass);
        }

    }

    private void AddForce(Vector3 pos, GameObject otherObject, float mod, ForceMode forceMode, float mass)
    {
        Rigidbody.AddForce(GravPullDir(pos) * (float)GravPull(mass, Rigidbody.mass, Vector3.Distance(otherObject.transform.position, this.gameObject.transform.position)) * mod * Time.fixedDeltaTime, forceMode);
    }

    private void InitialAddForce(Vector3 pos, GameObject otherObject, float mod, ForceMode forceMode, float mass)
    {
        Rigidbody.AddForce(pos * (float)GravPull(mass, Rigidbody.mass, Vector3.Distance(otherObject.transform.position, this.gameObject.transform.position)) * mod * Time.fixedDeltaTime, forceMode);
    }

    private Vector3 GravPullDir(Vector3 pos)
    {
        return (pos - this.gameObject.transform.position).normalized;
    }

    private double GravPull(double mass1, double mass2, double distance)
    {
        //So things don't explode when dividing by 0...
        if (distance == 0)
            distance = 1;
        return (GCONST * mass1 * mass2) / (distance * distance);
    }
}
