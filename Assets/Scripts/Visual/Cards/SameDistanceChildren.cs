using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

// place first and last elements in children array manually
// others will be placed automatically with equal distances between first and last elements
public class SameDistanceChildren : MonoBehaviour 
{

	[FormerlySerializedAs("Children")]
	[SerializeField, Tooltip("First and last elements in this array should be placed manually. " +
	                         "The script will automatically place the other elements with equal distances between the first and last ones.")]
    private Transform[] children;

	// Use this for initialization
	private void Awake () 
    {
	    if (children == null || children.Length == 0)
	    {
		    Debug.LogError("SameDistanceChildren: Children array is not assigned or empty. Please assign the first and last elements in the inspector.");
		    return;
	    }
	    
        Vector3 firstElementPos = children[0].transform.position;
        Vector3 lastElementPos = children[^1].transform.position;

        // dividing by Children.Length - 1 because for example: between 10 points that are 9 segments
        float xDist = (lastElementPos.x - firstElementPos.x)/(children.Length - 1);
        float yDist = (lastElementPos.y - firstElementPos.y)/(children.Length - 1);
        float zDist = (lastElementPos.z - firstElementPos.z)/(children.Length - 1);

        Vector3 dist = new Vector3(xDist, yDist, zDist);

        for (int i = 1; i < children.Length; i++)
            children[i].transform.position = children[i - 1].transform.position + dist;
	}
	
	#region Accessors
	public Transform[] Children => children;
	#endregion
	
}
