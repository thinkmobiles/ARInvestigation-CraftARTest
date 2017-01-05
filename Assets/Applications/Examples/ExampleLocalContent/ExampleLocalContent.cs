using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
This example shows how to load a content pre-loaded in the app Resources/ folder. By doing this, you can skip network delays
and save network usage, but your application will be bigger. 

To use it, run the App, point to your reference image. When any of your AR items in your collection is found,
this app will show the AR experience with the pre-loaded content.
 */
public class ExampleLocalContent : MonoBehaviour, CraftARSDK.CraftARSDKCallbacks, CraftARSDK.CraftARItemEvents {

	public string CollectionToken = "Put your token here";

	void Start () {
		//Set this app as Callback handler in the SDK.
		CraftARSDK.instance.setCraftARSDKCallbacksHandler(this);
		CraftARSDK.instance.setCraftARItemEventsHandler (this);
	}
	
	//CraftARSDK events:
	void CraftARSDK.CraftARSDKCallbacks.CraftARReady() { 
		//#warning Set your collection token!
        CraftARSDK.instance.setToken(CollectionToken);
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
			Debug.Log ("Found IR Item with name: "+bestMatch.itemName);
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

	void CraftARSDK.CraftARItemEvents.ItemAdded(CraftARItemAR item) {
		CraftARSDK.instance.startTracking();
		GameObject content = Resources.Load ("Examples/ExampleLocalContent/ARItem_LocalARItem",typeof(GameObject)) as GameObject;
		item.contentInstance =  GameObject.Instantiate(content,content.transform.position, content.transform.rotation) as GameObject;
	}

	void CraftARSDK.CraftARItemEvents.AddItemError(CraftARItemAR item, CraftARError error) {
		Debug.Log("Error Adding item: " + item.itemName + " Message: " + error.errorMessage);
	}
	
	void CraftARSDK.CraftARItemEvents.TrackingStarted(CraftARItemAR item) {
		Debug.Log ("Tracking started: " + item.itemName);
	}
	void CraftARSDK.CraftARItemEvents.TrackingLost(CraftARItemAR item) {
		Debug.Log ("Tracking lost: " + item.itemName);
	}

	void CraftARSDK.CraftARSDKCallbacks.PictureTaken(Texture2D image) {
	}
	
	void CraftARSDK.CraftARSDKCallbacks.TakePictureFailed() {
	}
}
