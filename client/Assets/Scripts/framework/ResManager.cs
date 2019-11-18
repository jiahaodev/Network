using UnityEngine;

public class ResManager : MonoBehaviour {

	//加载预设
	public static GameObject LoadPrefab(string path){
        string newPath = "Prefab/" + path;
		return Resources.Load<GameObject>(newPath);
	}

}