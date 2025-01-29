using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSkyBackground : MonoBehaviour
{
    [SerializeField] private float backgroundLenght = 81.9f;
    private float  _movementSpeed = 100.0f;
    private Rigidbody2D _rigidbody = null; 
    private bool spawnedNext;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        spawnedNext = false;
    }

    void Update()
    {
        _rigidbody.MovePosition(_rigidbody.position + _movementSpeed * -(Vector2)transform.right * Time.deltaTime); 
        CheckExitBounds();
    }

    private void CheckExitBounds()
    {
        var position = _rigidbody.position;
        // background in center of screen
        if (position.x <= 0 && !spawnedNext)
        {
            spawnedNext = true;
            Instantiate(gameObject, new Vector2(position.x + backgroundLenght, 0), Quaternion.identity);
        }
        
        if (position.x < - backgroundLenght) {
            Destroy(gameObject);
        }
    }
}
