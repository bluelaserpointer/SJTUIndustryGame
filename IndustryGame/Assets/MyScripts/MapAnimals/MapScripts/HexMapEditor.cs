using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class HexMapEditor : MonoBehaviour {

	public HexGrid hexGrid;

	int activeElevation;
	int activeWaterLevel;

	int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex,
		activeRainLevel;

	int activeregionId;
	int buildingIndex;
	int buildingLevel;
	bool highLighted;
	int activeTerrainTypeIndex;
	int activeLandformIndex;

	int brushSize;

	bool applyElevation = true;
	bool applyWaterLevel = true;

	bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex,
		 applyRainLevel;
	public Material terrainMaterial;
	enum OptionalToggle {
		Ignore, Yes, No
	}

	OptionalToggle riverMode, roadMode, walledMode, expertMode;

	bool isDrag;
	HexDirection dragDirection;
	HexCell previousCell;


	void Awake()
	{
		terrainMaterial.DisableKeyword("GRID_ON");
		//terrainMaterial.EnableKeyword("GRID_ON");
		SetEditMode(false);
	}
	public void ShowGrid(bool visible)
	{
		if (visible)
		{
			terrainMaterial.EnableKeyword("GRID_ON");
		}
		else
		{
			terrainMaterial.DisableKeyword("GRID_ON");
		}
	}
	HexCell GetCellUnderCursor()
	{
		return
				hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}
	void CreateUnit()
	{
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit)
		{
			hexGrid.AddUnit(
					Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f)
				);
		}
	}

	void DestroyUnit()
	{
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit)
		{
			hexGrid.RemoveUnit(cell.Unit);
		}
	}


	public void SetEditMode(bool toggle)
	{
		enabled = toggle;
	}
	public void setRegionId(float index)
	{
		activeregionId = (int)index;
	}
	public void SetBuildingIndex(float index)
	{
		buildingIndex = (int)index;
	}
	public void SetBuildingLevel(float level)
	{
		buildingLevel = (int)level;
	}
	public void SetTerrainTypeIndex (int index) {
		activeTerrainTypeIndex = index;
	}
	public void SetLandformIndex(float index)
	{
		activeLandformIndex = (int)index;
	}

	public void SetApplyHighLight(bool toggle)
	{
		highLighted = toggle;
	}

	public void SetApplyElevation (bool toggle) {
		applyElevation = toggle;
	}

	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}

	public void SetApplyWaterLevel (bool toggle) {
		applyWaterLevel = toggle;
	}

	public void SetWaterLevel (float level) {
		activeWaterLevel = (int)level;
	}

	public void SetApplyUrbanLevel (bool toggle) {
		applyUrbanLevel = toggle;
	}

	public void SetUrbanLevel (float level) {
		activeUrbanLevel = (int)level;
	}

	public void SetApplyFarmLevel (bool toggle) {
		applyFarmLevel = toggle;
	}

	public void SetFarmLevel (float level) {
		activeFarmLevel = (int)level;
	}

	public void SetApplyPlantLevel (bool toggle) {
		applyPlantLevel = toggle;
	}

	public void SetPlantLevel (float level) {
		activePlantLevel = (int)level;
	}
	public void SetApplyRainLevel(bool toggle)
	{
		applyRainLevel = toggle;
	}
	public void SetRainLevel(float level)
	{
		activeRainLevel = (int)level;
	}

	public void SetApplySpecialIndex (bool toggle) {
		applySpecialIndex = toggle;
	}

	public void SetSpecialIndex (float index) {
		activeSpecialIndex = (int)index;
	}

	public void SetBrushSize (float size) {
		brushSize = (int)size;
	}

	public void SetRiverMode (int mode) {
		riverMode = (OptionalToggle)mode;
	}

	public void SetRoadMode (int mode) {
		roadMode = (OptionalToggle)mode;
	}

	public void SetWalledMode (int mode) {
		walledMode = (OptionalToggle)mode;
	}
	public void SetExpertMode(int mode)
	{
		expertMode = (OptionalToggle)mode;
	}

	void Update () {
		
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			
			if (Input.GetMouseButton(0))
			{
				HandleInput();
				return;
			}
			if (Input.GetKeyDown(KeyCode.U))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					DestroyUnit();
				}
				else
				{
					
					CreateUnit();
				}
				return;
			}
		}
		previousCell = null;
	}

	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
				EditCells(currentCell);
			
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}

	void ValidateDrag (HexCell currentCell) {
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		) {
			if (previousCell.GetNeighbor(dragDirection) == currentCell) {
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}

	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
			for (int x = centerX - brushSize; x <= centerX + r; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	void EditCell (HexCell cell) {
		if (cell) {
			cell.HighLighted = highLighted;
			if (activeLandformIndex > 0)
			{
				ApplyLandform(cell);
				//return;
			}
			//if(activeregionId >= 0)
			//{
			//cell.RegionId = activeregionId;
			//}
			
			if (activeTerrainTypeIndex >= 0) {
				cell.TerrainTypeIndex = activeTerrainTypeIndex;
			}
			cell.HighLighted = highLighted;
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (applyWaterLevel) {
				cell.WaterLevel = activeWaterLevel;
			}
			if (applySpecialIndex) {
				cell.SpecialIndex = activeSpecialIndex;
			}
			if (applyUrbanLevel) {
				cell.UrbanLevel = activeUrbanLevel;
			}
			if (applyFarmLevel) {
				cell.FarmLevel = activeFarmLevel;
			}
			if (applyPlantLevel) {
				cell.PlantLevel = activePlantLevel;
			}
			if (applyRainLevel)
			{
				cell.RainLevel = activeRainLevel;
			}
			//cell.BuildingIndex = buildingIndex;
			//cell.BuildingLevel = buildingLevel;

			if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			if (roadMode == OptionalToggle.No) {
				cell.RemoveRoads();
			}
			if (walledMode != OptionalToggle.Ignore) {
				cell.Walled = walledMode == OptionalToggle.Yes;
			}
			if (expertMode != OptionalToggle.Ignore)
			{
				cell.Expert = expertMode == OptionalToggle.Yes;

			}
			if (isDrag) {
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell) {
					if (riverMode == OptionalToggle.Yes) {
						otherCell.SetOutgoingRiver(dragDirection);
					}
					if (roadMode == OptionalToggle.Yes) {
						otherCell.AddRoad(dragDirection);
					}
				}
			}
		}
	}


	void ApplyLandform(HexCell cell)
	{
		cell.Elevation = 0;
		cell.WaterLevel = 0;
		cell.PlantLevel = 0;
		cell.UrbanLevel = 0;
		cell.FarmLevel = 0;
		//cell.TerrainTypeIndex = (activeLandformIndex-1); //改变地貌颜色

		/*switch (activeLandformIndex-1)
		{
			case 0://sea
				
				cell.Elevation = -1;
				cell.WaterLevel =0;
				
				break;
			case 1://grassland
				cell.Elevation = Random.Range(0,2);
				cell.PlantLevel = 1;
				break;
			case 2://forest
				cell.Elevation = Random.Range(0, 2);
				cell.PlantLevel = 2;
				break;
			case 3://PrimalForest
				cell.Elevation = Random.Range(0, 2);
				cell.PlantLevel = 3;
				break;
			case 4://beach
				cell.Elevation = 0;
				break;
			case 5://desert
				cell.Elevation = 0;
				break;
			case 6://mountains
				cell.Elevation = Random.Range(3, 6);
				break;
			case 7://city
				cell.Elevation = 0;
				cell.UrbanLevel = Random.Range(2,4);
				break;
			case 8://farmland
				cell.Elevation = 0;
				cell.FarmLevel = Random.Range(2,4);
				break;
			case 9://polluted
				cell.Elevation = 0;
				break;
			case 10://preserve
				cell.Elevation = 0;
				break;

		}*/

		EnvironmentType type = EnvironmentType.PickOne(EnvironmentType.indexMap[activeLandformIndex - 1]);
		cell.TerrainTypeIndex = type.landformIndex;
		cell.Elevation = Random.Range(type.elevationMin, type.elevationMax + 1);
		cell.WaterLevel = type.waterLevel;
		cell.PlantLevel = Random.Range(type.plantLevelMin, type.plantLevelMax + 1);
		cell.UrbanLevel = Random.Range(type.urbanLevelMin, type.urbanLevelMax+1);
		cell.FarmLevel = Random.Range(type.farmLevelMin, type.farmLevelMax + 1);
		

	}
}