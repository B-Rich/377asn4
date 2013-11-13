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
using UnityEngine;
using System.Collections;
using Thot.GameAI;
using System.Collections.Generic;
//using System.Collections;

[AddComponentMenu("Scripts/Managers/Path Manager")]

public struct PathRequestEventPayload
{
	public GameObject gameObject;
	public Vector2 destination;
	
	public PathRequestEventPayload(
        GameObject gameObject,
        Vector2 destination)
    {
        this.gameObject = gameObject;
        this.destination = destination;
    }
}

public struct PathReadyEventPayload
{
	public GameObject gameObject;
	public Path path;
	
	public PathReadyEventPayload(
        GameObject gameObject,
        Path path)
    {
        this.gameObject = gameObject;
        this.path = path;
    }
}

public class PathManager : MonoBehaviour {
	
	private static PathManager _instance;
	
	public Queue<Event<PathRequestEventPayload>> reqQueue;

	public bool searchSpaceCanChange;
	public bool requestPath;
	public SearchSpace searchSpace;
	public int currentSource;
	public bool ignoreObstructions;

	public bool showPath = true;


	public static PathManager Instance
	{
		get {return _instance;}
	}
	
	public void Awake()
	{
		if (_instance != null)
		{
			Debug.Log("Multiple instances of PathManager!");
		}

		_instance = this;
	
	}
	// Use this for initialization
	void Start () 
	{
		SetSearchSpace();
		reqQueue = new Queue<Event<PathRequestEventPayload>>();
		Debug.Log("Queue created");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	//Debug.Log("Path Manager Update");
		if (searchSpaceCanChange)
		{
			SetSearchSpace();
		}
		
		if (reqQueue.Count > 0){
			Debug.Log ("Queue count" + reqQueue.Count);
			//SetSearchSpace();
			FindPath();
		}	
		if (searchSpace == null || !searchSpace.enabled)
		{
			Debug.Log("No Search Space");
			return;
		}
		
		// This is for manual testing via the Inspector
		if (requestPath)
		{
			PathRequestEventPayload request = 
				new PathRequestEventPayload(searchSpace.gameObject, searchSpace.GetRandomEntityPosition());
			EventManager.Instance.Enqueue<PathRequestEventPayload>(Events.PathRequest, request);
			requestPath = false;
		}
		
	}
	
	
	private void OnEnable()
	{
		EventManager.Instance.Subscribe<PathRequestEventPayload>(Events.PathRequest, OnPathRequest);
	}
	
	private void OnDisable()
	{
		EventManager.Instance.Unsubscribe<PathRequestEventPayload>(Events.PathRequest, OnPathRequest);
	}
	
	private void FindPath()
	{
		
		Debug.Log("Finding Path");
		
		if (reqQueue.Count > 0)
		{
			
			if (searchSpace == null)
			{
				return;
			}
		
			PathRequestEventPayload request = reqQueue.Dequeue().EventData;
			
			Debug.Log("Finding Path request");
			
			if (request.gameObject != searchSpace.gameObject)
			{
				return; // request not for us
			}
			
			Debug.Log("Finding Path is for us");
			
//			Transform weebleTransform = GameObject.Find("Agents/Weebles").transform;
			
			//MovingEntity movingEntity = weebleTransform.GetComponent<MovingEntity>();
			MovingEntity movingEntity = request.gameObject.GetComponent<MovingEntity>();
			
			Debug.Log("Ent X: " + movingEntity.Position2D.x  + "Y: " + movingEntity.Position2D.y);
			
			Vector2 requestorPosition2D = (movingEntity != null && movingEntity.enabled) ? movingEntity.Position2D : request.gameObject.transform.position.To2D();
		
			Debug.Log("Pos X: " + requestorPosition2D.x  + "Y: " + requestorPosition2D.y);
			
			int source = searchSpace.GetClosestNodeToPosition(requestorPosition2D);
		
			if (source != Node.INVALID_NODE_INDEX)
			{
				// Requestor may be inside or too close to obstruction
        	    // so let's find the closest node to warp to
				source = searchSpace.GetClosestNodeToPosition(requestorPosition2D, true);
        	    if (source == SearchSpace.NO_CLOSEST_NODE_FOUND)
        	    {
					Debug.Log("No closest node found 1");
        	        return; // screwed
        	    }
			}
		
			int target = searchSpace.GetClosestNodeToPosition(request.destination);
    	    if (target == SearchSpace.NO_CLOSEST_NODE_FOUND)
    	    {
    	        ////TODO: should we instead move the target to closest valid node??
				Debug.Log("No closest node found 1");
    	        return;
    	    }
			
			var currentSearch = new AStarSearch(searchSpace.Graph, source, target);
			
			Debug.Log("Finding Path: done search");
			
			var path = new Path(
			        this,
			        searchSpace.gameObject,
        	        requestorPosition2D, 
        	        request.destination, 
        	        currentSearch.GetPathToTarget(),
        	        searchSpace.Graph);
			
			Debug.Log("Finding Path: created path");
			
			PathReadyEventPayload result =
				new PathReadyEventPayload(request.gameObject, path);
			EventManager.Instance.Enqueue<PathReadyEventPayload>(Events.PathReady, result); 	
			
			Debug.Log("Finding Path: done");
			
		}
	}
	
	private void OnPathRequest(Event<PathRequestEventPayload> eventArg) //when path request, queue it, then on update, find path for first in queue
	{
		reqQueue.Enqueue(eventArg);
	}

	private void SetSearchSpace()
	{
		
		if (searchSpace == null || !searchSpace.enabled)
		{
			foreach (SearchSpace ss in GetComponents<SearchSpace>())
			{
				if (ss.enabled)
				{
					searchSpace = ss;
					break;
				}
			}
		}
		
		//searchSpace = reqQueue.Peek ().EventData.gameObject.GetComponentInChildren<SearchSpace>();
		
	}
	
}
