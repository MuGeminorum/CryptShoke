using UnityEditor;
using UnityEngine;
using System.Collections;

public class CurvesTransferer {
	
	const string duplicatePostfix = "_copy";
	
	[MenuItem ("Assets/Transfer Clip Curves to Copy")]
	static void CopyCurvesToDuplicate () {
        // Get selected AnimationClip
        AnimationClip imported = Selection.activeObject as AnimationClip;
        if (imported == null) {
        	Debug.Log("Selected object is not an AnimationClip");
        	return;
        }
        
        // Find path of copy
        string importedPath = AssetDatabase.GetAssetPath(imported);
        string copyPath = importedPath.Substring(0, importedPath.LastIndexOf("/"));
        copyPath += "/" + imported.name + duplicatePostfix + ".anim";
        
        // Get copy AnimationClip
        AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
        if (copy == null) {
        	Debug.Log("No copy found at "+copyPath);
        	return;
        }
        
        // Copy curves from imported to copy
        AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(imported, true);
        for (int i=0; i<curveDatas.Length; i++) {
        	AnimationUtility.SetEditorCurve(
        		copy,
        		curveDatas[i].path,
        		curveDatas[i].type,
        		curveDatas[i].propertyName,
        		curveDatas[i].curve
        	);
        }
        
        Debug.Log("Copying curves into "+copy.name+" is done");
	}
}
