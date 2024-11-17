using UnityEngine;

public class AsteroidData
{
    public Vector3 position;
    public Vector3 velocity;
    public AsteroidType type;

    public AsteroidData(Vector3 pos, Vector3 velo, AsteroidType ty)
    {
        position = pos;
        type = ty;
        velocity = velo;
    }
}
