using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GreenWater_KillPart : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        try
        {
            Debug.Log(tilemap.gameObject); //Debug

            var tile = tilemap.GetTile(new Vector3Int(1, 1, 0));
            Debug.Log(tile);
            
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
