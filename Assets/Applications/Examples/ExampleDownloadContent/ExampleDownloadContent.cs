using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
This example shows how to use the OnItemDownloadProgress and OnItemDownloadFinished callbacks 
to show a 3DText label updating with the download percentage when the contents are being downloaded.
The 3DText label is an ARitem content created in the editor (CraftAR-> Create -> Empty AR Item) and 
stored in the Resources/ folder of the Unity project. 

To use it, run the App, point to your reference image. While the content bundle is being downloaded, 
you will see a 3DText label tracking the reference image.
 */
public class ExampleDownloadContent : MonoBehaviour, CraftARSDK.CraftARSDKCallbacks, CraftARSDK.CraftARItemEventsWithContentDownload {

	public string CollectionToken = "Put your token here";

	GameObject loading3DtextARItem;
	TextMesh loadingText;
	void Start () {
		//Set this app as Callback handler in the SDK.
		CraftARSDK.instance.setCraftARSDKCallbacksHandler(this);
		//Set this app as Callback handler for the CraftARItem events in the SDK
		CraftARSDK.instance.setCraftARItemEventsHandler(this);
	}

	//CraftARSDK events:
	void CraftARSDK.CraftARSDKCallbacks.CraftARReady() { 
		//#warning Set your collection token!
        CraftARSDK.instance.setToken(CollectionToken);
		//CraftARSDK.instance.EmbedItemCustomData (true);
		//CraftARSDK.instance.RequestBoundingBoxes(true);
	}
	void CraftARSDK.CraftARSDKCallbacks.SearchResults(List<CraftARSearchResult> results){
		if (results.Count <= 0) {
			//Nothing found!
			Debug.Log("Results is empty");
			return;
		}
		//Something was found! We stop the finder mode.
		CraftARSDK.instance.stopFinderMode();

		//As an example, we take only the best match (the first in the list of matches). 
		CraftARItem bestMatch = results [0].Item;
		switch (bestMatch.ItemType) {
		case CraftARItem.CraftARItemType.AUGMENTED_REALITY_ITEM:
			CraftARItemAR arItem = bestMatch as CraftARItemAR;
			Debug.Log ("Found AR Item with name: "+arItem.itemName);

			// Add it for tracking, this will trigger the CraftARSDK.CraftARItemEvents.ItemAdded callback on success
			CraftARSDK.instance.AddSceneARItem(arItem);
			break;
		case  CraftARItem.CraftARItemType.IMAGE_RECOGNITION_ITEM:
			Debug.Log ("Found IR Item with name:"+bestMatch.itemName);
			Debug.Log("Item url: " + bestMatch.itemUrl);
			Debug.Log("Custom data: " + bestMatch.custom);
			CraftARSearchResult.CraftARBoundingBox bbox = results[0].BoundingBox;
			if (bbox == null) {
				Debug.Log("NO BBOX");
			} else {
				Debug.Log("Bounding box: TL(" + bbox.topLeftX + "," + bbox.topLeftY + ") BR(" + bbox.bottomRightX +"," + bbox.bottomRightY+")");
			}
			break;
		} 
	}
	void CraftARSDK.CraftARSDKCallbacks.SearchError(CraftARError error) {
		Debug.Log("Search error: "+error.errorMessage);
	}
	void CraftARSDK.CraftARSDKCallbacks.CollectionReady() {
		Debug.Log("token OK");
		CraftARSDK.instance.startFinderMode ();
	}
	void CraftARSDK.CraftARSDKCallbacks.TokenValidationError(CraftARError error) {
		Debug.Log("Token validation error: " + error.errorMessage);
	}
	
	// Item events:
	void CraftARSDK.CraftARItemEvents.TrackingStarted(CraftARItemAR item) {
		Debug.Log("TrackingStarted: "+ item.itemName);
	}
	void CraftARSDK.CraftARItemEvents.TrackingLost(CraftARItemAR item) {
		Debug.Log("TrackingLost: "+ item.itemName);
	}

	void CraftARSDK.CraftARItemEvents.ItemAdded (CraftARItemAR item)
	{
			//Start tracking and download item contents. During download, we change the ARItem content for a text label we have pre-loaded.
			CraftARSDK.instance.startTracking ();
			//Change the ARItem content to a 3DText content during download.
			GameObject loading3DTextARItemPrefab = Resources.Load ("Examples/ExampleDownloadContent/ARItem_Loading3DTextContent", typeof(GameObject)) as GameObject;
			loading3DtextARItem = GameObject.Instantiate (loading3DTextARItemPrefab, loading3DTextARItemPrefab.transform.position, loading3DTextARItemPrefab.transform.rotation) as GameObject;
			item.contentInstance = loading3DtextARItem;
			loadingText = GameObject.Find ("Loading3DText").GetComponent<TextMesh> ();
			//Download the bundle from this ARItem, and automatically enable it when download finishes.
			CraftARSDK.instance.DownloadItemContents (item, true);
			downloading = true;
	}

	void CraftARSDK.CraftARItemEvents.AddItemError (CraftARItemAR item, CraftARError error)
	{
			Debug.Log ("Error Adding item: " + item.itemName + " Message: " + error.errorMessage);
	}

	void CraftARSDK.CraftARItemEventsWithContentDownload.OnItemContentDownloadProgress (CraftARItemAR item, float progress)
	{
			Debug.Log ("Download progress: " + progress + " - " + item.itemName);
			loadingText.text = "Loading... " + (int)(progress * 100) + "%";
	}

	void CraftARSDK.CraftARItemEventsWithContentDownload.OnItemContentDownloadFinished (CraftARItemAR item)
	{
			Debug.Log ("Download Finished!  " + item.itemName);
			if (loading3DtextARItem != null) {
					loadingText.text = "";
					Destroy (loading3DtextARItem);
			}
			downloading = false;
			contentsDownloaded = true;
	}

	void CraftARSDK.CraftARItemEventsWithContentDownload.OnItemContentDownloadFailed (CraftARItemAR item, CraftARError error)
	{
			Debug.Log ("Download error: " + error.errorMessage);
			Destroy (loading3DtextARItem);
			downloading = false;
	}

	void CraftARSDK.CraftARSDKCallbacks.PictureTaken(Texture2D image) {
	}
	
	void CraftARSDK.CraftARSDKCallbacks.TakePictureFailed() {
	}


	bool contentsDownloaded = false;
	bool downloading = false;

	void OnGUI ()
	{
			if (contentsDownloaded & (!downloading)) {
					if (GUI.Button (new Rect (0, 0, 300, 100), "Clear Scene")) {
							CraftARSDK.instance.stopTracking (); // Appears to be necesseary, because without it, RemoveAllARItems will cause errors to be printed all the time
							CraftARSDK.instance.RemoveALLARItems (); // Clean up so we get memory back for recognizing other stuff
					}
			}
			if (contentsDownloaded & (!downloading)) {
					if (GUI.Button (new Rect (0, 100, 300, 100), "Scan Again")) {
							CraftARSDK.instance.startFinderMode (); // Resume scanning for new stuff
					}
			}
	}

}
