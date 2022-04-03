using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldStrategy.UI
{
    public class BottomSelect : StaticInstance<BottomSelect>
    {
        [SerializeField] private GameObject parent;
        [SerializeField] private Transform contentParent;
        [ShowAssetPreview, SerializeField] private GameObject itemPrefab;

        public void NewSelect(Sprite[] images, Action<int> callback)
        {
            parent.SetActive(true);
            contentParent.DestroyChildren();

            for (int i = 0; i < images.Length; i++)
            {
                int index = i;

                GameObject temp = Instantiate(itemPrefab);
                temp.transform.SetParent(contentParent);
                temp.transform.localScale = Vector3.one;

                temp.GetComponent<Image>().sprite = images[i];
                temp.GetComponent<Button>().onClick.AddListener(delegate {
                    RemoveSelect();
                    callback(index);
                });
            }
        }

        private void RemoveSelect()
        {
            contentParent.DestroyChildren();
            parent.SetActive(false);
        }
    }
}
