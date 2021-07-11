
using UnityEngine;

public class TowerBtn : MonoBehaviour {

    [SerializeField]
    private Tower _towerObject;
    [SerializeField]
    private Sprite _dragSprite;
    [SerializeField]
    private int _towerPrice;

    public Tower TowerObject
    {
        get
        {
            return _towerObject; 
        }
    }

    public Sprite DragSprite
    {
        get
        {
            return _dragSprite;
        }
    }

    public int TowerPrice
    {
        get
        {
            return _towerPrice;
        }
    }

}
