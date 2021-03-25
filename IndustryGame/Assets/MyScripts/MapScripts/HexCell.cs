using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public RectTransform uiRect;

	public HexGridChunk chunk;


	public int RegionId
	{
		get
		{
			return regionId;
		}
		set
		{
			if (regionId == value)
			{
				return;
			}
			regionId = value;
		}
	
	}

	public bool HighLighted
	{
		get
		{
			return highLighted;
		}
		set
		{
			if (highLighted != value)
			{
				highLighted = value;
				if (terrainTypeIndex < HexMetrics.TerrainTypeTotal && highLighted) terrainTypeIndex += HexMetrics.TerrainTypeTotal;
				else if (terrainTypeIndex >= HexMetrics.TerrainTypeTotal && !highLighted) terrainTypeIndex -= HexMetrics.TerrainTypeTotal;
				RefreshSelfOnly();
			}
		}
	}
	public HexCell PathFrom { get; set; }
	public int SearchHeuristic { get; set; }
	public int SearchPriority
	{
		get
		{
			return distance + SearchHeuristic;
		}
	}
	public HexCell NextWithSamePriority { get; set; }
	public int SearchPhase { get; set; }
	public HexUnit Unit { get; set; }
	private void Start()
	{

	}
	public Color Color {
		get {

			highLightColor.r = HexMetrics.colors[terrainTypeIndex].r * 1.4f < 255 ? HexMetrics.colors[terrainTypeIndex].r * 1.4f : 255;
			highLightColor.g = HexMetrics.colors[terrainTypeIndex].g * 1.4f < 255 ? HexMetrics.colors[terrainTypeIndex].g * 1.4f : 255;
			highLightColor.b = HexMetrics.colors[terrainTypeIndex].b * 1.4f < 255 ? HexMetrics.colors[terrainTypeIndex].b * 1.4f : 255;
			highLightColor.a = 150.0f;
			return highLighted == false ? HexMetrics.colors[terrainTypeIndex] : highLightColor; 
		}
	}

	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			elevation = value;
			RefreshPosition();
			ValidateRivers();

			for (int i = 0; i < roads.Length; i++) {
				if (roads[i] && GetElevationDifference((HexDirection)i) > 1) {
					SetRoad(i, false);
				}
			}

			Refresh();
		}
	}

	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			waterLevel = value;
			ValidateRivers();
			Refresh();
		}
	}

	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}

	public bool HasIncomingRiver {
		get {
			return hasIncomingRiver;
		}
	}

	public bool HasOutgoingRiver {
		get {
			return hasOutgoingRiver;
		}
	}

	public bool HasRiver {
		get {
			return hasIncomingRiver || hasOutgoingRiver;
		}
	}

	public bool HasRiverBeginOrEnd {
		get {
			return hasIncomingRiver != hasOutgoingRiver;
		}
	}

	public HexDirection RiverBeginOrEndDirection {
		get {
			return hasIncomingRiver ? incomingRiver : outgoingRiver;
		}
	}

	public bool HasRoads {
		get {
			for (int i = 0; i < roads.Length; i++) {
				if (roads[i]) {
					return true;
				}
			}
			return false;
		}
	}

	public HexDirection IncomingRiver {
		get {
			return incomingRiver;
		}
	}

	public HexDirection OutgoingRiver {
		get {
			return outgoingRiver;
		}
	}

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}


	public float StreamBedY {
		get {
			return
				(elevation + HexMetrics.streamBedElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float RiverSurfaceY {
		get {
			return
				(elevation + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float WaterSurfaceY {
		get {
			return
				(waterLevel + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public int UrbanLevel {
		get {
			return urbanLevel;
		}
		set {
			if (urbanLevel != value) {
				urbanLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int FarmLevel {
		get {
			return farmLevel;
		}
		set {
			if (farmLevel != value) {
				farmLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int PlantLevel {
		get {
			return plantLevel;
		}
		set {
			if (plantLevel != value) {
				plantLevel = value;
				RefreshSelfOnly();
			}
		}
	}



	public int RainLevel
	{
		get
		{
			return rainLevel;
		}
		set
		{
			if (rainLevel != value)
			{
				rainLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int SpecialIndex {
		get {
			return specialIndex;
		}
		set {
			if (specialIndex != value && !HasRiver) {
				specialIndex = value;
				RemoveRoads();
				RefreshSelfOnly();
			}
		}
	}

	public bool IsSpecial {
		get {
			return specialIndex > 0;
		}
	}

	public bool Walled {
		get {
			return walled;
		}
		set {
			if (walled != value) {
				walled = value;
				Refresh();
			}
		}
	}

	public bool Expert
	{
		get
		{
			return expert;
		}
		set
		{
			if (expert != value)
			{
				expert = value;
				RefreshSelfOnly();
			}
		}
	}
	public int TerrainTypeIndex {
		get {
			return highLighted? terrainTypeIndex : terrainTypeIndex;
		}
		set {
			/*if (terrainTypeIndex != value)
			{
				terrainTypeIndex = value;
			}
			if (highLighted && terrainTypeIndex <= 10)
			{
				terrainTypeIndex += HexMetrics.TerrainTypeTotal;
			}*/
			terrainTypeIndex = highLighted ? value + HexMetrics.TerrainTypeTotal:value;
				Refresh();
			
		}
	}

	public int LandformIndex
	{
		get
		{
			return landformIndex;
		}
		set
		{
			if (landformIndex != value)
			{
				landformIndex = value;
				Refresh();
			}
		}
	}

	public GameObject BuildingPrefab
	{
		get
		{
			return buildingPrefab;
		}
		set
		{
			if (buildingPrefab != value)
			{
				buildingPrefab = value;
				RefreshSelfOnly();
			}
		}
	}
	public List<GameObject> BuildingPrefabs;
	public int Distance
	{
		get
		{
			return distance;
		}
		set
		{
			distance = value;
			
		}
	}

	int regionId = -2; // -1,0,1,2
	int terrainTypeIndex;
	bool highLighted;
	Color highLightColor;
	int landformIndex;

	int elevation = int.MinValue;
	int waterLevel;

	int urbanLevel, farmLevel, plantLevel,rainLevel;

	int specialIndex;
	GameObject buildingPrefab;

	bool walled,expert;

	bool hasIncomingRiver, hasOutgoingRiver;
	HexDirection incomingRiver, outgoingRiver;
	//HexDirection migrateDirection;


	int distance;
	[SerializeField]
	HexCell[] neighbors;

	[SerializeField]
	bool[] roads;
	public void DisableHighlight()
	{
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.enabled = false;
	}

	public void EnableHighlight(Color color)
	{
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.color = color;
		highlight.enabled = true;
	}
	public void EnableMigrate(Color color,HexDirection hexDirection)
	{
		//Debug.Log("EnableMigrate");
		this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
		RawImage arrow = this.gameObject.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RawImage>();
		RectTransform arrowCanvasTransform = this.gameObject.transform.GetChild(1).transform.GetComponent<RectTransform>();
		arrowCanvasTransform.rotation = Quaternion.Euler(new Vector3(90, 0, 150-60* (int)hexDirection));
		arrow.color = color;
		
	}

	public void DisableMigrate()
	{
		//Debug.Log("DisableMigrate");
		this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
	}

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}

	public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}

	public bool HasRiverThroughEdge (HexDirection direction) {
		return
			hasIncomingRiver && incomingRiver == direction ||
			hasOutgoingRiver && outgoingRiver == direction;
	}

	public void RemoveIncomingRiver () {
		if (!hasIncomingRiver) {
			return;
		}
		hasIncomingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(incomingRiver);
		neighbor.hasOutgoingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveRiver () {
		RemoveOutgoingRiver();
		RemoveIncomingRiver();
	}

	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}

		HexCell neighbor = GetNeighbor(direction);
		if (!IsValidRiverDestination(neighbor)) {
			return;
		}

		RemoveOutgoingRiver();
		if (hasIncomingRiver && incomingRiver == direction) {
			RemoveIncomingRiver();
		}
		hasOutgoingRiver = true;
		outgoingRiver = direction;
		specialIndex = 0;

		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
		neighbor.specialIndex = 0;

		SetRoad((int)direction, false);
	}

	public bool HasRoadThroughEdge (HexDirection direction) {
		return roads[(int)direction];
	}

	public void AddRoad (HexDirection direction) {
		if (
			!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
			!IsSpecial && !GetNeighbor(direction).IsSpecial &&
			GetElevationDifference(direction) <= 1
		) {
			SetRoad((int)direction, true);
		}
	}

	public void RemoveRoads () {
		for (int i = 0; i < neighbors.Length; i++) {
			if (roads[i]) {
				SetRoad(i, false);
			}
		}
	}

	public int GetElevationDifference (HexDirection direction) {
		int difference = elevation - GetNeighbor(direction).elevation;
		return difference >= 0 ? difference : -difference;
	}

	bool IsValidRiverDestination (HexCell neighbor) {
		return neighbor && (
			elevation >= neighbor.elevation || waterLevel == neighbor.elevation
		);
	}

	void ValidateRivers () {
		if (
			hasOutgoingRiver &&
			!IsValidRiverDestination(GetNeighbor(outgoingRiver))
		) {
			RemoveOutgoingRiver();
		}
		if (
			hasIncomingRiver &&
			!GetNeighbor(incomingRiver).IsValidRiverDestination(this)
		) {
			RemoveIncomingRiver();
		}
	}

	void SetRoad (int index, bool state) {
		roads[index] = state;
		neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
		neighbors[index].RefreshSelfOnly();
		RefreshSelfOnly();
	}

	void RefreshPosition () {
		Vector3 position = transform.localPosition;
		position.y = elevation * HexMetrics.elevationStep;
		position.y +=
			(HexMetrics.SampleNoise(position).y * 2f - 1f) *
			HexMetrics.elevationPerturbStrength;
		transform.localPosition = position;

		Vector3 uiPosition = uiRect.localPosition;
		uiPosition.z = -position.y;
		uiRect.localPosition = uiPosition;
	}

	public void Refresh () {
		if (chunk) {
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++) {
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk) {
					neighbor.chunk.Refresh();
				}
			}
			if (Unit)
			{
				Unit.ValidateLocation();
			}
		}
	}

	void RefreshSelfOnly () {
		chunk.Refresh();
		if (Unit)
		{
			Unit.ValidateLocation();
		}
	}

	public void Save (BinaryWriter writer) {
		if (elevation == -1) elevation = 255;
		if (regionId == -1)  regionId = 255;
		writer.Write((byte)regionId);
		writer.Write((byte)terrainTypeIndex);
		writer.Write((byte)elevation);
		writer.Write((byte)waterLevel);
		writer.Write((byte)landformIndex);
		writer.Write((byte)urbanLevel);
		writer.Write((byte)farmLevel);
		writer.Write((byte)plantLevel);
		writer.Write((byte)specialIndex);
		writer.Write(walled);

		if (hasIncomingRiver) {
			writer.Write((byte)(incomingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		if (hasOutgoingRiver) {
			writer.Write((byte)(outgoingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		int roadFlags = 0;
		for (int i = 0; i < roads.Length; i++) {
			if (roads[i]) {
				roadFlags |= 1 << i;
			}
		}
		writer.Write((byte)roadFlags);
	}

	public void Load (BinaryReader reader) {

		regionId = reader.ReadByte();
		if (regionId == 255) regionId = -1;
		terrainTypeIndex = reader.ReadByte();
		int tmpElevation = reader.ReadByte();
		if (tmpElevation == 255) tmpElevation = -1;
		elevation = tmpElevation;
		
		//elevation = reader.ReadByte();
		RefreshPosition();
		waterLevel = reader.ReadByte();
		landformIndex = reader.ReadByte();
		urbanLevel = reader.ReadByte();
		farmLevel = reader.ReadByte();
		plantLevel = reader.ReadByte();
		specialIndex = reader.ReadByte();
		walled = reader.ReadBoolean();
		byte riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasIncomingRiver = true;
			incomingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasIncomingRiver = false;
		}

		riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasOutgoingRiver = true;
			outgoingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasOutgoingRiver = false;
		}

		int roadFlags = reader.ReadByte();
		for (int i = 0; i < roads.Length; i++) {
			roads[i] = (roadFlags & (1 << i)) != 0;
		}
	}

	public void SetLabel(string text)
	{
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = text;
	}
	void Update()
	{
		//UpdateAnimals();
	}
	public void UpdateAnimals()
	{
		/*Area area = GetComponentInChildren<Area>();
		ICollection <Animal> animals = area.GetSpecies();
		foreach(Animal a in animals)
		{
			int spieciesAmount = area.GetSpeciesAmount(a);
			for(int j = transform.GetChild(2).childCount; j < spieciesAmount / 10; j++)
			{
				GameObject myAnimal = GameObject.Instantiate(area.animalPrefabs[0], this.Position, new Quaternion(0, 0, 0, 0));
				myAnimal.transform.SetParent(this.transform.GetChild(2));
			}
			
			for (int j = spieciesAmount / 10; j < transform.GetChild(2).childCount; j++)
			{
				Destroy(transform.GetChild(2).GetChild(0).gameObject);
			}
		}*/

		Area area = GetComponentInChildren<Area>();
		int habitatLevel = area.habitat.Level;
		Animal habitatAnimal = area.habitat.animal;

		for(int i = transform.GetChild(2).childCount; i <= habitatLevel * 10; i++)
		{
			GameObject myAnimal = GameObject.Instantiate(area.animalPrefabs[0], this.Position, new Quaternion(0, 0, 0, 0));
			myAnimal.transform.SetParent(this.transform.GetChild(2));
			
		}
		for(int j = habitatLevel * 10;j<= transform.GetChild(2).childCount; j++)
		{
			Destroy(transform.GetChild(2).GetChild(0).gameObject);
		}
	}
}