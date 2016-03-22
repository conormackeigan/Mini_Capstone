using UnityEngine;
using System.Collections;

public static class GLOBAL
{
    public static string VERSION_NUMBER = "0.0.1";

    public static Vector3 gridToWorld(Vector2i gridPos)
    {
        Vector3 v;

        v.x = gridPos.x * (int)IntConstants.TileSize;
        v.y = gridPos.y * (int)IntConstants.TileSize;
        v.z = 0;

        return v;
    }

    public static Vector3 gridToWorld(int posX, int posY)
    {
        Vector3 v;

        v.x = posX * (int)IntConstants.TileSize;
        v.y = posY * (int)IntConstants.TileSize;
        v.z = 0;

        return v;
    }

    public static Vector2i worldToGrid(Vector3 pos)
    {
        Vector2i v;
        v.x = (int)pos.x / (int)IntConstants.TileSize;
        v.y = (int)pos.y / (int)IntConstants.TileSize;

        return v;
    }

    public static void setLock(bool locked)
    {
        GameDirector.Instance.locked = locked;
    }

    public static bool locked()
    {
        if (GameDirector.Instance.locked)
            return true;

        return false;
    }

    public static int attackSpeed = 128;

    public static float dmgTime = 0.75f; // length of a damage sequence (hp bar drain)
}

public enum IntConstants : int
{
    TileSize = 64, //pixels per tile edge (must be square)
    MoveSpeed = 50
}

//we never work with floating point with the grid so use an int wrapper instead of Vector2
[System.Serializable]
public struct Vector2i
{
    public int x;
    public int y;

    public Vector2i(int newX, int newY)
    {
        x = newX;
        y = newY;
    }
    public Vector2i(float newX, float newY)
    {
        x = (int)newX;
        y = (int)newY;
    }

    //============
    // OPERATORS:
    //============
    public static bool operator ==(Vector2i v, Vector2i v2)
    {
        if (v.x == v2.x && v.y == v2.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator !=(Vector2i v, Vector2i v2)
    {
        if (v.x != v2.x || v.y != v2.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static Vector2i operator *(Vector2i v, int i)
    {
        v.x *= i;
        v.y *= i;
        return v;
    }
    public static Vector2i operator *(Vector2i v, float f)
    {
        v.x = (int)(v.x * f);
        v.y = (int)(v.y * f);
        return v;
    }
    public static Vector2i operator /(Vector2i v, int i)
    {
        v.x /= i;
        v.y /= i;
        return v;
    }
    public static Vector2i operator /(Vector2i v, float f)
    {
        v.x = (int)(v.x / f);
        v.y = (int)(v.y / f);
        return v;
    }
    public static Vector2i operator +(Vector2i v, Vector2i v2)
    {
        v.x += v2.x;
        v.y += v2.y;
        return v;
    }
    public static Vector2i operator +(Vector2i v, Vector3 v2)
    {
        v.x += (int)v2.x;
        v.y += (int)v2.y;
        return v;
    }
    public static Vector2i operator -(Vector2i v, Vector2i v2)
    {
        v.x -= v2.x;
        v.y -= v2.y;
        return v;
    }
    public static Vector3 operator -(Vector2i v, Vector3 v2)
    {
        Vector3 vec;
        vec.x = (float)v.x - v2.x;
        vec.y = (float)v.y - v2.y;
        vec.z = v2.z;
        return vec;
    }
    public static implicit operator Vector2(Vector2i v)
    {
        return new Vector2(v.x, v.y);
    }
    public static implicit operator Vector3(Vector2i v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static implicit operator Vector2i(Vector3 v)
    {
        return new Vector2i((int)v.x, (int)v.y);
    }
    public static implicit operator Vector2i(Vector2 v)
    {
        return new Vector2i((int)v.x, (int)v.y);
    }

    //=============
    // Methods
    //=============

    // returns the number of edges needed to travel to reach v
    public int Distance(Vector2i v)
    {
        return (Mathf.Abs(v.x - x) + Mathf.Abs(v.y - y));
    }

    public int Distance(int vX, int vY)
    {
        return (Mathf.Abs(vX - x) + Mathf.Abs(vY - y));
    }

    // returns world distance from quantized position
    public float Distance(Vector3 v)
    {
        return (new Vector3(v.x - GLOBAL.gridToWorld(this).x, v.y - GLOBAL.gridToWorld(this).y, 0).magnitude);
    }
}