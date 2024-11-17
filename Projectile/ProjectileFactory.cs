using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileFactory : MonoBehaviour
{
    // Start is called before the first frame update
    [FormerlySerializedAs("prefab")] [SerializeField] private GameObject Prefab;
    private Stack<GameObject> inactiveObjects = new();
    private int MaxProjectiles = 20;
    
    private Vector3 defaultPosition;
    
    private void Start()
    {
        defaultPosition = Prefab.transform.position;
        if (Prefab == null) 
            return;
        for (var i = 0; i < MaxProjectiles; i++)
        {
            var newProj = Instantiate(Prefab);
            newProj.name = $"Projectile{i}";
            newProj.transform.position = defaultPosition;
            newProj.GetComponent<Projectile>()._onProjectileFinished += ReturnProjectileToPool; 
            
            newProj.SetActive(false);
            inactiveObjects.Push(newProj);
        }
        Prefab.SetActive(false);
    }

    public void RequestProjectile(Vector3 position, float zRotDeg)
    {
        var newProjectile = GrabProjectileFromPool();

        newProjectile.SetActive(true);
        newProjectile.GetComponent<Projectile>().Initialize(position,zRotDeg);
    }

    private GameObject GrabProjectileFromPool()
    {
        while (inactiveObjects.Count > 0)
        {
            var obj = inactiveObjects.Pop();

            if (obj != null)
            {
                return obj; 
            }
        }
        Debug.LogError("ALL PROJECTILES IN USE");
        return null;
    }

    private void ReturnProjectileToPool(GameObject gameObj)
    {
        if (gameObj == null) 
            return;
        gameObj.transform.position = defaultPosition;
        gameObj.SetActive(false);
        inactiveObjects.Push(gameObj);

    }

}
