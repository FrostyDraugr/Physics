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

    private void Start()
    {
        //Find Player Controller, it holds the references for all other celestial objects (needed for physics calls)
        _player = FindObjectOfType<PlayerScript>();
        _player.AddToList(this);

        //Should initial inertia be added?
        if (_isStatic)
            return;

        InitialAddForce(new Vector3(0, 0, 1), _sun.gameObject, OFFSET * boostMod, ForceMode.VelocityChange, _sun.Rigidbody.mass);

    }

    private void OnCollisionEnter(Collision other)
    {
        //Check if the other colliding object has the correct tag
        if (other.gameObject.tag == "CelestialObject")
        {
            //Compare mass, the one with the greater mass absorbs the weaker object
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

    //Add mass to oneself and become bigger based on gravity scale
    private void Absorb(CelestialObject absorbed)
    {
        var scaleAdd = absorbed.Rigidbody.mass / MASSTOSCALE;
        this.gameObject.transform.localScale += new Vector3(scaleAdd, scaleAdd, scaleAdd);
        this.Rigidbody.mass += absorbed.Rigidbody.mass;
        Destroy(absorbed.gameObject);
    }

    void FixedUpdate()
    {
        //Go through every single celestial object in the scene and add force based on mass and distance
        foreach (var cObject in _player._celestialObjects)
        {
            if (cObject != this)
                AddForce(cObject.transform.position, cObject.gameObject, OFFSET, ForceMode.Force, cObject.Rigidbody.mass);
        }

        //Catch Celestial objects that might go to far away from the player controller, it's to prevent a bunch of small little objects having to be calculated
        //The player follows the object with the largest mass
        if (Vector3.Distance(_player.transform.position, this.transform.position) > 1000)
        {
            Destroy(this.gameObject);
        }

    }

    //Add Force based on Gravity equation
    //Force = (GravityConstant * m1 * m2) / d^2, get mass through rigidbody
    //Added mod for scaling down the effects since we're not to scale
    //Uses Forcemode to have mass taken into account, all gravity objects affect each other but aren't equal
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
