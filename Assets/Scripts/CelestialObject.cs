using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CelestialObject : MonoBehaviour
{
    public Rigidbody Rigidbody;
    [SerializeField] bool _isStatic;
    [SerializeField] CelestialObject _sun;
    [SerializeField] float boostMod;
    List<CelestialObject> otherObjects;
    PlayerScript _player;



    //Running on a scale of 1:10, decrease everything by 10!
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
        _player = FindObjectOfType<PlayerScript>();
        _player.AddToList(this);

        if (_isStatic)
            return;
        InitialAddForce(new Vector3(0, 0, 1), _sun.gameObject, OFFSET * boostMod, ForceMode.VelocityChange, _sun.Rigidbody.mass);

    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Success!");
        Destroy(this);
        if (other.gameObject.tag == "CelestialObject")
        {
            var go = other.gameObject.GetComponent<CelestialObject>();
            if (go.Rigidbody.mass < this.Rigidbody.mass)
            {

            }
        }
    }

    private void Absorb(Scale scale, float mass, GameObject Absorber, GameObject Absorbed)
    {

    }

    void FixedUpdate()
    {
        foreach (var cObject in otherObjects)
        {
            AddForce(cObject.transform.position, cObject.gameObject, OFFSET, ForceMode.Force, cObject.Rigidbody.mass);
        }

    }

    public void AddToList(CelestialObject cObject)
    {
        otherObjects.Add(cObject);
    }

    public void RemoveFromList(CelestialObject cObject)
    {
        otherObjects.Remove(cObject);
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
        //So things don't explode if dividing by 0...
        if (distance == 0)
            distance = 1;
        return (GCONST * mass1 * mass2) / (distance * distance);
    }

    private void OnDestroy()
    {
        foreach (var item in otherObjects)
        {
            RemoveFromList(this);
        }
        _player.RemoveFromList(this);
    }
}
