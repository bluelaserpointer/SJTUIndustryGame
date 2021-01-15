using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	public int width = 6;
	public int height = 6;

	public Color defaultColor = Color.white;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;

	public Texture2D noiseSource;
	public Color[,] level1_color = new Color[,]{ {Color.yellow, Color.blue, Color.yellow, Color.green, Color.white, Color.yellow },
									             {Color.blue, Color.blue, Color.yellow, Color.green, Color.green, Color.white },
												 {Color.yellow, Color.yellow, Color.yellow, Color.yellow, Color.green, Color.white},
												 {Color.blue, Color.yellow, Color.yellow, Color.green, Color.green, Color.green },
												 {Color.yellow, Color.yellow, Color.yellow, Color.yellow, Color.green, Color.white },
												 {Color.blue, Color.blue, Color.green, Color.white, Color.green, Color.green },
											   };
	public int[,] level1_elevation = new int[,] {   { 1, 0, 1, 3, 5, 2 },
												   { 0, 0, 2, 3, 4, 5 },
												   { 1, 1, 1, 2, 4, 5 },
												   { 0, 1, 2, 3, 3, 2 },
												   { 2, 1, 2, 2, 4, 5 },
												   { 0, 0, 3, 5, 3, 4 },
												 };
	
	HexCell[] cells;

	Canvas gridCanvas;
	HexMesh hexMesh;

	void Awake () {
		HexMetrics.noiseSource = noiseSource;

		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

	void Start () {
		hexMesh.Triangulate(cells);
	}

	void OnEnable () {
		HexMetrics.noiseSource = noiseSource;
	}

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		return cells[index];
	}

	public void Refresh () {
		hexMesh.Triangulate(cells);
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		//position.y = level1_height[x, z]*10;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		//cell.color = defaultColor;
		cell.color = level1_color[x,z];

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		//label.text = cell.coordinates.ToStringOnSeparateLines();
		cell.uiRect = label.rectTransform;

		//cell.Elevation = 0;
		cell.Elevation = level1_elevation[x, z];
	}
}