using UnityEngine;
using UnityEditor;

public class MeshPostprocessor : AssetPostprocessor {
	
	void OnPreprocessModel (){

		// (assetImporter as ModelImporter).globalScale = 1.0f;
		(assetImporter as ModelImporter).animationType = ModelImporterAnimationType.Legacy;
		(assetImporter as ModelImporter).materialName = ModelImporterMaterialName.BasedOnMaterialName;

	}

}