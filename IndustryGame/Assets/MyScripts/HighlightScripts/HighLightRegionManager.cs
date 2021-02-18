using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;
public class HighLightRegionManager : MonoBehaviour
{
    public HexGrid hexGrid;
    public HighlightManager highlightManager;
    public GameObject pivot;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int chunkCountX = hexGrid.cellCountX / HexMetrics.chunkSizeX;
        int chunkCountZ = hexGrid.cellCountZ / HexMetrics.chunkSizeZ;
        HexCell relatedCell;
        for (int z = 0; z < hexGrid.cellCountZ; z+=HexMetrics.chunkSizeZ)
        {
            for (int x = 0; x < hexGrid.cellCountX; x+=HexMetrics.chunkSizeX)
            {
                HexCoordinates hexCoordinate = HexCoordinates.FromOffsetCoordinates(x, z);
                relatedCell = hexGrid.GetCell(hexCoordinate);
                
                HexGridChunk hexGridChunk = relatedCell.GetComponentInParent<HexGridChunk>();
                if (hexGridChunk.highLighted)
                {
                    //if (relatedCell.RegionId == -1) return;
                    hexGridChunk.transform.SetParent(pivot.transform);
                    
                }
                else
                {
                    hexGridChunk.transform.SetParent(hexGrid.transform);
                }
            }
            //highlightManager.SwitchesCollider(transform);

        }

        //transform.position = pivot.transform.position;
        transform.position = new Vector3(pivot.transform.position.x, pivot.transform.position.y + 10, pivot.transform.position.z);
        
        for (int z = 0; z < hexGrid.cellCountZ; z+=HexMetrics.chunkSizeZ)
        {
            for (int x = 0; x < hexGrid.cellCountX; x+=HexMetrics.chunkSizeX)
            {
                HexCoordinates hexCoordinate = HexCoordinates.FromOffsetCoordinates(x, z);
                relatedCell = hexGrid.GetCell(hexCoordinate);
                //Debug.Log(x.ToString() + " " + z.ToString() + " " + relatedCell.GetComponentInParent<HexGridChunk>().highLighted);
                HexGridChunk hexGridChunk = relatedCell.GetComponentInParent<HexGridChunk>();
                if (hexGridChunk.highLighted)
                {
                    hexGridChunk.transform.SetParent(transform);

                }
                else
                {
                    hexGridChunk.transform.SetParent(hexGrid.transform);
                }
            }

        }

    }

}
