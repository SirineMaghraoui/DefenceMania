using UnityEngine;
public enum ProjectileType
{
    rock,arrow,fireball
};
public class Projectile : MonoBehaviour {
    [SerializeField]
    private int _attackStrength;
  
    [SerializeField]
    private ProjectileType _projectileType;

    public int AttackStrength
    {
        get
        {
            return _attackStrength;
        }
        set
        {
            _attackStrength = value;
        }
    }
 
    public ProjectileType getProjectileType
    {
        get 
        {
           return _projectileType;
        }
    }
   

}
