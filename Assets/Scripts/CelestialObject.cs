using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class CelestialObject : MonoBehaviour
{
    public Rigidbody Rigidbody;
    [SerializeField] bool _isStatic;
    [SerializeField] CelestialObject _sun;
    [SerializeField] float boostMod;
    //List<CelestialObject> otherObjects;
    PlayerScript _player;



    //Running on a scale of 1:10, decrease everything by 10!
    const float OFFSET = 0.01f;
    const double GCONST = 0.667408;

    const float MASSTOSCALE = 1989209 * 2;
    //const double SUN_SIZE = EARTH_SIZE * 333000;
    //const double DISTANCE_EARTH_SUN = 14.9;

    private void Awake()
    {
        // //otherObjects = new();
        // CelestialObject[] CelestialObjects = FindObjectsOfType<CelestialObject>();
        // foreach (var cObject in CelestialObjects)
        // {
        //     if (cObject != this)
        //         otherObjects.Add(cObject);
        // }


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
        if (other.gameObject.tag == "CelestialObject")
        {
            var go = other.gameObject.GetComponent<CelestialObject>();
            if (go.Rigidbody.mass > this.Rigidbody.mass)
            {
                return;
            }
            else
            {
                Absorb(go);
            }
        }
    }

    private void Absorb(CelestialObject absorbed)
    {
        var scaleAdd = absorbed.Rigidbody.mass / MASSTOSCALE;
        this.gameObject.transform.localScale += new Vector3(scaleAdd, scaleAdd, scaleAdd);
        this.Rigidbody.mass += absorbed.Rigidbody.mass;
        Destroy(absorbed.gameObject);
    }

    void FixedUpdate()
    {
        foreach (var cObject in _player._celestialObjects)
        {
            if (cObject != this)
                AddForce(cObject.transform.position, cObject.gameObject, OFFSET, ForceMode.Force, cObject.Rigidbody.mass);
        }

        if (Vector3.Distance(_player.transform.position, this.transform.position) > 1000)
        {
            Destroy(this.gameObject);
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
        if (distance == 0)
            distance = 1;
        return (GCONST * mass1 * mass2) / (distance * distance);
    }

    private void OnDestroy()
    {
        _player.RemoveFromList(this);
    }
}
