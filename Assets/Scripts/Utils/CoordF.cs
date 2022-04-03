using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldStrategy
{
    [System.Serializable]
    public class CoordF
    {
        public float x;
        public float y; // Up down
        public float z;

        public CoordF(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public CoordF(Vector3 pos)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public static CoordF Zero()
        {
            return new CoordF(0, 0, 0);
        }

        public static CoordF Invalid()
        {
            return new CoordF(-1,-1,-1);
        }

        public static CoordF operator +(CoordF a, CoordF b)
        {
            return new CoordF(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static CoordF operator -(CoordF a, CoordF b)
        {
            return new CoordF(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static CoordF operator /(CoordF a, CoordF b)
        {
            return new CoordF(b.x != 0 ? a.x / b.x : 0, b.y != 0 ? a.y / b.y : 0, b.z != 0 ? a.z / b.z : 0);
        }

        public static CoordF operator /(CoordF a, float div)
        {
            return new CoordF(a.x / div, a.y / div, a.z / div);
        }

        public static CoordF operator *(CoordF a, float mult)
        {
            return new CoordF(a.x * mult, a.y * mult, a.z * mult);
        }

        public static CoordF operator *(CoordF a, CoordF b)
        {
            return new CoordF(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static implicit operator Vector3(CoordF v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static bool operator ==(CoordF a, CoordF b)
        {
            return a.x == b.x && a.z == b.z && a.y == b.y;
        }

        public static bool operator !=(CoordF a, CoordF b)
        {
            return a.x != b.x || a.z != b.z || a.y != b.y;
        }

        public override string ToString()
        {
            return "(" + x + " ; " + y + " ; " + z + ")";
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        //From seblauge WTF
        public override bool Equals(object other)
        {
            return (CoordF)other == this;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}