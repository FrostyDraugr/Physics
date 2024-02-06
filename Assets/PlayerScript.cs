using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    public List<CelestialObject> _celestialObjects;
    [SerializeField] CelestialObject _sun;
    CelestialObject _largestObject;
    [SerializeField] CelestialObject _spawnable;

    [SerializeField] float _powerMax;
    float _currentPower;
    [SerializeField] float _spawnDistanceFromPlayerMax;
    [SerializeField] float _spawnDistanceFromPlayerMin;
    // Start is called before the first frame update
    void Awake()
    {
        _celestialObjects = new();
        Time.timeScale = 3f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_sun == null)
        {
            _largestObject = null;

            foreach (CelestialObject item in _celestialObjects)
            {
                if (_largestObject == null || item.Rigidbody.mass > _largestObject.Rigidbody.mass)
                    _largestObject = item;
            }
        }
        else
        {
            _largestObject = _sun;
        }

        this.gameObject.transform.position = _largestObject.transform.position;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _currentPower += Time.deltaTime;
            _currentPower = Mathf.Clamp(_currentPower, 0, _powerMax);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnCelestialObject();
            _currentPower = 0;
        }
    }

    private void SpawnCelestialObject()
    {
        Instantiate(_spawnable, transform.position + RandomVector(), Quaternion.identity);
    }

    private Vector3 RandomVector()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * SpawnDistance();
    }

    private float SpawnDistance()
    {
        return Mathf.Lerp(_spawnDistanceFromPlayerMin, _spawnDistanceFromPlayerMax, Mathf.InverseLerp(0, _powerMax, _currentPower));
    }

    public void AddToList(CelestialObject cObject)
    {
        _celestialObjects.Add(cObject);
    }

    public void RemoveFromList(CelestialObject cObject)
    {
        _celestialObjects.Remove(cObject);
    }
}
