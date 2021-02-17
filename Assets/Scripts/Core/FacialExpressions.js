#pragma strict

var smr : SkinnedMeshRenderer;

var ctrlBocaCanto_L : GameObject;
var ctrlLabioCentroInfer : GameObject;
var ctrlLabioCentroSuper : GameObject;
var ctrlBocaCanto_R : GameObject;

var ctrlSobrancelha_L : GameObject;
var ctrlSobrancelha_R : GameObject;

var ctrlSobrancCentro : GameObject;
var ctrlBochecha_L : GameObject;
var ctrlBochecha_R : GameObject;

var BnMandibula : GameObject;

function Start () {
    
	smr = gameObject.GetComponent(SkinnedMeshRenderer);
	
	ctrlBocaCanto_L = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnBocaCanto_L/");
	ctrlBocaCanto_R = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnBocaCanto_R/");
	ctrlLabioCentroInfer = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnLabioCentroInfer/");
	ctrlLabioCentroSuper = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnLabioCentroSuper/");		
	
	ctrlSobrancCentro = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnSobrancCentro/");
	ctrlSobrancelha_L = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnSobrancLateral_L/");
	ctrlSobrancelha_R = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnSobrancLateral_R/");
	
	ctrlBochecha_L = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnBochecha_L/");
	ctrlBochecha_R = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnBochecha_R/");
	
	BnMandibula = GameObject.Find("Avatar/Armature_001/BnBacia_001/BnCol-01/BnCol-02/BnCol-03/BnPescoco/BnCabeca/BnMandibula/");
}

function Update () {
	// CORRECAO ABRE BOCA
//	smr.SetBlendShapeWeight(16, (BnMandibula.transform.localEulerAngles.y - 90.01227 ) * -100000 ); //ERRO NO SHAPE KEY

	// SORRISO - BOCA CANTO ESQUERDO
	smr.SetBlendShapeWeight( 0, (ctrlBocaCanto_L.transform.localPosition.x + 0.327 ) *-1000 );
	smr.SetBlendShapeWeight(19, (ctrlBocaCanto_L.transform.localPosition.x + 0.327 ) * 1000 );
	
	// LABIO INFERRIOR
	smr.SetBlendShapeWeight(10, (ctrlLabioCentroInfer.transform.localPosition.x + 0.284 ) * 1000 );
	smr.SetBlendShapeWeight(17, (ctrlLabioCentroInfer.transform.localPosition.x + 0.284 ) *-1000 );
		
	// LABIO SUPERIOR
	smr.SetBlendShapeWeight( 7, (ctrlLabioCentroSuper.transform.localPosition.x + 0.4202 ) *-1000 );
	smr.SetBlendShapeWeight( 8, (ctrlLabioCentroSuper.transform.localPosition.x + 0.4202 ) * 1000 );
	
	// BOCA CANTO DIREITO
	smr.SetBlendShapeWeight(20, (ctrlBocaCanto_R.transform.localPosition.y - 3.89 ) * 1000 );
	
	// SOBRANCELHA ESQUERDA -------------------------------------------------------------
	smr.SetBlendShapeWeight(13, (ctrlSobrancelha_L.transform.localPosition.x + 1.248 ) *-1000 );
	smr.SetBlendShapeWeight( 6, (ctrlSobrancelha_L.transform.localPosition.x + 1.248 ) * 1000 );
	smr.SetBlendShapeWeight( 3, (ctrlSobrancelha_L.transform.localEulerAngles.z - 61.60743 ) * 10 );
	smr.SetBlendShapeWeight( 2, (ctrlSobrancelha_L.transform.localEulerAngles.z - 61.60743 ) *-10 );
	
	// SOBRANCELHA CENTRO
	smr.SetBlendShapeWeight(18, (ctrlSobrancCentro.transform.localPosition.x + 1.064 ) * 1000 );
	
	// SOBRANCELHA DIREITA --------------------------------------------------------------
	smr.SetBlendShapeWeight(12, (ctrlSobrancelha_R.transform.localPosition.x + 1.248 ) *-1000 );
	smr.SetBlendShapeWeight( 5, (ctrlSobrancelha_R.transform.localPosition.x + 1.248 ) * 1000 );
	smr.SetBlendShapeWeight( 4, (ctrlSobrancelha_R.transform.localEulerAngles.z - 118.3932 ) *-10 );
	smr.SetBlendShapeWeight( 1, (ctrlSobrancelha_R.transform.localEulerAngles.z - 118.3932 ) * 10 );
	
	// BOCHECHA ESQUERDA
	smr.SetBlendShapeWeight( 9, (ctrlBochecha_L.transform.localPosition.x + 0.307 ) *-1000 );
	smr.SetBlendShapeWeight(14, (ctrlBochecha_L.transform.localPosition.y - 0.604 ) *-10000 );	//duvidas
	
	// BOCHECHA DIREITA
	smr.SetBlendShapeWeight(11, (ctrlBochecha_R.transform.localPosition.y - 0.604 ) * 10000 );
	smr.SetBlendShapeWeight(15, (ctrlBochecha_R.transform.localPosition.y - 0.604 ) *-10000 );	

	// print("X: "+Selection.activeTransform.localPosition.x);
	// print("Y: "+Selection.activeTransform.localPosition.y);	
			
//	print("X: "+Selection.activeTransform.localPosition.x);
//	print("Y: "+Selection.activeTransform.localPosition.y);
//	Debug.Log( (ctrlSobrancelha_L.transform.position.y ) );
//	Debug.Log( (ctrlSobrancelhaEsq.transform.position.y - 4.8) *1000 );
	
}

