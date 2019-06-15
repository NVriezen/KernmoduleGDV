using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BombBase : MonoBehaviour
{
    public int turnsBeforeExplode = 2;
    public int explodeRange = 3;

    public virtual List<Vector3>[] HittedObjects()
    {
        Vector3 bombPosition = transform.position;
        List<Vector3> explodeX = new List<Vector3>();
        List<Vector3> explodeY = new List<Vector3>();
        List<Vector3> hittableObjects = new List<Vector3>();
        int y = 0;
        int x = 0;
        for (x = (int)-explodeRange; x < explodeRange + 1; x++)
        {
            //if (x == 0)
            //{
            //    continue;
            //}
            Vector3 checkingPosition = bombPosition + new Vector3(x, bombPosition.y, y);
            if (HasHitHittable(checkingPosition))
            {
                hittableObjects.Add(checkingPosition);
                continue;
            }
            if (!HasHitWall(checkingPosition))
            {
                explodeX.Add(checkingPosition);
                continue;
            }
            if (x < 0)
            {
                explodeX.Clear();
                continue;
            }
            break;
        }

        x = 0;
        for (y = (int)-explodeRange; y < explodeRange+1; y++)
        {
            if (y == 0)
            {
                continue;
            }
            Vector3 checkingPosition = bombPosition + new Vector3(x, bombPosition.y, y);
            if (HasHitHittable(checkingPosition))
            {
                hittableObjects.Add(checkingPosition);
                continue;
            }
            if (!HasHitWall(checkingPosition))
            {
                explodeY.Add(checkingPosition);
                continue;
            }
            if (y < 0)
            {
                explodeY.Clear();
                continue;
            }
            break;
        }

        List<Vector3> hittedObjects = explodeX;
        foreach (Vector3 hitted in explodeY)
        {
            hittedObjects.Add(hitted);
        }
        
        return new List<Vector3>[2] { hittedObjects, hittableObjects};
    }

    private bool HasHitWall(Vector3 positionToCheck)
    {
        return Physics.CheckBox(positionToCheck, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, 1 << 9);
    }

    private bool HasHitHittable(Vector3 positionToCheck)
    {
        return Physics.CheckBox(positionToCheck, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, 1 << 12);
    }

    public virtual bool UpdateGetStatus()
    {
        turnsBeforeExplode -= 1;
        return turnsBeforeExplode == 0 ? true : false;
    }

    public virtual void OnSpawn() { }
}
