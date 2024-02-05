using Unity.VisualScripting;
using UnityEngine;

public class CelestialObject : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] bool _isStatic;
    [SerializeField] GameObject otherObject;

    //Running on a scale of 1:10, increase everything by 10!
    const float OFFSET = 0.01f;
    const double GCONST = 0.667408;
    const double EARTH_SIZE = 5.9736;
    const double SUN_SIZE = EARTH_SIZE * 333000;
    //const double DISTANCE_EARTH_SUN = 14.9;

    private void Start()
    {
        if (_isStatic)
            return;
        //Debug.Log(GravPull(SUN_SIZE, EARTH_SIZE, DISTANCE_EARTH_SUN));
        _rigidbody.AddForce(new Vector3(0, 0, 1) * (float)GravPull(SUN_SIZE, EARTH_SIZE, Vector3.Distance(otherObject.transform.position, this.gameObject.transform.position)) * OFFSET * 1.4f * Time.deltaTime, ForceMode.VelocityChange);
    }

    void Update()
    {
        if (_isStatic)
            return;

        AddForce(OFFSET, ForceMode.Acceleration);
        Debug.Log(Vector3.Magnitude(gameObject.transform.position - otherObject.transform.position));
    }

    private void AddForce(float mod, ForceMode forceMode)
    {
        _rigidbody.AddForce(GravPullDir(otherObject) * (float)GravPull(SUN_SIZE, EARTH_SIZE, Vector3.Distance(otherObject.transform.position, this.gameObject.transform.position)) * mod * Time.deltaTime, forceMode);
    }

    private Vector3 GravPullDir(GameObject gameObject)
    {
        return (otherObject.transform.position - this.gameObject.transform.position).normalized;
    }

    private double GravPull(double mass1, double mass2, double distance)
    {
        return (GCONST * mass1 * mass2) / (distance * distance);
    }
}
