using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModelSwitch : MonoBehaviour
{
    public Transform[] ts;

    public GameObject MaleParts, FemaleParts;

    public characterCreationItem[] lists;

    // Start is called before the first frame update
    void Start()
    {
        
        //ActivatePart("Male_04_Arm_Upper_Right", 1, true);

        
       

    }

    public void MalePartsActivate()
    {
        lists = new characterCreationItem[MaleParts.transform.childCount];
        lists[0] = new characterCreationItem(MaleParts.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Transform>(), "Head");
        for (int i = 1; i < lists.Length; i++)
        {
            Transform t = MaleParts.transform.GetChild(i);
            lists[i] = new characterCreationItem(t.GetComponentsInChildren<Transform>(), t.name);
        }
    }

    public void FemalePartsActivate()
    {
        lists = new characterCreationItem[FemaleParts.transform.childCount];
        lists[0] = new characterCreationItem(FemaleParts.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Transform>(), "Head");
        for (int i = 1; i < lists.Length; i++)
        {
            Transform t = FemaleParts.transform.GetChild(i);
            lists[i] = new characterCreationItem(t.GetComponentsInChildren<Transform>(), t.name);
        }
    }




   

    [System.Serializable]
    public class characterCreationItem{

        int currentIndex;
        GameObject[] items;
        public string name;

        public characterCreationItem(GameObject [] _GOs)
        {
            items = _GOs;
            DisableAll();
            RandomStartIndex(items.Length);
        }

        public characterCreationItem(Transform [] _t, string _name)
        {
            name = _name;
            items = new GameObject[_t.Length-1];
            for (int i = 0; i < items.Length; i++)
            {
                
                items[i] = _t[i+1].gameObject;
            }
            DisableAll();
            RandomStartIndex(items.Length);
        }
        public void RandomStartIndex(int size)
        {
            currentIndex = Random.Range(0, size);
            items[currentIndex].SetActive(true);
        }
        public void DisableAll()
        {
            foreach(GameObject g in items)
            {
                g.SetActive(false);
            }
        }

        public void nextItem(bool _increase)
        {
            int old = currentIndex;
            if (_increase)
            {
                if (currentIndex <= items.Length - 1)
                    currentIndex = 0;
                else
                {
                    currentIndex++;
                }
            }
            else
            {
                if(currentIndex == 0)
                {
                    currentIndex = items.Length - 1;
                }
                else
                {
                    currentIndex--;
                }
            }
            ActivateItem(currentIndex, old);
        }

        void ActivateItem(int _newInx, int _oldInx)
        {
            items[_oldInx].SetActive(false);
            items[_newInx].SetActive(true);
            
        }


        }

}
