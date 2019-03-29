using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public static List<Boids> allBoids;
    public float minDistance = 0.2f;
    public float minNeighbourDistance = 0.2f;
    private Vector3 direction;
    public Vector3 velocity;
    public float maxVelocity;

    private void OnEnable()
    {
        if (allBoids == null)
        {
            allBoids = new List<Boids>();
        }
        allBoids.Add(this);

        this.transform.position = new Vector3();
        
        //velocity = Camera.main.ScreenToWorldPoint(Vector3.zero) - this.transform.position;
        //velocity.Normalize();
    }

    private void OnDisable()
    {
        if (allBoids != null)
        {
            allBoids.Remove(this);
        }
    }

    private void Update()
    {
        //direction to centre of mass
        //match velocity
        //keep minimal space between objects
        CalculatePosition();

        Move();
    }

    private void Move()
    {
        //float speed = velocity * Time.deltaTime;
        //Vector3.MoveTowards(this.transform.position, this.transform.position + direction.normalized, speed);
        //Vector3.RotateTowards(this.transform.position, this.transform.position + direction.normalized, speed, 0);
        //this.transform.position
    }

    private void LimitVelocity()
    {
        if (this.velocity.magnitude > maxVelocity)
        {
            this.velocity = (this.velocity / this.velocity.magnitude) * maxVelocity;
        }
    }

    private void CalculatePosition()
    {
        List<Boids> neighbours = new List<Boids>();
        foreach(Boids b in allBoids)
        {
            if (b != this && Vector3.Distance(b.transform.position, this.transform.position) < minNeighbourDistance)
            {
                neighbours.Add(b);
            }
        }

        if (neighbours.Count == 0)
        {
            Debug.LogError("List is empty");
        }

        this.velocity = this.velocity + Seperation(neighbours) + CenterOfMass(neighbours) + VelocityMatch(neighbours);
        LimitVelocity();
        Vector3 targetPos = this.transform.position + this.velocity;
        Vector3.RotateTowards(this.transform.position, targetPos, maxVelocity * Time.deltaTime, 0);
        this.transform.position = targetPos;

    }

    private Vector3 Seperation(List<Boids> nb)
    {
        Vector3 result = Vector3.zero;

        foreach (Boids b in nb)
        {
            if (b != this && Vector3.Distance(b.transform.position, this.transform.position) < minDistance)
            {
                result = result - (b.transform.position - this.transform.position);
            }
        }

        return result;
    }

    private Vector3 CenterOfMass(List<Boids> nb)
    {
        Vector3 result = Vector3.zero;

        foreach(Boids b in nb)
        {
            if (b != this)
            {
                result += b.transform.position;
            }
        }
        result = result / (allBoids.Count - 1);

        return (result - this.transform.position) / 100;
    }

    private Vector3 VelocityMatch(List<Boids> nb)
    {
        Vector3 result = Vector3.zero;

        foreach (Boids b in nb)
        {
            if (b != this)
            {
                result += b.velocity;
            }
        }
        result = result / (allBoids.Count - 1);

        return (result - this.velocity) / 8;
    }
}
