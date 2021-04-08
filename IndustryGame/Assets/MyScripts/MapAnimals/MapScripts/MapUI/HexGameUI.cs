using UnityEngine;
using UnityEngine.EventSystems;
using HighlightingSystem;

public class HexGameUI : MonoBehaviour
{

	public HexGrid grid;
	HexCell currentCell;

	public HexCell CurrentCell
	{
		get
		{
			return currentCell;
		}
		set
		{
			if (currentCell == value)
			{
				return;
			}
			currentCell= value;
		}

	}


	bool activeMigrate;
	int activeRegion;

	HexUnit selectedUnit;
	public HexUnit SelectedUnit
	{
		get
		{
			return selectedUnit;
		}
		set
		{
			if (selectedUnit == value)
			{
				return;
			}
			selectedUnit = value;
		}

	}


	public void SetEditMode(bool toggle)
	{
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
	}

	public void SetMigrateMode(bool toggle)
	{
		activeMigrate = toggle;
	}
	public void SetHighlightRegion(float regionIndex)
	{
		bool highlightRegionChanged = (activeRegion != (int)regionIndex);
		activeRegion = (int)regionIndex;
		if (highlightRegionChanged)
			HandleHighlightChange();
		
	}


	bool UpdateCurrentCell()
	{
		HexCell cell =
					grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell)
		{
			currentCell = cell;
			return true;
		}
		return false;
	}


	void DoSelection()
	{
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell)
		{
			selectedUnit = currentCell.Unit;
		}
	}

	void HandleInput()
	{
		if (currentCell)
		{
			if (activeMigrate)
			{
				currentCell.EnableMigrate(Color.blue, HexDirection.E);
			}
			else
			{
				currentCell.DisableMigrate();
			}
		}
	}
	void HandleHighlightChange()
	{
			for (int z = 0; z < grid.cellCountZ; z++)
				for (int x = 0; x < grid.cellCountX; x++)
				{
					HexCoordinates hexCoordinate = HexCoordinates.FromOffsetCoordinates(x, z);
					HexCell cell = grid.GetCell(hexCoordinate);
					cell.GetComponentInParent<Highlighter>().enabled = (cell.RegionId == activeRegion);
				}
		
	}


	void Update()
	{
		/*if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Input.GetMouseButtonDown(0))
			{
				DoSelection();
				HandleInput();
			}
			else if (selectedUnit)
			{
				if (Input.GetMouseButtonDown(1))
				{
					DoMove();
				}
				else
				{
					DoPathfinding();
				}
			}
		}*/

	}
	public void DoPathfinding()
	{
		//if (UpdateCurrentCell())
		//{
			if (currentCell && selectedUnit.IsValidDestination(currentCell))
			{
				grid.FindPath(selectedUnit.Location, currentCell, 4);
			}
			else
			{
				grid.ClearPath();
			}
		//}
	}

	public void DoMove()
	{
		if (grid.HasPath)
		{
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}
}