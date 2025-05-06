using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private int width, height;
    
    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }
    
    public int GridWidth => width;
    public int GridHeight => height;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
