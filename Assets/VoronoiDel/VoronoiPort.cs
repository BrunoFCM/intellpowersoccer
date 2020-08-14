using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Delaunay;
using Delaunay.Geo;
using UnityEditor;
using System;

public class VoronoiPort : MonoBehaviour
{
	private List<Vector2> m_points;
	private List<LineSegment> m_edges = null;
	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;
	private Delaunay.Voronoi voronoi;

	public GameEnvironmentInfo gameEnvironmentInfo;
	private float time;

	void Start()
	{
		time = 0;
	}

	void FixedUpdate ()
	{
		//VoronoiTriangulation();
	}

	void Update() {
		time -= Time.deltaTime;
		//if(time < 1){
			VoronoiTriangulation();
		//	time = 3;
		//}
	}

	private void VoronoiTriangulation()
	{
				
		List<uint> colors = new List<uint> ();
		m_points = new List<Vector2> ();

		foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
			colors.Add (0);
			m_points.Add (new Vector2 (agent.transform.position.x+14, agent.transform.position.z+7.5f));
		}

		foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
			colors.Add (0);
			m_points.Add (new Vector2 (agent.transform.position.x+14, agent.transform.position.z+7.5f));
		}

		/*colors.Add (0);
		m_points.Add(new Vector2 (14f+14, 7.5f+14f));
		colors.Add (0);
		m_points.Add(new Vector2 (-14f+14, 7.5f+14f));
		colors.Add (0);
		m_points.Add(new Vector2 (14f+14, -7.5f+14f));
		colors.Add (0);
		m_points.Add(new Vector2 (-14f+14, -7.5f+14f));*/
	

		voronoi = new Delaunay.Voronoi (m_points, colors, new Rect(0, 0, 28, 15));
		m_edges = voronoi.VoronoiDiagram ();
			
		m_spanningTree = voronoi.SpanningTree (KruskalType.MINIMUM);
		m_delaunayTriangulation = voronoi.DelaunayTriangulation ();
	}


	public Vector2 getPointToGo(int agentNr){
		
		List<Vector2> playerRegionPoints = new List<Vector2>();

		foreach(Vector2 points in voronoi.Region(m_points[agentNr-1])){
			playerRegionPoints.Add(points);
		}

		Vector2 goal;

		if(agentNr < 5){
			goal = new Vector2(14, 0);
		}
		else{
			goal = new Vector2(-14, 0);
		}


		playerRegionPoints.Sort((v1,v2) =>Vector2.Distance(new Vector2(v1.x-14, v1.y-7.5f), goal)
			.CompareTo(Vector2.Distance(new Vector2(v2.x-14, v2.y-7.5f), goal)));


		return playerRegionPoints[0];
	}

	public Vector2 getPointToGoAttacking(int agentNr, AgentCore playerWithBall){
		
		List<Vector2> playerRegionPoints = new List<Vector2>();

		foreach(Vector2 points in voronoi.Region(m_points[agentNr-1])){
			playerRegionPoints.Add(points);
		}

		/*Vector2 goal = new Vector2(playerWithBall.transform.position.x, playerWithBall.transform.position.z);


		playerRegionPoints.Sort((v1,v2) =>Vector2.Distance(new Vector2(v1.x-14, v1.y-7.5f), goal)
			.CompareTo(Vector2.Distance(new Vector2(v2.x-14, v2.y-7.5f), goal)));


		return playerRegionPoints[0];*/

		return GetCentroid(playerRegionPoints);
	}

	public Vector2 getPointToGoDefending(int agentNr, AgentCore playerDefending){
		
		List<Vector2> playerRegionPoints = new List<Vector2>();

		foreach(Vector2 points in voronoi.Region(m_points[agentNr-1])){
			playerRegionPoints.Add(points);
		}

		/*Vector2 goal = new Vector2(playerDefending.transform.position.x, playerDefending.transform.position.z);


		playerRegionPoints.Sort((v1,v2) =>Vector2.Distance(new Vector2(v1.x-14, v1.y-7.5f), goal)
			.CompareTo(Vector2.Distance(new Vector2(v2.x-14, v2.y-7.5f), goal)));


		return playerRegionPoints[0];*/

		return GetCentroid(playerRegionPoints);
	}

	public static Vector2 GetCentroid(List<Vector2> poly)
	{
		float accumulatedArea = 0.0f;
		float centerX = 0.0f;
		float centerY = 0.0f;

		for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
		{
		float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
		accumulatedArea += temp;
		centerX += (poly[i].x + poly[j].x) * temp;
		centerY += (poly[i].y + poly[j].y) * temp;
		}

		if (Math.Abs(accumulatedArea) < 1E-7f)
		return new Vector2();  // Avoid division by zero

		accumulatedArea *= 3f;
		return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
	}

	void OnDrawGizmos ()
	{
		if (m_edges != null) {
			Gizmos.color = Color.yellow;
			for (int i = 0; i< m_edges.Count; i++) {
				Vector2 left = (Vector2)m_edges [i].p0;
				Vector2 right = (Vector2)m_edges [i].p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}


		if(voronoi != null){
			Gizmos.color = Color.green;
			foreach(List<Vector2> region in voronoi.Regions()){
				Gizmos.DrawSphere(GetCentroid(region), 0.2f);
			}
		}
		

		/*if (m_points != null && voronoi != null) {
			Gizmos.color = Color.cyan;
			List<Vector2> region1 = voronoi.Region(m_points[(int)Char.GetNumericValue(gameEnvironmentInfo.getNearestPlayerToBall().tag[5])-1]);

			for (int i = 0; i< region1.Count-1; i++) {
				Vector2 left = (Vector2)region1[i];
				Vector2 right = (Vector2)region1[i+1];
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
			
			Vector2 l = (Vector2)region1[region1.Count-1];
			Vector2 r = (Vector2)region1[0];
			Gizmos.DrawLine ((Vector3)l, (Vector3)r);
		}*/

		Gizmos.color = Color.white;
		if (m_delaunayTriangulation != null) {
			for (int i = 0; i< m_delaunayTriangulation.Count; i++) {
				Vector2 left = (Vector2)m_delaunayTriangulation [i].p0;
				Vector2 right = (Vector2)m_delaunayTriangulation [i].p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}

		/*if (m_spanningTree != null) {
			Gizmos.color = Color.green;
			for (int i = 0; i< m_spanningTree.Count; i++) {
				LineSegment seg = m_spanningTree [i];				
				Vector2 left = (Vector2)seg.p0;
				Vector2 right = (Vector2)seg.p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}*/

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (new Vector2 (14+14, 7.5f+7.5f), new Vector2 (14+14, -7.5f+7.5f));
		Gizmos.DrawLine (new Vector2 (14+14, -7.5f+7.5f), new Vector2 (-14+14, -7.5f+7.5f));
		Gizmos.DrawLine (new Vector2 (-14+14, -7.5f+7.5f), new Vector2 (-14+14, 7.5f+7.5f));
		Gizmos.DrawLine (new Vector2 (-14+14, 7.5f+7.5f), new Vector2 (14+14, 7.5f+7.5f));


		Gizmos.color = Color.red;
		if (m_points != null) {
			for (int i = 0; i < m_points.Count; i++) {
				if(i<4)
					Gizmos.color = Color.red;
				else
					Gizmos.color = Color.blue;
				Gizmos.DrawSphere (m_points [i], 0.2f);
			}
		}

		/*if (m_points != null && voronoi != null) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(getPointToGo(1), 0.3f);
		}*/
	}
}