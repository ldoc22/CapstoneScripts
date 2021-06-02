using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SimpleJSON;

public class ItemManager : MonoBehaviour
{
    /*
    Action<string> _createItemsCallback;
    // Start is called before the first frame update
    void OnEnable()
    {
        _createItemsCallback = (jsonArray) => {
            //StartCoroutine(CraeteItemsRoutine(jsonArray));
        };
        CreateItems();
    }

    public void CreateItems()
    {
        string userID = Main.instance.userInfo.UserID;
       // StartCoroutine(Main.instance.web.GetItemsIDs(userID, _createItemsCallback));
    }

    IEnumerator CraeteItemsRoutine(string jsonarray)
    {
        //Debug.Log("test: " + jsonarray);
        JSONArray jsonArray = JSON.Parse(jsonarray) as JSONArray;
        for (int i = 0; i < jsonArray.Count; i++)
        {

            bool isDone = false; //are we done downloading
            string itemId = jsonArray[i].AsObject["itemid"];
            string inventoryID = jsonArray[i].AsObject["id"]; 
            JSONObject itemInfoJson = new JSONObject();
            

            //callback
            Action<string> getItemInfoCallback = (itemInfo) =>
            {
                isDone = true;
                JSONArray tmpArray = JSON.Parse(itemInfo) as JSONArray;
                itemInfoJson = tmpArray[0].AsObject;
            };
           ////// StartCoroutine(Main.instance.web.GetItem(itemId, getItemInfoCallback));

            //wait until callback from web
            yield return new WaitUntil(() => isDone == true);

            //spawn gameobject
            GameObject itemGO = Instantiate(Resources.Load("Prefabs/Item") as GameObject);
            UI_Item item = itemGO.AddComponent<UI_Item>();
            item.ID = inventoryID;
            item.ItemID = itemId;
            itemGO.transform.SetParent(this.transform);
            itemGO.transform.localScale = Vector3.one;
            itemGO.transform.localPosition = Vector3.zero;

            //Fill info
            itemGO.transform.Find("Name").GetComponent<Text>().text = itemInfoJson["name"];
            itemGO.transform.Find("Price").GetComponent<Text>().text = itemInfoJson["price"];
            itemGO.transform.Find("Description").GetComponent<Text>().text = itemInfoJson["description"];


            int imgVer = itemInfoJson["imgver"].AsInt;


            byte[] bytes = ImageManager.instance.LoadImage(itemId,imgVer);
            if (bytes.Length == 0)
            {

                Action<byte[]> getItemIconCallback = (downloadedBytes) =>
                {

                    Sprite sprite = ImageManager.instance.BytesToSprite(downloadedBytes);
                    itemGO.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
                    ImageManager.instance.SaveImage(itemId, downloadedBytes,imgVer);
                    ImageManager.instance.SaveVersionJson();

                };

                ////// StartCoroutine(Main.instance.web.GetItemIcon(item.ItemID, getItemIconCallback));
            }
            else
            {
                Sprite sprite = ImageManager.instance.BytesToSprite(bytes);
                itemGO.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
            }



                //Set Sell Button
                itemGO.transform.Find("SellButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                string iID = itemId;
                string idInInventory = inventoryID;
                string uID = Main.instance.userInfo.UserID;

                //////  StartCoroutine(Main.instance.web.SellItem(idInInventory, iID ,uID)) ;
            });
            


            //to next item
        }
        
        

        yield return null;
    }

  */ 


}
