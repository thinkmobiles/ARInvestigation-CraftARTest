// TestNewApp is free software. You may use it under the MIT license, which is copied
// below and available at http://opensource.org/licenses/MIT
//
// Copyright (c) 2014 Catchoom Technologies S.L.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
// Software, and to permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleCollectionManager : MonoBehaviour, CraftARCollectionManager.CraftARCollectionManagerCallbacks, CraftAROnDeviceCollection.CraftAROnDeviceCollectionCallbacks, CraftARSDK.CraftARSDKCallbacks, CraftARSDK.CraftARItemEvents
{

	public string CollectionToken = "Set your token here";

	void Start () {
		//Set this app as Callback handler in the SDK.
		Debug.Log ("CollectionManager");
		CraftARSDK.instance.setCraftARSDKCallbacksHandler(this);
		CraftARSDK.instance.setCraftARItemEventsHandler (this);
		//Set this app as Callback handler for the CraftARCollectionManager events in the SDK
		CraftARCollectionManager.instance.setCraftARCollectionManagerCallbacksHandler(this);

	}
	
	public void OnGUI()
	{
	}

	//CraftARSDK events:
	void CraftARSDK.CraftARSDKCallbacks.CraftARReady() { 
		//Download collection using a CollectionToken

		Debug.Log ("CraftAR Ready");
		CraftAROnDeviceCollection collection = CraftARCollectionManager.instance.getCollectionWithToken(CollectionToken);
		if (collection == null) {
			Debug.Log ("Add collection");
			CraftARCollectionManager.instance.addCollectionWithToken(CollectionToken);
		} else {
			Debug.Log ("Sync collection");
			collection.setCraftAROnDeviceCollectionCallbacksHandler(this);
			collection.Sync();
		}
	}

	//CollectionManager Events
	void CraftARCollectionManager.CraftARCollectionManagerCallbacks.CollectionAdded(CraftAROnDeviceCollection collection){
		CollectionReady(collection);
	}

	void CollectionReady(CraftAROnDeviceCollection collection) {
		Debug.Log("CollectionAdded:");
		Debug.Log("\tCollection UUID:" + collection.GetUUID());
		Debug.Log("\tCollection Name: " + collection.GetName());
		Debug.Log("\tCollection tokens:" + collection.GetTokens());
		Debug.Log("\tCollection Items:");
		List<string> itemList = collection.ListItems();

		foreach(string itemUUID in itemList){
			Debug.Log("\t\tItem: " + itemUUID);
			CraftARItem item = collection.GetItem(itemUUID);
			Debug.Log("\t\t\tItem Name:" + item.itemName);

			switch (item.ItemType) {
			case CraftARItem.CraftARItemType.AUGMENTED_REALITY_ITEM:
				CraftARItemAR arItem = item as CraftARItemAR;
				Debug.Log ("Found AR Item with name: "+arItem.itemName);
				
				// Add it for tracking, this will trigger the CraftARSDK.CraftARItemEvents.ItemAdded callback on success
				CraftARSDK.instance.AddSceneARItem(arItem);
				break;
			case  CraftARItem.CraftARItemType.IMAGE_RECOGNITION_ITEM:
				Debug.Log ("Found IR Item with name: "+item.itemName);
				break;
			}
		}
	}

	void CraftARCollectionManager.CraftARCollectionManagerCallbacks.AddCollectionFailed(CraftARError error){
		Debug.Log ("AddCollectionFailed: " + error.errorMessage);
	}

	void CraftARCollectionManager.CraftARCollectionManagerCallbacks.AddCollectionProgress(float progress){
		Debug.Log ("AddCollectionProgress: " + progress);
	}

	void CraftARCollectionManager.CraftARCollectionManagerCallbacks.DeleteCollectionFailed(CraftARError error){
		Debug.Log ("DeleteCollectionFailed: " + error.errorMessage);
	}

	void CraftARCollectionManager.CraftARCollectionManagerCallbacks.CollectionDeleted(){
		Debug.Log ("CollectionDeleted: ");
	}

	//Collection Events
	void CraftAROnDeviceCollection.CraftAROnDeviceCollectionCallbacks.SyncFinished(CraftAROnDeviceCollection collection){
		Debug.Log ("Sync Done!");
		CollectionReady(collection);
	}

	void CraftAROnDeviceCollection.CraftAROnDeviceCollectionCallbacks.SyncProgress(CraftAROnDeviceCollection collection, float progress){
		Debug.Log ("SyncProgress: " + collection.GetName() + " Progress: " + progress);
	}

	void CraftAROnDeviceCollection.CraftAROnDeviceCollectionCallbacks.SyncFailed(CraftAROnDeviceCollection collection, CraftARError error){
		Debug.Log ("SyncFailed: " + collection.GetName() + " Error:" + error.errorMessage);
	}

	GameObject contentPrefab = null;

	bool isTracking = false;
	void CraftARSDK.CraftARItemEvents.ItemAdded(CraftARItemAR item) {
		Debug.Log("Item Added: " + item.itemName);
		if (!isTracking) {
			CraftARSDK.instance.startTracking();
			isTracking = true;
		}
		if (contentPrefab == null ) {
			contentPrefab = Resources.Load ("Examples/ExampleLocalContent/ARItem_LocalARItem",typeof(GameObject)) as GameObject;
		}
		item.contentInstance =  GameObject.Instantiate(contentPrefab,contentPrefab.transform.position, contentPrefab.transform.rotation) as GameObject;
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

	void CraftARSDK.CraftARSDKCallbacks.SearchResults (List<CraftARSearchResult> results)
	{
		// Using local AR items so, nothing to do here
	}
	void CraftARSDK.CraftARSDKCallbacks.SearchError(CraftARError error)
	{
		// Using local AR items so, nothing to do here
	}
	void CraftARSDK.CraftARSDKCallbacks.CollectionReady ()
	{
		// Using local AR items so, nothing to do here
	}
	void CraftARSDK.CraftARSDKCallbacks.TokenValidationError(CraftARError error)
	{
		// Using local AR items so, nothing to do here
	}
}