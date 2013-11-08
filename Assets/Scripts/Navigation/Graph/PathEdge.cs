#region Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

// Microsoft Reciprocal License (Ms-RL)
//
// This license governs use of the accompanying software. If you use the software, you accept this
// license. If you do not accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
// meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// copyright license to reproduce its contribution, prepare derivative works of its contribution,
// and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works of the contribution in
// the software.
//
// 3. Conditions and Limitations
// (A) Reciprocal Grants- For any file you distribute that contains code from the software (in
// source code or binary format), you must provide recipients the source code to that file along
// with a copy of this license, which license will govern that file. You may license other files
// that are entirely your own work and do not contain code from the software under any terms you
// choose.
// (B) No Trademark License- This license does not grant you rights to use any contributors' name,
// logo, or trademarks.
// (C) If you bring a patent claim against any contributor over patents that you claim are
// infringed by the software, your patent license from such contributor to the software ends
// automatically.
// (D) If you distribute any portion of the software, you must retain all copyright, patent,
// trademark, and attribution notices that are present in the software.
// (E) If you distribute any portion of the software in source code form, you may do so only under
// this license by including a complete copy of this license with your distribution. If you
// distribute any portion of the software in compiled or object code form, you may only do so under
// a license that complies with this license.
// (F) The software is licensed "as-is." You bear the risk of using it. The contributors give no
// express warranties, guarantees or conditions. You may have additional consumer rights under your
// local laws which this license cannot change. To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for a particular purpose
// and non-infringement.

#endregion Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

namespace Thot.GameAI
{
	using UnityEngine;
	
	/// <summary>
    /// Class to represent a path edge. This path can be used by a path planner in the creation of
    /// paths. 
    /// </summary>
	public sealed class PathEdge 
	{	
		private GameObject waypointBeacon;
		private GameObject edgeBeacon;
//		private GameObject edgeMarker;
		private GameObject waypointMarker;
		
		public PathEdge(
			Vector2 source, 
		    Vector2 destination, 
		    Node fromNode, 
		    Node toNode,
		    Edge edge,
		    PathManager pathManager,
		    GameObject movingObject)
		{
			Source = source;
            Destination = destination;
			FromNode = fromNode;
			ToNode = toNode;
			Edge = edge;
			PathManager = pathManager;
			MovingObject = movingObject;
			MovingEntity = movingObject.GetComponent<MovingEntity>();
		}

        /// <summary>
        /// Gets or sets the edge source position.
        /// </summary>
        public Vector2 Source { get; set; }

        /// <summary>
        /// Gets or sets the edge destination position.
        /// </summary>
        public Vector2 Destination { get; set; }
		
		public Node FromNode { get; set; }
		public Node ToNode { get; set; }
		public Edge Edge { get; set; }
		public PathManager PathManager { get; set; }
		public GameObject MovingObject { get; set; }
		public MovingEntity MovingEntity { get; set; }
		public float Radius
		{
			get
			{
				if (MovingEntity != null && MovingEntity.enabled)
				{
					return MovingEntity.Radius;
				}
				else
				{
					return 1;
				}
			}
		}
				
		public override string ToString()
		{
			return Source + "-->" + Destination;
		}
		
		public void ShowEdge(bool show)
		{
//			if (FromNode != null)
//			{
//				if (show)
//				{
//
//				}
//				else
//				{
//					
//				}
//			}
			
			if (ToNode != null)
			{		
				if (show)
				{
					if (PathManager.showPath)
					{
						waypointBeacon = CreatePointBeacon(ToNode.Position);
						waypointMarker = CreatePointMarker(ToNode.Position);
					}
				}
				else
				{
					Object.Destroy(waypointBeacon);
					waypointBeacon = null;
					Object.Destroy(waypointMarker);
					waypointMarker = null;
				}
			}
			
			if (Edge != null)
			{
				if (show)
				{
					if (PathManager.showPath)
					{
//						edgeMarker = CreateEdgeMarker(FromNode.Position, ToNode.Position);
						edgeBeacon = CreateEdgeBeacon(FromNode.Position, ToNode.Position);
					}
				}
				else
				{
//					Object.Destroy(edgeMarker);
//					edgeMarker = null;
					Object.Destroy(edgeBeacon);
					edgeBeacon = null;
				}
			}
			
			if (Edge == null)
			{
				if (FromNode == null && ToNode != null)
				{
					if (show)
					{
						if (PathManager.showPath)
						{
//							edgeMarker = CreateEdgeMarker(Source, ToNode.Position);
							edgeBeacon = CreateEdgeBeacon(Source, ToNode.Position);
						}
					}
					else
					{
//						Object.Destroy(edgeMarker);
//						edgeMarker = null;
						Object.Destroy(edgeBeacon);
						edgeBeacon = null;
					}	
				}
				
				if (FromNode != null && ToNode == null)
				{
					if (show)
					{
						if (PathManager.showPath)
						{
//							edgeMarker = CreateEdgeMarker(FromNode.Position, Destination);
							edgeBeacon = CreateEdgeBeacon(FromNode.Position, Destination);
							waypointMarker = CreatePointMarker(Destination);
							waypointBeacon = CreatePointBeacon(Destination);
						}
					}
					else
					{
//						Object.Destroy(edgeMarker);
//						edgeMarker = null;
						Object.Destroy(edgeBeacon);
						edgeBeacon = null;
						Object.Destroy(waypointMarker);
						waypointMarker = null;
						Object.Destroy(waypointBeacon);
						waypointBeacon = null;
					}	
				}
			}
		}
		
		private GameObject CreatePointBeacon(Vector2 point)
		{
			GameObject nodeMarkers = GameObject.Find("Game/NodeMarkers");
			GameObject pointMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);	
			pointMarker.transform.position = World.Instance.GroundPositionAt(point);
			pointMarker.transform.localScale = Vector3.one * Radius + Vector3.up * 5;
			pointMarker.transform.parent = nodeMarkers.transform;
			pointMarker.name = "PointBeacon" + point;
			pointMarker.renderer.enabled = true;
			pointMarker.renderer.material = new Material(MovingObject.renderer.material);
			pointMarker.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			Color color = pointMarker.renderer.material.color;
			color.a = 0.4f;
			pointMarker.renderer.material.color = color;	
			pointMarker.renderer.castShadows = false;
			Object.Destroy(pointMarker.collider);
			return pointMarker;
		}
		
		private GameObject CreateEdgeBeacon(Vector2 startPoint, Vector2 endPoint)
		{
			GameObject edgeMarkers = GameObject.Find("Game/EdgeMarkers");
			GameObject edgeBeacon = GameObject.CreatePrimitive(PrimitiveType.Cube);
			edgeBeacon.name = "EdgeBeacon " + startPoint + " ---> " + endPoint;
			Vector3 startPosition = World.Instance.GroundPositionAt(startPoint);
			Vector3 endPosition = World.Instance.GroundPositionAt(endPoint);
			edgeBeacon.transform.position = (startPosition + endPosition) / 2;		
			edgeBeacon.transform.localScale = 
				new Vector3(0.3f, 0.3f, (endPosition - startPosition).magnitude);
			
	    	Vector2 direction = endPoint - startPoint;
	    	float angle = Vector2.Angle(Vector2.up, direction);
	    	Vector3 cross = Vector3.Cross(Vector2.up, direction);
	    	if (cross.z > 0)
			{
				angle = 360f - angle;
			}
	
			edgeBeacon.transform.eulerAngles = new Vector3(0, angle, 0);
			
			edgeBeacon.transform.parent = edgeMarkers.transform;
			edgeBeacon.renderer.material = new Material(MovingObject.renderer.material);
			edgeBeacon.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			Color color = edgeBeacon.renderer.material.color;
			color.a = 0.4f;
			edgeBeacon.renderer.material.color = color;	
			edgeBeacon.renderer.castShadows = false;
			edgeBeacon.renderer.enabled = true;
			Object.Destroy(edgeBeacon.collider);
			return edgeBeacon;
		}
		
		private GameObject CreatePointMarker(Vector2 point)
		{
			GameObject nodeMarkers = GameObject.Find("Game/NodeMarkers");
			GameObject pointMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);	
			pointMarker.transform.position = World.Instance.GroundPositionAt(point);
			pointMarker.transform.localScale = Vector3.one * Radius * 1.5f;
			pointMarker.transform.parent = nodeMarkers.transform;
			pointMarker.name = "PointMarker" + point;
			pointMarker.renderer.enabled = true;
			pointMarker.renderer.material = MovingObject.renderer.material;
			pointMarker.renderer.castShadows = false;
			Object.Destroy(pointMarker.collider);
			return pointMarker;
		}
		
//		private GameObject CreateEdgeMarker(Vector2 startPoint, Vector2 endPoint)
//		{
//			GameObject edgeMarkers = GameObject.Find("Game/EdgeMarkers");
//			GameObject edgeMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			edgeMarker.name = "EdgeMarker " + startPoint + " ---> " + endPoint;
//			Vector3 startPosition = World.Instance.GroundPositionAt(startPoint);
//			Vector3 endPosition = World.Instance.GroundPositionAt(endPoint);
//			edgeMarker.transform.position = (startPosition + endPosition) / 2;		
//			edgeMarker.transform.localScale = 
//				new Vector3(0.11f, 0.11f, (endPosition - startPosition).magnitude);
//			
//	    	Vector2 direction = endPoint - startPoint;
//	    	float angle = Vector2.Angle(Vector2.up, direction);
//	    	Vector3 cross = Vector3.Cross(Vector2.up, direction);
//	    	if (cross.z > 0)
//			{
//				angle = 360f - angle;
//			}
//	
//			edgeMarker.transform.eulerAngles = new Vector3(0, angle, 0);
//			
//			edgeMarker.transform.parent = edgeMarkers.transform;
//			edgeMarker.renderer.material = MovingObject.renderer.material;
//			edgeMarker.renderer.castShadows = false;
//			edgeMarker.renderer.enabled = true;
//			Object.Destroy(edgeMarker.collider);
//			return edgeMarker;
//		}
	}
}
